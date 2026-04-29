using UnityEngine;
using System.IO;
using UnityEngine.Profiling;

public class PerformanceLogger : MonoBehaviour
{
    private string filePath;
    private float timer = 0f;
    public float logInterval = 0.5f;

    // Etichetta cambiata da StressTester
    [HideInInspector] public string scenarioLabel = "CALIBRATION";

    private Recorder cpuRecorder;

    void Awake()
    {
        // Pathfinder
        string rootPath = Directory.GetParent(Application.dataPath).FullName;
        filePath = Path.Combine(rootPath, "PerformanceData.csv");

        UnityEngine.Debug.Log("<color=cyan>Percorso finale rilevato: </color>" + filePath);

        // Inizializzazione Recorder
        Sampler sampler = Sampler.Get("PlayerLoop");
        if (sampler != null)
        {
            cpuRecorder = sampler.GetRecorder();
            cpuRecorder.enabled = true;
        }

        // Crea il file di log
        try
        {
            // Scrive l'intestazione
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "Label,Time_sec,FrameTime_ms,FPS,Batches_DrawCalls,MainThreadTime_ms,Memory_MB\n");
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

        float cpuMainTime = 0f;
        if (cpuRecorder != null && cpuRecorder.isValid && cpuRecorder.elapsedNanoseconds > 0)
        {
            cpuMainTime = cpuRecorder.elapsedNanoseconds / 1000000f;
        }
        else
        {
            // Fallback se il recorder non è ancora pronto
            cpuMainTime = frameTime * 0.7f;
        }

        int batches = 0;
#if UNITY_EDITOR
        batches = UnityEditor.UnityStats.batches;
#endif

        float memoryMB = System.GC.GetTotalMemory(false) / 1048576f;

        string riga = string.Format(System.Globalization.CultureInfo.InvariantCulture,
            "{0},{1:F2},{2:F2},{3:F0},{4},{5:F2},{6:F2}\n", 
            scenarioLabel, Time.timeSinceLevelLoad, frameTime, fps, batches, cpuMainTime, memoryMB);
        
        try
        {
            File.AppendAllText(filePath, riga);
        }
        catch (System.IO.IOException)
        {
        }
    }
}