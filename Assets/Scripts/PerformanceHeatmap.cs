using UnityEngine;
using System.IO;
using System.Text;
using System.Globalization;

public class PerformanceHeatmap : MonoBehaviour
{
    [Header("Opzioni Screenshot")]
    [Tooltip("Attiva per scattare una foto del gioco durante i lag gravi")]
    public bool abilitaScreenshot = true;

    [Tooltip("Scatta se il Ratio di lag supera questo valore (es. 1.8 significa lag molto visibile)")]
    public float sogliaScreenshot = 1.8f;

    private string logPath;
    private string cartellaScreenshots;
    private Transform targetTransform;
    private bool playerTrack = false;

    void Awake()
    {
        AggiornaTarget();

        // Crea il percorso del file CSV e della cartella per le foto
        string rootPath = Directory.GetParent(Application.dataPath).FullName;
        logPath = Path.Combine(rootPath, "PerformanceHeatmap.csv");
        cartellaScreenshots = Path.Combine(rootPath, "DebugScreenshots");

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

        System.DateTime now = System.DateTime.Now;
        string timestampCSV = now.ToString("yyyy-MM-dd HH:mm:ss");
        string timestampFile = now.ToString("yyyy-MM-dd_HH-mm-ss");

        // Formatta la riga
        string line = string.Format(CultureInfo.InvariantCulture, "{0},{1:F2},{2:F2},{3:F2},{4},{5:F2}\n",
            timestampCSV, pos.x, pos.y, pos.z, issueType, severity);

        File.AppendAllText(logPath, line);

        if (abilitaScreenshot && severity >= sogliaScreenshot)
        {
            // Crea la cartella non è presente
            if (!Directory.Exists(cartellaScreenshots))
            {
                Directory.CreateDirectory(cartellaScreenshots);
            }

            // Crea il nome del file: es. 2026-05-28_16-30-00_GPU_STRESS_Lag2.1.png
            string nomeFile = Path.Combine(cartellaScreenshots, $"{timestampFile}_{issueType}_Lag{severity:F1}.png");

            ScreenCapture.CaptureScreenshot(nomeFile);

            Debug.Log($"<color=cyan>[Heatmap]</color> Lag critico rilevato ({severity:F2}x). Screenshot salvato in DebugScreenshots!");
        }
    }
}