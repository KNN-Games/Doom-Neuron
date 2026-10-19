# Struktura projektu

## Układ katalogów

```
Assets/
  Scripts/        kod gry, podzielony według obszaru: Player/, Enemy/, Map/, Managers/, UI/...
  Editor/         kod tylko dla edytora (nie trafia do buildu); ma własny asmdef
  Prefabs/        według obszaru: Enemies/, MapObjects/, UI/...
  Scenes/         Levels/ = poziomy, w głównym folderze menu i sceny techniczne
  Sprites/        grafika 2D i animacje sprite'ów
  ScriptableObjects/  dane gry: statystyki broni i przeciwników, fale, dialogi
  Materials/      materiały
  Shaders/        shadery i Shader Graph
  Sounds/         Music/, SFX/
  Input/          Input Actions
  Localization/   tabele tekstów
  Settings/       ustawienia URP i inne assety konfiguracyjne
  ThirdParty/     zewnętrzne assety, których nie edytujemy (zob. ThirdParty.md)
  _Sandbox/<NazwiskoImie>/  prywatne sceny testowe; nic spoza Sandboxa nie może się do nich odwoływać
  TextMesh Pro/, AddressableAssetsData/, UI Toolkit/   foldery tworzone przez Unity i paczki: commitujemy, nie przenosimy
Docs/             dokumentacja techniczna
```

- Foldery i pliki nazywamy w PascalCase, **bez spacji i polskich znaków**. Istniejące `UI overrides` i `Map objects` zmienimy w osobnym PR.
- Pliki przenosimy i zmieniamy im nazwy **tylko w edytorze Unity**, nie w Eksploratorze, bo inaczej `.meta` traci powiązanie i referencje się psują.
- Nie zakładamy nowego folderu najwyższego poziomu bez uzgodnienia z zespołem.

> [!NOTE]
> Przy przenoszeniu pliku w edytorze Unity jego `.meta` wędruje razem z nim. Przeniesienie w Eksploratorze zostawia `.meta` w starym miejscu. Unity uznaje wtedy plik za nowy asset i wszystko, co z niego korzystało, pokazuje `Missing`.

## Nazewnictwo assetów

Nazwa ma postać `Kategoria_Nazwa_Wariant` w PascalCase, z numeracją dwucyfrową.

| Typ | Przykład |
|---|---|
| Prefab | `Enemy_Rotter`, `Pickup_BloodVial` |
| Sprite / tekstura | `Enemy_Rotter_Walk_01` |
| Materiał | `Wall_Stone_01` |
| Dźwięk | `SFX_Sword_Swing_01`, `Music_Arena` |
| Scena | `Level_01_Gate`, `MainMenu` |
