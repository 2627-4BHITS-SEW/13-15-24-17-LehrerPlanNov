using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorSignManager : MonoBehaviour
{
    [Header("Daten")]
    public TeacherScraper scraper;

    [Header("Großes Türschild")]
    public GameObject doorSign;      // das Objekt "tuerschild"
    public Transform closeButton;    // "close" (braucht einen Collider2D)
    public TMP_Text raumText;        // "Raum"
    public TMP_Text[] nameTexts;           // Name1..Name4
    public TMP_Text[] sprechstundeTexts;   // Sprechstunde1..4
    public TMP_Text[] emailTexts;          // Email1..4

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>();
        if (doorSign != null) doorSign.SetActive(false);
    }

    void Update()
    {
        if (!ClickedThisFrame(out Vector2 screenPos)) return;
        if (cam == null || scraper == null || doorSign == null) return;

        Vector2 world = cam.ScreenToWorldPoint(screenPos);
        Collider2D[] hits = Physics2D.OverlapPointAll(world);

        // Solange das Schild offen ist, zählt nur der Close-Button
        if (doorSign.activeSelf)
        {
            foreach (var h in hits)
            {
                if (closeButton != null && (h.transform == closeButton || h.transform.IsChildOf(closeButton)))
                {
                    doorSign.SetActive(false);
                    return;
                }
            }
            return;
        }

        if (hits.Length == 0) return;

        if (!scraper.IsLoaded)
        {
            Debug.LogWarning("Lehrerdaten werden noch geladen, bitte kurz warten.");
            return;
        }

        // Raum suchen: Objektname oder ein Parent heißt z.B. "C205"
        foreach (var h in hits)
        {
            for (Transform t = h.transform; t != null; t = t.parent)
            {
                if (scraper.byRoom.TryGetValue(t.name, out var teachers))
                {
                    ShowSign(t.name, teachers);
                    return;
                }
            }
        }

        Debug.Log("Keine Lehrer für dieses Objekt gefunden: " + hits[0].name);
    }

    void ShowSign(string room, List<Teacher> teachers)
    {
        if (raumText != null) raumText.text = "Raum: " + room;

        for (int i = 0; i < nameTexts.Length; i++)
        {
            bool has = i < teachers.Count;
            if (nameTexts[i] != null)
                nameTexts[i].text = has ? "Name: " + teachers[i].name : "";
            if (i < sprechstundeTexts.Length && sprechstundeTexts[i] != null)
                sprechstundeTexts[i].text = has ? "Sprechstunde: " + teachers[i].sprechstunde : "";
            if (i < emailTexts.Length && emailTexts[i] != null)
                emailTexts[i].text = has ? "Email: " + teachers[i].email : "";
        }

        doorSign.SetActive(true);
    }

    bool ClickedThisFrame(out Vector2 pos)
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            pos = mouse.position.ReadValue();
            return true;
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            pos = Input.mousePosition;
            return true;
        }
#endif
        pos = default;
        return false;
    }
}
