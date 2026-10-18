# Praca z Unity

## Wersja Unity

Wszyscy pracują na wersji z `ProjectSettings/ProjectVersion.txt`. Zmieniamy ją tylko po uzgodnieniu z zespołem, w osobnym PR, i wszyscy przechodzą na nią naraz. Jeśli Unity zapyta o aktualizację projektu, wybierz "nie".

## Sceny i prefaby

- **Jedną scenę edytuje jedna osoba naraz.** Zanim zaczniesz edytować wspólną scenę, daj znać zespołowi - przynajmniej tak to powinno wyglądać w idealnym świecie
- Obiekty budujemy jako prefaby, a w scenie tylko je układamy, przynajmniej na ile to możliwe.
- Konfliktu w scenie lub prefabie nie łączymy ręcznie. Wybieramy jedną wersję (`git checkout --ours` albo `--theirs`) i drugą zmianę nanosimy ponownie w edytorze.

> [!NOTE]
> Dwie osoby zmieniające ten sam obiekt w tej samej scenie zawsze wywołają konflikt, a sceny nie da się sensownie skleić w edytorze tekstu. Prefab to osobny plik, więc praca na prefabach rzadko koliduje z pracą innych.

## Pliki projektu

- Zmiany w `ProjectSettings/` i `Packages/manifest.json` robimy tylko świadomie i zawsze opisujemy w PR.
- Paczek nie dodajemy ani nie aktualizujemy przy okazji innej pracy. Robimy to w osobnym PR.
