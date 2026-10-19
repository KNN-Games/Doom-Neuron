# Organizacja pracy

## Język

- **Angielski:** kod, komentarze, nazwy plików i assetów, commity, PR.
- **Polski:** issues, dokumentacja.

## Gdzie co trzymamy

- **Notion:** notatki, roadmapa, dokumentacja nietechniczna (lore itp.). W Notion niczego nie przypisujemy i nie śledzimy statusu.
- **GitHub:** bieżąca praca, czyli issues, PR. To jedyne miejsce, w którym widać, co jest do zrobienia i kto co robi.
- **Discord:** dyskusje o pracy.

Gdy coś z roadmapy w Notion idzie do realizacji, zakładamy issue, a w Notion należałoby zostawić do niego link.

## Issues

- Każda praca ma issue. Zanim zaczniesz, przypisz je do siebie.
- Tytuł opisuje zadanie, np. "Dodaj VFX do kuli krwi" albo "Gracz przenika przez drzwi w Arenie".
- **Type**:
  - `Feature`: *co* ma się pojawić w grze, np. "System broni", "Boss: Zaru". Zwykle potrzebuje kilku PR.
  - `Task`: konkretna praca na jeden PR, np. "Animacja ataku X". Także praca, która nie jest nową funkcją: refactor, porządki, konfiguracja.
  - `Bug`: coś działa źle.
- **Label**, czyli obszar (jeden lub więcej): `code`, `ui`, `art`, `audio`, `level-design`, `lore` (teksty, dialogi etc), `documentation`.
- Duże zadanie dzielimy na **sub-issues**. Feature dostaje Taski jako podzadania, a GitHub pokazuje postęp, np. 3/5. Dzielimy, jeśli zadanie zajmie więcej niż kilka dni, jest skomplikowane lub pracować nad nim będzie kilka osób.
- Każdy błąd znaleziony w grze zgłaszamy jako issue typu `Bug`, także drobny i taki, którego nie zamierzasz naprawiać sam. Zgłoszenie tylko na Discordzie łatwo przeoczyć i nikt go nie przypisze. Zanim założysz nowe, sprawdź, czy takiego issue jeszcze nie ma.
- Zgłoszenie błędu zawiera kroki do odtworzenia, oczekiwany i faktyczny efekt oraz, jeśli się da, zrzut ekranu/filmik.
- Dyskusje, pytania i prośby o pomoc prowadzimy na Discordzie. Ustalenia z dyskusji dopisujemy do issue.
- Issue zamykamy z właściwym powodem: **Close as completed** (zrobione), **Close as not planned** (nie robimy) albo **Close as duplicate** (już jest inne issue).

> [!NOTE]
> **Type** mówi, czym jest zadanie, a **label** czego dotyczy. Dzięki temu da się łatwo filtrować, np. wszystkie bugi w dźwięku to type `Bug` + label `audio`. Powód zamknięcia wybiera się ze strzałki przy przycisku "Close issue".
