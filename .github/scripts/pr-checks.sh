#!/usr/bin/env bash
# Kontrole PR uruchamiane przez .github/workflows/pr-checks.yml.
# Lokalnie: bash .github/scripts/pr-checks.sh <kontrola> [baza]
#   kontrola: meta | conflicts | large-files | project-version
#   baza:     ref, z którym porównujemy zmiany (domyślnie origin/main)
# Sprawdzany jest stan zacommitowany (HEAD), nie katalog roboczy ani indeks.

set -euo pipefail

CHECK="${1:?Podaj kontrolę: meta | conflicts | large-files | project-version}"
BASE="${2:-origin/main}"
MAX_SIZE_MB="${MAX_SIZE_MB:-5}"

git() { command git -c core.quotePath=false "$@"; }

# Format adnotacji GitHuba; lokalnie to po prostu czytelny wiersz.
error()   { echo "::error file=$1${3:+,line=$3}::$2"; }
warning() { echo "::warning file=$1::$2"; }
fail()    { echo "::error::$1"; exit 1; }

# Pliki dodane lub zmienione w PR względem bazy, w zmiennej CHANGED (jeden na wiersz).
# Błąd Gita kończy kontrolę porażką: pusta lista nie może znaczyć "nic nie sprawdzono, więc OK".
load_changed() {
    git rev-parse --verify --quiet "$BASE^{commit}" > /dev/null \
        || fail "Nie znaleziono bazy porównania '$BASE'. Pobierz ją (git fetch) albo podaj inny ref."
    CHANGED=$(git diff --name-only --diff-filter=ACMR "$BASE"...HEAD) \
        || fail "Nie udało się ustalić zmian względem '$BASE'."
}

# Wywołuje funkcję $1 dla każdego zmienionego pliku.
each_changed() {
    local file
    while IFS= read -r file; do
        [ -n "$file" ] && "$1" "$file"
    done <<< "$CHANGED"
}

check_meta() {
    # Unity pomija pliki i foldery zaczynające się od "." lub kończące "~", więc nie mają .meta.
    git ls-tree -r --name-only HEAD -- Assets | awk '
        function ignored(name) { return name ~ /^\./ || name ~ /~$/ }
        {
            n = split($0, part, "/")
            # Każdy folder w Assets/ ma swój .meta, także taki, w którym jest tylko .gitkeep.
            path = part[1]
            for (i = 2; i < n; i++) {
                if (ignored(part[i])) next
                path = path "/" part[i]
                assets[path] = 1
            }
            if (ignored(part[n])) next
            if ($0 ~ /\.meta$/) metas[substr($0, 1, length($0) - 5)] = 1
            else assets[$0] = 1
        }
        END {
            bad = 0
            for (a in assets)
                if (!(a in metas)) {
                    printf "::error file=%s::Brak pliku %s.meta. Otwórz projekt w Unity, żeby go wygenerował, i dodaj go do commita.\n", a, a
                    bad = 1
                }
            for (m in metas)
                if (!(m in assets)) {
                    printf "::error file=%s.meta::Osierocony .meta: %s nie istnieje (albo to pusty folder). Usuń ten .meta.\n", m, m
                    bad = 1
                }
            exit bad
        }'
    echo "Pliki .meta w porządku."
}

check_conflicts() {
    load_changed
    [ -z "$CHANGED" ] && { echo "Brak zmienionych plików."; return 0; }

    local files hits status=0
    mapfile -t files <<< "$CHANGED"
    # -I pomija pliki binarne. "=======" nie sprawdzamy, bo bywa zwykłym tekstem (np. w Markdownie).
    # git grep: 0 = są trafienia, 1 = brak trafień, więcej = błąd.
    hits=$(git grep -I -nE '^(<<<<<<<|>>>>>>>)( |$)' HEAD -- "${files[@]}") || status=$?
    [ "$status" -gt 1 ] && fail "git grep zakończył się błędem ($status)."
    if [ -n "$hits" ]; then
        while IFS=: read -r _ file line _; do
            error "$file" "Znacznik konfliktu merge. Dokończ rozwiązywanie konfliktu." "$line"
        done <<< "$hits"
        return 1
    fi
    echo "Brak znaczników konfliktu."
}

