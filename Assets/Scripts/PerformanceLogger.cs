using UnityEngine;
using System.IO;

public class PerformanceLogger : MonoBehaviour
{
    private string filePath;
    private float timer = 0f;
    public float logInterval = 0.5f;

    // Etichetta cambiata da StressTester
    [HideInInspector] public string scenarioLabel = "CALIBRATION";

    private float lastLogFrameTime = 0f;
    private float actualCpuMeasured = 0f;

    void Awake()
    {
        lastLogFrameTime = Time.realtimeSinceStartup;

        // Pathfinder
        string rootPath = Directory.GetParent(Application.dataPath).FullName;
        filePath = Path.Combine(rootPath, "PerformanceData.csv");

        UnityEngine.Debug.Log("<color=cyan>Percorso finale rilevato: </color>" + filePath);

        // Crea il file di log
        try
        {
            // Scrive l'intestazione
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "Label,Time_sec,FrameTime_ms,FPS,Batches_DrawCalls,MainThreadTime_ms\n");
            }
            UnityEngine.Debug.Log("<color=green>File CSV inizializzato con successo!</color>");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("ERRORE CRITICO: Impossibile creare il file. " + e.Message);
        }
    }

    void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        actualCpuMeasured = (startTime - lastLogFrameTime) * 1000f;
        lastLogFrameTime = startTime;

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

        float cpuMainTime = actualCpuMeasured;

#if UNITY_EDITOR
        batches = UnityEditor.UnityStats.batches;
#endif

        string riga = string.Format(System.Globalization.CultureInfo.InvariantCulture,"{0},{1:F2},{2:F2},{3:F0},{4},{5:F2}\n", scenarioLabel, Time.timeSinceLevelLoad, frameTime, fps, batches, cpuMainTime);
        File.AppendAllText(filePath, riga);
    }
}