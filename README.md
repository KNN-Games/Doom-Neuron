# Doom-Neuron

Boomer shooter w klimacie dark fantasy, robiony w Unity (URP).

- [Zasady współpracy](CONTRIBUTING.md): spis wszystkich zasad (praca, git, Unity, kod, licencje)

> [!NOTE]
> Ramki takie jak ta zawierają wyjaśnienia dla osób, które mniej znają gita albo Unity. Jeśli wiesz, o co chodzi, możesz je pominąć.

## Jak postawić projekt

### 1. Zainstaluj Git i włącz Git LFS

Pobierz [Git](https://git-scm.com/), a potem włącz LFS (raz na komputer):
```bash
git lfs install
```
> [!NOTE]
> Git LFS przechowuje duże pliki, czyli grafikę, dźwięk i fonty, poza zwykłą historią gita. Bez niego zamiast obrazków i dźwięków pobrałyby się kilkulinijkowe pliki-wskaźniki.

### 2. Sklonuj repo

```bash
git clone https://github.com/KNN-Games/Doom-Neuron.git
```

### 3. Zainstaluj Unity

Potrzebujesz dokładnie wersji z [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt), obecnie **6000.5.2f1**. Najłatwiej zainstalować ją przez Unity Hub > Installs > Install Editor > Archive.
> [!NOTE]
> Otwarcie projektu w innej wersji Unity po cichu przepisuje część plików. Gdy to trafi do repo, wszyscy pozostali muszą się zaktualizować. Wersję zmieniamy tylko wspólnie, zob. [Praca z Unity](Docs/Unity.md#wersja-unity).

### 4. Podepnij Smart Merge

Uruchom poniższe polecenia w folderze repo (raz). `<UNITY>` zamień na folder edytora, np. `C:/Program Files/Unity/Hub/Editor/6000.5.2f1`. Dokładną ścieżkę znajdziesz w Unity Hub > Installs > "..." przy wersji > Show in Explorer.
```bash
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver "'<UNITY>/Editor/Data/Tools/UnityYAMLMerge.exe' merge -h -p --force %O %B %A %A"
git config merge.unityyamlmerge.recursive binary
```
> [!NOTE]
> Sceny i prefaby to pliki tekstowe opisujące obiekty. Zwykły merge gita łączy je linia po linii i potrafi zepsuć scenę: zostają zerwane referencje albo obiekty bez rodzica. UnityYAMLMerge łączy je obiekt po obiekcie. Bez tej konfiguracji wszystko działa, tylko bez tej ochrony.

### 5. Otwórz projekt

W Unity Hub wybierz Add i wskaż folder repo. Pierwsze otwarcie trwa długo, bo Unity buduje cache w folderze `Library/`.

## Jak git traktuje pliki ([`.gitattributes`](.gitattributes))

| Pliki | Reguła |
|---|---|
| Tekstowe (`.cs`, `.json`, `.md`...) | końce linii przechowywane jako LF (Unix style) |
| YAML Unity (`.unity`, `.prefab`, `.asset`, `.mat`, `.meta`...) | zawsze LF, merge przez UnityYAMLMerge |
| Binarne (`.png`, `.psd`, `.mp3`, `.wav`, `.fbx`, `.ttf`...) | Git LFS, bez mergowania |

> [!NOTE]
> **Końce linii.** Windows kończy linie dwoma znakami (CRLF), a macOS i Linux jednym (LF). Gdyby nikt tego nie pilnował, plik otwarty na innym systemie wyglądałby w gicie na zmieniony w całości. Nowoczesne IDE (Visual Studio, VSCode) same ogarniają i pilnują takich rzeczy - niezależnie jaki masz system pliki powinny wyglądać jak u innych.
>
> **Dlaczego LF?** To de facto standard: używają go Linux, macOS, większość narzędzi programistycznych i sam git, który domyślnie trzyma teksty w repo z LF. Unity też zapisuje sceny i prefaby z LF na każdym systemie.
>
> **Binarne bez mergowania.** Dwóch wersji obrazka czy dźwięku nie da się połączyć. Przy konflikcie wybiera się jedną z nich (zapewne nowszą) w całości.

Gdy w projekcie pojawia się nowy format pliku, dopisz go do `.gitattributes` w tym samym PR, w którym pojawia się pierwszy taki plik. Rozszerzenie binarne zapisuj w obu wielkościach liter, np. `*.[wW][aA][vV]`, bo na Linuksie i macOS `*.wav` nie obejmie pliku `.WAV`.
