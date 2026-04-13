using UnityEngine;
using System.IO;

public class PerformanceLogger : MonoBehaviour
{
    private string filePath;
    private float timer = 0f;
    public float logInterval = 0.5f;

    // Questa è l'etichetta che lo StressTester cambierà
    [HideInInspector] public string scenarioLabel = "CALIBRATION";

    void Awake()
    {
        // Questo comando trova la cartella del progetto in modo più pulito
        string rootPath = Directory.GetParent(Application.dataPath).FullName;
        filePath = Path.Combine(rootPath, "PerformanceData.csv");

        Debug.Log("<color=cyan>Percorso finale rilevato: </color>" + filePath);

        // Creiamo il file (o lo resettiamo se vuoi dati freschi)
        try
        {
            // Scriviamo l'intestazione (sovrascrive il vecchio file per evitare confusione)
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "Label,Time_sec,FrameTime_ms,FPS,Batches_DrawCalls\n");
            }
            Debug.Log("<color=green>File CSV inizializzato con successo!</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogError("ERRORE CRITICO: Impossibile creare il file. " + e.Message);
        }
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
        float fps = (Time.unscaledDeltaTime > 0) ? 1.0f / Time.unscaledDeltaTime : 0;
        int batches = 0;

#if UNITY_EDITOR
            batches = UnityEditor.UnityStats.batches;
#endif

        // Formatta la riga CSV
        string dataRow = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1:F2},{2:F2},{3:F0},{4}\n",
            scenarioLabel, Time.time, frameTime, fps, batches);
        File.AppendAllText(filePath, dataRow);
    }
}