# Kod C#

## Nazewnictwo

| Element | Styl | Przykład |
|---|---|---|
| Klasy, struktury, enumy, metody, właściwości, eventy, stałe | PascalCase | `PlayerHealth`, `TakeDamage()`, `MaxBlood` |
| Pola (także `[SerializeField]`), parametry, zmienne lokalne | camelCase, bez `_` | `bloodBar`, `damage` |
| Interfejsy | `I` + PascalCase | `IDamageable` |
| Wartości logiczne | `is` / `has` / `can` | `isInvulnerable`, `IsDead`, `canShoot` |

## Struktura

- Jedna klasa to jeden plik, a nazwa pliku jest taka sama jak nazwa klasy. Długą klasę można podzielić na pliki `partial` (np. `OptionsMenu.Controls.cs`).
- Pola edytowane w Inspectorze deklarujemy jako `[SerializeField] private`, a nie `public` (przy ewentualnych zmianach takich pól pamiętaj o możliwości `[FormerlySerializedAs("...")]`). Dostęp z zewnątrz daje właściwość (`public int CurrentBlood { get; private set; }`).
- Formatowanie: 4 spacje, klamry w nowej linii. Jednolinijkowe `if (...) return;` jest dozwolone.
- Metody cyklu życia Unity piszemy na początku klasy, w kolejności wywoływania: `Awake`, `OnEnable`, `Start`, `FixedUpdate`, `Update`, `LateUpdate`, `OnDisable`, `OnDestroy`.
- Wartości do strojenia, np. obrażenia broni czy życie przeciwnika, trzymamy w ScriptableObjectach, a nie w kodzie ani porozrzucane po prefabach.

> [!NOTE]
> `[SerializeField] private` pokazuje pole w Inspectorze, ale nie pozwala innym skryptom go zmieniać. Pole `public` może zmienić dowolny skrypt, a wtedy trudno ustalić, kto zepsuł wartość.

## Unity

- Obiekty w scenie i prefabie łączymy domyślnie referencjami przez Inspector (`[SerializeField]`).
- `Singleton<T>` stosujemy tylko dla systemów, które w całej grze są jedne, np. game, save, input, muzyka.
- Do singletonów żyjących między scenami (`DontDestroyOnLoad`, np. `GameManager`) odwołujemy się zawsze przez `.Instance`, nigdy przez referencję w scenie.
- `GetComponent`, `Find...` i podobne wywołania robimy w `Awake` albo `Start` i zapamiętujemy wynik. Nigdy w `Update`.
- W `Update` i innych metodach wywoływanych co klatkę nie alokujemy pamięci: bez `new` list i tablic, bez sklejania stringów i bez LINQ.
- Do tagów używamy `CompareTag("...")`, a nie `tag == "..."`.

> [!NOTE]
> **Singleton między scenami.** Gdy scena ma własną kopię np. `GameManager`, a ten już przyszedł z poprzedniej sceny, `Singleton<T>` niszczy kopię jako duplikat. Referencje przeciągnięte w scenie do tej kopii wskazują wtedy na zniszczony obiekt. `.Instance` zawsze wskazuje na ten, który przetrwał.
>
> **Alokacje.** Każda alokacja w `Update` tworzy śmieci, które garbage collector musi co jakiś czas posprzątać. Sprzątanie zatrzymuje grę na ułamek sekundy i widać to jako zacięcia. W szybkiej strzelance są one bardzo odczuwalne.

## Komentarze

- Publiczne klasy dostają `/// <summary>` z opisem, za co klasa odpowiada. Dobrze też opisywać publiczne pola i metody, szczególnie, jeśli ich działanie czy argumenty nie są oczywiste.
- Zamiast zostawiać `// TODO`, zakładamy issue. Jeśli TODO musi zostać w kodzie, zapisujemy je jako `// TODO(#12): ...`.
