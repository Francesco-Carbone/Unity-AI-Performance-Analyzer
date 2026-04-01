using UnityEngine;
using System.IO;
using UnityEditor; // Solo per usare UnityStats nell'editor

public class PerformanceLogger : MonoBehaviour
{
    private string filePath;
    private float timer = 0f;
    public float logInterval = 0.5f; // Salva i dati ogni mezzo secondo
    public string scenarioLabel = "Stress GPU";

    void Start()
    {
        // Crea il file CSV nella cartella del progetto

        filePath = Application.dataPath + "/PerformanceData.csv";

        // Scrive l'intestazione del file (solo se non esiste)
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Label,Time_sec,FrameTime_ms,FPS,Batches_DrawCalls\n");
        }

        Debug.Log("Inizio registrazione dati in: " + filePath);

    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer >= logInterval)
        {
            LogData();
            timer = 0f;
        }
    }

    void LogData()
    {
        float frameTime = Time.unscaledDeltaTime * 1000f;
        float fps = 1.0f / Time.unscaledDeltaTime;

        // UnityStats.batches funziona solo all intero dell'Editor di Unity.
        int batches = UnityEditor.UnityStats.batches;

        // Formatta la riga CSV
        string dataRow = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1:F2},{2:F2},{3:F0},{4}\n", scenarioLabel, Time.time, frameTime, fps, batches);

        File.AppendAllText(filePath, dataRow);

    }

}
