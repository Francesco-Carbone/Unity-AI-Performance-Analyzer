using UnityEngine;
using System.IO;
using System.Text;
using System.Globalization;

public class PerformanceHeatmap : MonoBehaviour
{
    private string logPath;
    private string screenshotFolder;
    private Transform targetTransform;
    private bool playerTrack = false;

    [Header("Configurazione Screenshot")]
    [Tooltip("Minimi secondi tra uno screenshot e l'altro per evitare spam")]
    public float minScreenshotInterval = 10.0f;
    private float lastScreenshotTime = -999f;

    void Awake()
    {
        AggiornaTarget();

        // Crea il percorso del file
        string rootPath = Directory.GetParent(Application.dataPath).FullName;
        logPath = Path.Combine(rootPath, "PerformanceHeatmap.csv");
        screenshotFolder = Path.Combine(rootPath, "PerformanceScreenshots");

        if (!Directory.Exists(screenshotFolder))
        {
            Directory.CreateDirectory(screenshotFolder);
        }

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

        if (Time.time - lastScreenshotTime >= minScreenshotInterval) 
        {
            string timeStampFile = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Issue_{issueType}_{timeStampFile}.png";
            string fullPath = Path.Combine(screenshotFolder, fileName);

            ScreenCapture.CaptureScreenshot(fullPath);
            lastScreenshotTime = Time.time;
        }
    }
}