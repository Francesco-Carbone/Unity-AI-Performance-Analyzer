using UnityEngine;
using System.IO;
using System.Text;
using System.Globalization;

public class PerformanceHeatmap : MonoBehaviour
{
    private string logPath;
    private Transform targetTransform;
    private bool playerTrack = false;

    void Awake()
    {
        AggiornaTarget();

        // Crea il percorso del file
        string rootPath = Directory.GetParent(Application.dataPath).FullName;
        logPath = Path.Combine(rootPath, "PerformanceHeatmap.csv");

        // Se il file non esiste, crea l'intestazione
        if (!File.Exists(logPath))
        {
            File.WriteAllText(logPath, "Timestamp,PosX,PosY,PosZ,IssueType,Severity\n");
        }
    }

    void AggiornaTarget()
    {
        // Cerca l'oggetto Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            targetTransform = player.transform;
            playerTrack = true;
            return;
        }

        // Se non c'è l'oggetto Player, usa la Main Camera
        if (Camera.main != null)
        {
            targetTransform = Camera.main.transform;
            playerTrack = false;
        }
    }

    // Funzione chiamata dall'AIAssistant
    public void RecordIssue(string issueType, float severity)
    {
        // Se il target precedente è stato distrutto o non era ancora spawnato in Awake, riprova a cercarlo
        if (targetTransform == null || !playerTrack)
        {
            AggiornaTarget();
        }

        if (targetTransform == null) return;

        Vector3 pos = targetTransform.position;
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Formatta la riga
        string line = string.Format(CultureInfo.InvariantCulture, "{0},{1:F2},{2:F2},{3:F2},{4},{5:F2}\n",
            timestamp, pos.x, pos.y, pos.z, issueType, severity);

        File.AppendAllText(logPath, line);
    }
}