# Duże pliki poza LFS i spójność LFS. Rozmiar sprawdzamy tylko w zmienionych plikach, ale spójność
# LFS w całym drzewie: zmiana reguł w .gitattributes nie ruszy plików, które już są w repo.
check_large_files() {
    command git lfs version > /dev/null 2>&1 || fail "Brak git-lfs, a jest potrzebny do sprawdzania wskaźników."
    load_changed

    local -A size oid lfs
    local tree info mode type id sz path bad=0

    # Rozmiar i obiekt każdego pliku w HEAD. Submoduły (type "commit") pomijamy.
    tree=$(git ls-tree -r -l HEAD) || fail "Nie udało się odczytać drzewa HEAD."
    while IFS=$'\t' read -r info path; do
        read -r mode type id sz <<< "$info"
        [ "$type" = blob ] || continue
        size[$path]=$sz
        oid[$path]=$id
    done <<< "$tree"

    # Które pliki są objęte LFS według .gitattributes z HEAD.
    local attrs
    attrs=$(printf '%s\n' "${!size[@]}" | git check-attr --source=HEAD --stdin filter) \
        || fail "Nie udało się odczytać atrybutów LFS."
    while IFS= read -r path; do
        lfs[$path]=1
    done < <(sed -n 's/: filter: lfs$//p' <<< "$attrs")

    for path in "${!lfs[@]}"; do
        sz=${size[$path]}
        # Pusty plik jest swoim własnym wskaźnikiem; wskaźnik zawsze ma mniej niż 1024 bajty.
        [ "$sz" -eq 0 ] && continue
        if [ "$sz" -ge 1024 ] || ! git cat-file blob "${oid[$path]}" | command git lfs pointer --check --strict --stdin > /dev/null 2>&1; then
            error "$path" "Plik pasuje do reguły LFS w .gitattributes, ale nie jest poprawnym wskaźnikiem LFS (trafił do repo z pominięciem LFS). Uruchom: git lfs install, potem git add --renormalize \"$path\" i zacommituj."
            bad=1
        fi
    done

    # Odwrotny przypadek: wskaźnik LFS w pliku, którego żadna reguła nie obejmuje (np. po usunięciu
    # reguły). Unity dostałoby wtedy tekst wskaźnika zamiast assetu.
    local candidates status=0
    candidates=$(git grep -l -I -F 'version https://git-lfs.github.com/spec/v1' HEAD) || status=$?
    [ "$status" -gt 1 ] && fail "git grep zakończył się błędem ($status)."
    while IFS= read -r path; do
        path=${path#HEAD:}
        [ -z "$path" ] || [ -n "${lfs[$path]:-}" ] && continue
        if git cat-file blob "${oid[$path]}" | command git lfs pointer --check --stdin > /dev/null 2>&1; then
            error "$path" "Plik jest wskaźnikiem LFS, ale żadna reguła w .gitattributes go nie obejmuje. Przywróć regułę LFS dla tego typu pliku."
            bad=1
        fi
    done <<< "$candidates"

    check_size() {
        local path=$1
        [ -n "${lfs[$path]:-}" ] && return 0
        if [ "${size[$path]}" -gt $((MAX_SIZE_MB * 1024 * 1024)) ]; then
            error "$path" "Plik ma $(awk -v s="${size[$path]}" 'BEGIN { printf "%.1f", s / 1048576 }') MB, a limit poza LFS to $MAX_SIZE_MB MB. Dodaj jego rozszerzenie do LFS w .gitattributes."
            bad=1
        fi
    }
    each_changed check_size

    [ "$bad" -eq 0 ] && echo "LFS w porządku, brak dużych plików poza LFS."
    return "$bad"
}

check_project_version() {
    load_changed
    local file=ProjectSettings/ProjectVersion.txt
    if grep -qxF "$file" <<< "$CHANGED"; then
        local old new
        old=$(git show "$BASE:$file" 2> /dev/null | sed -n 's/^m_EditorVersion: //p') || old="(brak pliku)"
        new=$(git show "HEAD:$file" | sed -n 's/^m_EditorVersion: //p')
        warning "$file" "PR zmienia wersję Unity z $old na $new. Cały zespół musi przejść na tę wersję. Jeśli to niezamierzone, przywróć plik z main."
        if [ -n "${GITHUB_STEP_SUMMARY:-}" ]; then
            echo "### ⚠️ Zmiana wersji Unity: \`$old\` → \`$new\`" >> "$GITHUB_STEP_SUMMARY"
        fi
    else
        echo "Wersja Unity bez zmian."
    fi
}

case "$CHECK" in
    meta)            check_meta ;;
    conflicts)       check_conflicts ;;
    large-files)     check_large_files ;;
    project-version) check_project_version ;;
    *) echo "Nieznana kontrola: $CHECK" >&2; exit 2 ;;
esac
