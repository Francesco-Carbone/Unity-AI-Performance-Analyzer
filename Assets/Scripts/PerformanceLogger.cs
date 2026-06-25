using UnityEngine;
using System.IO;
using UnityEngine.Profiling;
using Unity.Profiling;

public class PerformanceLogger : MonoBehaviour
{
    private string filePath;
    private float timer = 0f;
    public float logInterval = 0.5f;

    // Etichetta cambiata da StressTester
    [HideInInspector] public string scenarioLabel = "CALIBRATION";

    private Recorder cpuRecorder;
    private ProfilerRecorder gpuRecorder;
    private ProfilerRecorder physicsRecorder;

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

        gpuRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "GPU Frame Time");
        physicsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Physics, "Physics Simulation Time");

        // Crea il file di log
        try
        {
            // Scrive l'intestazione
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "Label,Time_sec,FrameTime_ms,FPS,GPUTime_ms,MainThreadTime_ms,PhysicsTime_ms,Memory_MB\n");
            }
            UnityEngine.Debug.Log("<color=green>File CSV inizializzato con successo!</color>");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("ERRORE CRITICO: Impossibile creare il file. " + e.Message);
        }
    }

    void OnDestroy()
    {
        if (gpuRecorder.Valid) gpuRecorder.Dispose();
        if (physicsRecorder.Valid) physicsRecorder.Dispose();
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

    float GetGPUTime()
    {
        if (gpuRecorder.Valid && gpuRecorder.LastValue > 0)
        {
            return gpuRecorder.LastValue / 1000000f;
        }
        return 1.0f; // Valore di fallback se la GPU non ha ancora risposto
    }

    float GetPhysicsTime()
    {
        if (physicsRecorder.Valid && physicsRecorder.LastValue > 0)
        {
            return physicsRecorder.LastValue / 1000000f;
        }
        return 0.1f; // Fallback minimo se non ci sono calcoli fisici attivi
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

        float gpuTimeMs = GetGPUTime();
        float physicsTimeMS = GetPhysicsTime();

        float ramMB = System.GC.GetTotalMemory(false) / 1048576f;
        float vramMB = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576f;
        float memoryMB = ramMB + vramMB;

        string riga = string.Format(System.Globalization.CultureInfo.InvariantCulture,
            "{0},{1:F2},{2:F2},{3:F0},{4:F2},{5:F2},{6:F2},{7:F2}\n", 
            scenarioLabel, Time.timeSinceLevelLoad, frameTime, fps, gpuTimeMs, cpuMainTime, physicsTimeMS, memoryMB);
        
        try
        {
            File.AppendAllText(filePath, riga);
        }
        catch (System.IO.IOException)
        {
        }
    }
}