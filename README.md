# HTL Salzburg Lehrer-Navigator

## Kurzbeschreibung

Der **HTL Salzburg Lehrer-Navigator** ist eine interaktive Unity-2D-Anwendung. Sie zeigt die **Schultrakte B und C im 2. Stock** als nachgezeichneten Grundriss. Ein Klick auf einen Raum öffnet ein Türschild mit den Daten der Lehrkräfte.

## Funktionen

- Trakte B und C im 2. Stock nachgezeichnet, jeder Raum ist anklickbar
- Türschild mit Raumnummer sowie Name, Telefonnummer, E-Mail und Sprechstunde der Lehrkräfte
- Close-Button zum Schließen des Türschilds
- Lehrerdaten werden automatisch von der Schulwebseite geladen

## Funktionsweise

Die Daten stammen direkt von der HTML-Webseite der HTL Salzburg und werden über **Regex-Abfragen** ausgelesen. Die Ausdrücke stehen als einstellbare Felder im Script `TeacherScraper` und lassen sich im Unity-Inspector anpassen, falls sich die Webseite ändert.

Der **TeacherScraper** lädt beim Start die Lehrerliste (`lehrerinnen.html`) und sucht per Regex alle Links zu den Detailseiten der LehrkräfteEs werden vorab nur Seiten geladen, deren URL auf einen Raum im B- oder C-Trakt im 2. Stock endet.Von jeder Seite werden zuerst die HTML-Tags entfernt, dann liest je eine Regex Name, Raum, Sprechstunde und E-Mail aus. Nur Lehrkräfte mit Raum B/C im 2. Stock (2xx) werden übernommen. Sie werden in einem Dictionary nach Raumnummer abgelegt, z. B. `C205`. Sobald alles geladen ist, meldet `IsLoaded` den Abschluss.

Der **DoorSignManager** reagiert auf Mausklicks. Er prüft den Collider, von dem Raumschild, auf den man klickt und sucht bei diesem Objekt den Namen, der der Raumnummer entspricht. Findet er dazu Einträge im Dictionary des Scrapers, füllt er die Textfelder des Türschilds und blendet es ein. Solange die Daten noch laden, erscheint eine Warnung. Ist das Türschild offen, kann man über den Close-Button es wieder schließen. 

## Technik

| Bereich | Umsetzung |
|---|---|
| Engine | Unity 2D |
| Sprache | C# |
| Datenquelle | HTML-Seite der HTL Salzburg |
| Extraktion | Regex|
| UI | Klickbare Räume, Türschild mit TextMeshPro, 2D Skizze nachgebaut |

---
