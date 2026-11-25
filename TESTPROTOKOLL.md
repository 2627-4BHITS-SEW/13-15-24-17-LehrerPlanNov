# Testprotokoll – LehrerPlan

**Testdatum:** 23.11.2026 · **Getesteter Commit:** `
f0d470c
`

<!-- Commit ermitteln mit: git rev-parse --short HEAD
     Wenn ihr zwischen den Tests den Code ändert, tragt bei den betroffenen Zeilen den abweichenden Commit in die Spalte "Durchführung" ein (z. B. "Commit: def5678").
     Ein späterer Commit, der nur das Protokoll hinzufügt, ändert den getesteten Stand nicht. -->

**Unity-Version:** 6000.3.24f1

| Nr. | Durchführung | Soll | Ist | Status |
|-----|--------------|------|-----|--------|
| 1 | Play Mode gestartet, Raum im C Trakt angeklickt und Sprechstunde mit der Detailseite von HTL-Salzburg verglichen . | Tag und Uhrzeit stimmen überein. | Sichtbarer Text mit korrekten Sprechstundenzeiten | Bestanden |
| 2 | Räume im C Trakt und B Trakt mit unterschiedlichen Lehrkräften geprüft und Anzeige mit der Schulwebsite verglichen. | Raumnummer und Personen stimmen für beide Räume. | Raumnummer und Personen stimmen perfekt überein | Bestanden |
| 3 | Frische Sitzung, Lehrkraft Josef Schiendorfer in Raum C204 gewählt und in der Unity Console den Request geprüft . | Für diese URL erscheint HTTP 200 (Success). | HTTP 200 | Bestanden
| 4 |  Frische Sitzung, Play Mode gestartet und in der Unity Console den Abruf der Lehrkräfteübersicht `https://www.htl-salzburg.ac.at/lehrerinnen.html` beim Start geprüft. | Für diese URL erscheint HTTP 200 (Success). | HTTP 200 erscheint | Bestanden |
| 5 | Raum C204 angeklickt und Sprechstunde sowie E-Mail am Schild mit der Detailseite verglichen. | Die Angaben stimmen überein und stehen am vorgesehenen Platz. | Email und Sprechstunden stimmen.| Bestanden|
| 6 | Request-Start direkt vor dem Aufruf geloggt, auf `Fertig: …` gewartet, Console geleert und Raum C204 erneut angeklickt. | Das Schild öffnet sich ohne neuen Request. | Schild öffnet ja aber nicht ohen neuen Request | Nicht ganz Bestanden |
| 7 | Szene ohne Internet gestartet, auf den Abruffehler gewartet und Raum C204 angeklickt. | Die Console meldet den Fehler. Das Schild bleibt geschlossen. Beim Klick kann zusätzlich ein Ladehinweis erscheinen. | Fenster bleibt geschlossen und Fehler wird gemeldet | Bestanden |

## Hinweise zum Status

- **Bestanden:** Ist und Soll stimmen überein.
- **Nicht ganz Bestanden:** Stimmt teils, jedoch nicht alles korrekt.
- **Fehlgeschlagen:** Test durchgeführt, Ergebnis weicht ab.
- **Nicht ausführbar:** Eine Voraussetzung fehlt (z. B. Kompilierungsfehler, Unity startet nicht). Grund bitte in der Spalte „Ist" angeben.
