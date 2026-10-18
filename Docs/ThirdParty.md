# Zewnętrzne assety i licencje

## Zasady

**Repo jest publiczne.** Wszystko, co do niego trafia, może pobrać każdy.

- Zanim dodasz cudzy asset (model, teksturę, dźwięk, muzykę, font, kod), sprawdź jego licencję. Musi pozwalać na użycie w grze (dobrze też ew. sprawdzić użycie komercyjne) i na trzymanie w publicznym repo.
- **Assetów z Unity Asset Store itp. nie commitujemy.** Ich licencja (EULA) zabrania udostępniania plików źródłowych, a publiczne repo jest takim udostępnieniem. Uzgadniamy w zespole, jak je dystrybuować.
- Każdy zewnętrzny asset wpisujemy do [listy poniżej](#lista): nazwa, autor, link do źródła i licencja. Wpis dodajemy w tym samym PR, w którym pojawia się asset.
- Zewnętrzne assety trafiają do `Assets/ThirdParty/<Nazwa>/` i nie są edytowane w miejscu. Jeśli trzeba coś zmienić, kopiujemy plik do naszych folderów i zmieniamy kopię.
- Licencje wymagające podania autora (np. CC BY) oznaczamy na liście, bo trzeba je będzie umieścić w napisach końcowych.

> [!NOTE]
> Najczęstsze licencje: **CC0**, czyli "rób co chcesz", bez podawania autora. **MIT**, typowa dla kodu, pozwala na wszystko, ale trzeba dołączyć jej tekst z nazwą autora. **CC BY** pozwala na wszystko, ale trzeba podać autora. **CC BY-NC** zabrania użycia komercyjnego. **SIL OFL**, typowa dla fontów, pozwala używać fontu w grze. Jeśli licencja jest niejasna albo jej nie ma, **nie używamy assetu**. Brak licencji oznacza, że wszystkie prawa należą do autora.

## Lista

| Asset | Autor | Źródło | Licencja |
|---|---|---|---|
| TextMesh Pro (Essential Resources) | Unity | paczka `com.unity.ugui` | Unity Companion License |
| Liberation Sans | Red Hat | dołączony do TMP | SIL OFL 1.1 |
