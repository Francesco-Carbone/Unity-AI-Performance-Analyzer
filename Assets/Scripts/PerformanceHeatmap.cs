using UnityEngine;
using System.IO;
using System.Text;

public class PerformanceHeatmap : MonoBehaviour
{
    private string logPath;
    private Transform cameraTransform;

    void Awake()
    {
        // Trova il Main Camera
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Crea il percorso del file
        string rootPath = Directory.GetParent(Application.dataPath).FullName;
        logPath = Path.Combine(rootPath, "PerformanceHeatmap.csv");

        // Se il file non esiste, crea l'intestazione
        if (!File.Exists(logPath))
        {
            File.WriteAllText(logPath, "Timestamp,PosX,PosY,PosZ,IssueType,Severity\n");
        }
    }

    // Funzione chiamata dall'AIAssistant
    public void RecordIssue(string issueType, float severity)
    {
        // Se non trova la camera all'inizio, riprova qui nel caso la camera venga spawnata dopo
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform == null) return;

        Vector3 pos = cameraTransform.position;
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Formatta la riga
        string line = string.Format("{0},{1:F2},{2:F2},{3:F2},{4},{5:F2}\n",
            timestamp, pos.x, pos.y, pos.z, issueType, severity);

        File.AppendAllText(logPath, line);
    }
}