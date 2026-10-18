# Git i pull requesty

## Branche

- `main` zawsze się otwiera i działa. Nie pushujemy na niego bezpośrednio.
- Branch zakładamy z aktualnego `main`, według wzoru `<typ>/<nr-issue>-<krotki-opis>`. Nazwa jest pisana po angielsku małymi literami. Przykład: `feature/12-vfx-blood-orb`.
  - typy: `feature`, `fix`, `art`, `level`, `refactor`, `docs`
- Branch powinien żyć krótko, od kilku dni do tygodnia (im dłużej czekasz ze zmianami tym więcej konfliktów możesz mieć przy próbie zrobienia merge).
- Gdy `main` pójdzie do przodu, robimy `git merge main` na swoim branchu.
- Po merge'u PR branch jest usuwany i nie pracujemy na nim dalej.

> [!NOTE]
> **Branch** to osobna linia zmian. Pracujesz na niej, nie przeszkadzając innym, a do `main` trafia ona dopiero przez pull request. Numer (ID) issue w nazwie pozwala od razu znaleźć zadanie, którego branch dotyczy.

## Commity

- Na branchu commitujemy często (mamy historię zmian, łatwo cofnąć się gdy coś pójdzie nie tak). Opisy powinny odzwierciedlać co zostało zmienione, bo może to pomóc reviewerowi, ale nie muszą być idealne i komentować każdej drobnostki.
- Asset zawsze commitujemy razem z jego plikiem `.meta`. Usuwając asset, usuwamy też `.meta`.
- Przed commitem przegląda się diff. Przypadkowe zmiany w scenach, `ProjectSettings/` i `Packages/` się cofa.

> [!NOTE]
> **Pliki `.meta`** zawierają identyfikator assetu. Po nim Unity łączy prefaby, sceny i materiały z plikami. Bez `.meta` u kolegi wygeneruje się nowy identyfikator i wszystkie odwołania do tego assetu staną się `Missing`.

## Pull requesty

- **Tytuł PR staje się commitem na `main`**, więc powinien być konkretny, np. "#12: Blood orb and weapon system basics".
- W opisie wpisujemy `Closes #12`. Słowo musi być po angielsku, inaczej GitHub nie zamknie issue.
- Opisujemy, jak przetestować zmianę (o ile się da i jest sens).
- Niedokończoną pracę zgłaszamy jako **Draft**, np. gdy chcemy, żeby konkretna osoba ją skomentowała. Inni widzą, że to jeszcze nie jest gotowe do review.
- Jeden PR dotyczy jednej rzeczy.
- Przed oznaczeniem PR jako gotowego sprawdzamy, że:
  - projekt otwiera się bez błędów w konsoli,
  - nie ma brakujących referencji (`Missing`),
  - zmiana działa w Play Mode.

## Review i merge

- Wymagany jest 1 approve od osoby innej niż autor.
- Reviewer sprawdza, czy zmiana robi to, co opisano, i czy trzyma się zasad ([kod](Kod.md), [struktura](Struktura.md)). Zmiany w gameplayu odpala lokalnie (zmienia sobie branch na ten z PR).
- Wszystkie uwagi z review muszą zostać rozwiązane przed merge'em.
- Po approve autor merguje przez **Squash and merge**.

> [!NOTE]
> **Squash and merge** skleja wszystkie commity z brancha w jeden commit na `main`, opisany tytułem PR. Dzięki temu historia `main` to lista skończonych zadań, a nie "typo" i "poprawki", a całe zadanie da się w razie potrzeby cofnąć jednym ruchem (revert).
