using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Teacher
{
    public string name;
    public string room;
    public string sprechstunde;
    public string email;
    public string url;
}

public class TeacherScraper : MonoBehaviour
{
    [Header("Quelle")]
    public string baseUrl = "https://www.htl-salzburg.ac.at/";
    public string listUrl = "https://www.htl-salzburg.ac.at/lehrerinnen.html";

    [Header("Regex")]
    public string linkRegex = "href=\"([^\"]*lehrerinnen-details/[^\"]+\\.html)\"";
    public string nameRegex = @"Name:\s*(.+?)\s*Raum:";
    public string roomRegex = @"Raum:\s*([BC])\s*[-.]?\s*(2)\s*[-.]?\s*(\d{2})\b";
    public string sprechstundeRegex = @"Sprechstunde:\s*(.+?)\s*E-Mail:";
    public string emailRegex = @"E-Mail:\s*([^\s]+@[^\s]+)";

    [Header("Geschwindigkeit")]
    [Tooltip("Nur Seiten laden, deren URL auf B/C 2xx endet (z.B. ...-c-205.html). Fällt automatisch auf 'alle' zurück, wenn nichts passt.")]
    public bool prefilterByUrl = true;
    public string urlRoomRegex = @"-[bc]-?2\d\d[^/]*\.html$";
    [Tooltip("Wie viele Seiten gleichzeitig geladen werden")]
    public int parallelRequests = 12;

    public List<Teacher> result = new List<Teacher>();

    // Raum (z.B. "C205") -> alle Lehrer in diesem Raum
    public Dictionary<string, List<Teacher>> byRoom = new Dictionary<string, List<Teacher>>();
    public bool IsLoaded { get; private set; }

    Regex roomRx, nameRx, sprechRx, emailRx;

    void Start()
    {
        StartCoroutine(LoadTeachers());
    }

    IEnumerator LoadTeachers()
    {
        string listHtml = null;
        yield return Get(listUrl, html => listHtml = html);
        if (listHtml == null) yield break;

        var links = new List<string>();
        foreach (Match m in Regex.Matches(listHtml, linkRegex, RegexOptions.IgnoreCase))
        {
            string href = WebUtility.HtmlDecode(m.Groups[1].Value);
            string full = href.StartsWith("http") ? href : new Uri(new Uri(baseUrl), href).ToString();
            if (!links.Contains(full)) links.Add(full);
        }
        Debug.Log($"{links.Count} Lehrer-Links gefunden.");

        if (prefilterByUrl)
        {
            var filtered = links.FindAll(l => Regex.IsMatch(l, urlRoomRegex, RegexOptions.IgnoreCase));
            if (filtered.Count > 0)
            {
                links = filtered;
                Debug.Log($"{links.Count} Links nach URL-Filter (B/C, 2. Stock).");
            }
            else
            {
                Debug.LogWarning("URL-Filter hat nichts gefunden, lade alle Seiten.");
            }
        }

        roomRx = new Regex(roomRegex, RegexOptions.IgnoreCase);
        nameRx = new Regex(nameRegex, RegexOptions.IgnoreCase);
        sprechRx = new Regex(sprechstundeRegex, RegexOptions.IgnoreCase);
        emailRx = new Regex(emailRegex, RegexOptions.IgnoreCase);

        // Mehrere Worker laden gleichzeitig aus derselben Warteschlange
        var queue = new Queue<string>(links);
        int running = 0;
        int workers = Mathf.Max(1, parallelRequests);
        for (int i = 0; i < workers; i++)
        {
            running++;
            StartCoroutine(Worker(queue, () => running--));
        }
        while (running > 0) yield return null;

        IsLoaded = true;
        Debug.Log($"Fertig: {result.Count} Lehrer in {byRoom.Count} Räumen.");
    }

    IEnumerator Worker(Queue<string> queue, Action onFinished)
    {
        while (queue.Count > 0)
        {
            string link = queue.Dequeue();
            string html = null;
            yield return Get(link, h => html = h);
            if (html != null) Process(link, html);
        }
        onFinished();
    }

    void Process(string link, string html)
    {
        string text = StripTags(html);

        Match roomMatch = roomRx.Match(text);
        if (!roomMatch.Success) return; // nicht B/C, 2. Stock

        Match nameMatch = nameRx.Match(text);
        Match sprechMatch = sprechRx.Match(text);
        Match emailMatch = emailRx.Match(text);

        var t = new Teacher
        {
            name = nameMatch.Success ? nameMatch.Groups[1].Value.Trim() : "?",
            room = $"{roomMatch.Groups[1].Value.ToUpper()}{roomMatch.Groups[2].Value}{roomMatch.Groups[3].Value}",
            sprechstunde = sprechMatch.Success ? sprechMatch.Groups[1].Value.Trim() : "",
            email = emailMatch.Success ? emailMatch.Groups[1].Value.Trim() : "",
            url = link
        };
        result.Add(t);

        if (!byRoom.TryGetValue(t.room, out var list))
            byRoom[t.room] = list = new List<Teacher>();
        list.Add(t);

        Debug.Log($"{t.name} – {t.room}");
    }

    static string StripTags(string html)
    {
        string noScripts = Regex.Replace(html, "<(script|style)[^>]*>.*?</\\1>", " ",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);
        string text = Regex.Replace(noScripts, "<[^>]+>", " ");
        text = WebUtility.HtmlDecode(text);
        return Regex.Replace(text, @"\s+", " ");
    }

    IEnumerator Get(string url, Action<string> onDone)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Fehler bei {url}: {req.error}");
                onDone(null);
            }
            else onDone(req.downloadHandler.text);
        }
    }
}
