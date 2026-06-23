using UnityEngine;
using System.Collections;
using UnityEngine.Profiling;
using Unity.Profiling;

public class AIAssistant : MonoBehaviour
{
    public enum TipoIA { Random_Forest_Precisa, Decision_Tree_Veloce }

    [Header("Configurazione")]
    public TipoIA modelloDaUsare = TipoIA.Decision_Tree_Veloce;
    public float intervalloAnalisi = 0.5f;
    [Tooltip("Soglia di allarme: 1.1 = Molto Sensibile (+10% lag), 2.0 = Poco Sensibile (+100% lag).")]
    [Range(1.05f, 3f)]
    public float sogliaAllarme = 1.3f;

    [Header("Telemetria")]
    public PerformanceHeatmap heatmapLogger;

    [Header("Autopilota Prestazioni")]
    public bool correzioneAutomatica = true;
    public float tempoDiCooldown = 5.0f; // Secondi tra un intervento e l'altro
    public int tempoPerRipristino = 10; // Quanti cicli "Normal" servono per rialzare la qualità

    private float cooldownAttuale = 0f;
    private int contatoreNormal = 0;

    [Header("Dynamic Resolution (DRS)")] //Necessario attivare l opzione Dynamic Resolution della Main Camera
    [Tooltip("Limite minimo di risoluzione (0.5 = 50% della risoluzione originale)")]
    public float risoluzioneMinima = 0.5f;
    [Tooltip("Di quanto abbassare la risoluzione ad ogni intervento (0.15 = -15%)")]
    public float stepRisoluzione = 0.15f;
    private float risoluzioneAttuale = 1.0f;

    // Variabili per salvare lo stato originale
    private int originalFrameRate;
    private float originalLODBias;
    private int originalVSync;
    private float originalShadowDistance;
    private int originalTextureLimit;

    [Header("Debug Ratios (Sola Lettura)")]
    public float ratio_FrameTime;
    public float ratio_GPU;
    public float ratio_CPU;
    public float ratio_Memory;

    [HideInInspector] public string statoAttuale = "Performance Ok";
    public float OttieniRisoluzioneAttuale => risoluzioneAttuale;

    [Header("Metriche Grezze per la UI")]
    [HideInInspector] public float rawFPS;
    [HideInInspector] public float rawRAM_MB;
    [HideInInspector] public float rawVRAM_MB;
    private float smoothedDeltaTime = 0.0f;

    private float baselineFT = 0f;
    private float baselineGPU = 0f;
    private float baselineCPU = 0f;
    private float baselineMemory = 0f;

    [Header("Tasti Debug")]
    [Tooltip("Tasto per forzare la ricalibrazione dei dati di riferimento in tempo reale")]
    public KeyCode ricalibrazione = KeyCode.C;

    private bool calibrato = false;
    private bool inCalibrazione = false;
    private float timerAnalisi = 0f;

    private Recorder cpuRecorder;
    private ProfilerRecorder gpuRecorder;

    void Awake()
    {
        originalFrameRate = Application.targetFrameRate;
        originalLODBias = QualitySettings.lodBias;
        originalVSync = QualitySettings.vSyncCount;
        originalShadowDistance = QualitySettings.shadowDistance;
        originalTextureLimit = QualitySettings.globalTextureMipmapLimit;

        Sampler sampler = Sampler.Get("PlayerLoop");
        if (sampler != null)
        {
            cpuRecorder = sampler.GetRecorder();
            cpuRecorder.enabled = true;
        }

        gpuRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "GPU Frame Time");
    }

    void OnDestroy()
    {
        if (gpuRecorder.Valid)
            gpuRecorder.Dispose();
    }

    IEnumerator Start()
    {
        statoAttuale = "Riscaldamento";

        // Aspetta che Unity superi il lag iniziale
        yield return new WaitForSeconds(2f);

        // Avvia la prima calibrazione automatica
        StartCoroutine(RoutineCalibrazione(4f));
    }

    void Update()
    {
        smoothedDeltaTime += (Time.unscaledDeltaTime - smoothedDeltaTime) * 0.1f;
        rawFPS = smoothedDeltaTime > 0 ? 1.0f / smoothedDeltaTime : 0f;
        rawRAM_MB = System.GC.GetTotalMemory(false) / 1048576f;
        rawVRAM_MB = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576f;

        // Ricalibrazione manuale
        if (Input.GetKeyDown(ricalibrazione) && !inCalibrazione)
        {
            StartCoroutine(RoutineCalibrazione(4f));
        }

        // Se l'IA non è calibrata o sta calibrando ora, blocca le analisi successive
        if (!calibrato || inCalibrazione) return;

        if (cooldownAttuale > 0) cooldownAttuale -= Time.unscaledDeltaTime;

        timerAnalisi += Time.unscaledDeltaTime;
        if (timerAnalisi >= intervalloAnalisi)
        {
            EseguiDiagnosiIA();
            timerAnalisi = 0f;
        }
    }

    // Coroutine riutilizzabile per stabilire o ripristinare i valori di riferimento ("Baseline")
    IEnumerator RoutineCalibrazione(float durata)
    {
        inCalibrazione = true;
        calibrato = false;

        statoAttuale = "Calibrazione in corso";
        Debug.Log("<color=yellow>[AI]</color> Ricalibrazione sistema in corso... Muovi la visuale!");

        // Ripristina la grafica allo stato originale prima di raccogliere i dati
        RipristinaSenzaCooldown();

        float sommaFT = 0;
        float sommaGPU = 0;
        float sommaCPU = 0;
        float sommaMemory = 0;
        int campioni = 0;
        float t = 0;

        while (t < durata)
        {
            sommaFT += Time.unscaledDeltaTime * 1000f;
            sommaGPU += GetGPUTime();
            sommaCPU += GetCPUTime();
            sommaMemory += (System.GC.GetTotalMemory(false) + Profiler.GetAllocatedMemoryForGraphicsDriver()) / 1048576f;

            campioni++;
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (campioni > 0)
        {
            baselineFT = sommaFT / campioni;
            baselineGPU = sommaGPU / campioni;
            baselineCPU = sommaCPU / campioni;
            baselineMemory = sommaMemory / campioni;

            if (baselineGPU <= 0.1f) baselineGPU = 1f;
            if (baselineCPU <= 0.1f) baselineCPU = 1f;
        }

        calibrato = true;
        inCalibrazione = false;
        statoAttuale = "Sistema Pronto";

        Debug.Log($"<color=green>[AI]</color> Nuova Baseline salvata! FPS: {1000f / baselineFT:F0}, GPU Time: {baselineGPU:F2}ms, CPU Time: {baselineCPU:F2}ms, RAM: {baselineMemory:F0}MB");
    }

    // Metodo di utilità per resettare i filtri senza far scattare il timer di pausa
    void RipristinaSenzaCooldown()
    {
        Application.targetFrameRate = originalFrameRate;
        QualitySettings.shadowDistance = originalShadowDistance;
        QualitySettings.lodBias = originalLODBias;
        QualitySettings.globalTextureMipmapLimit = originalTextureLimit;
        QualitySettings.maximumLODLevel = 0;

        Time.fixedDeltaTime = 0.02f;
        Physics.defaultSolverIterations = 6;

        risoluzioneAttuale = 1.0f;
        ScalableBufferManager.ResizeBuffers(1f, 1f);
    }

    float GetCPUTime()
    {
        if (cpuRecorder != null && cpuRecorder.isValid && cpuRecorder.elapsedNanoseconds > 0)
        {
            return cpuRecorder.elapsedNanoseconds / 1000000f;
        }
        // Fallback: se il recorder fallisce, prende il frameTime e toglie una stima del rendering
        return Time.unscaledDeltaTime * 1000f * 0.6f;
    }

    float GetGPUTime()
    {
        if (gpuRecorder.Valid && gpuRecorder.LastValue > 0)
        {
           return gpuRecorder.LastValue / 1000000f;
        }
        return 1.0f; // Valore di fallback generico se il profiler non è ancora pronto
    }

    void EseguiDiagnosiIA()
    {
        float currentCPU = GetCPUTime();
        float currentGPU = GetGPUTime();
        float currentMem = (System.GC.GetTotalMemory(false) + Profiler.GetAllocatedMemoryForGraphicsDriver()) / 1048576f;

        double ratioFT = (Time.unscaledDeltaTime * 1000f) / baselineFT;
        double ratioGPU = (double)currentGPU / baselineGPU;
        double ratioCPU = currentCPU / baselineCPU;
        double ratioMem = currentMem / baselineMemory;

        // Aggiorna info nell'Inspector
        ratio_FrameTime = (float)ratioFT;
        ratio_GPU = (float)ratioGPU;
        ratio_CPU = (float)ratioCPU;
        ratio_Memory = (float)ratioMem;

        // Se le prestazioni sono entro il cap forza NORMAL.
        if (ratioFT < sogliaAllarme)
        {
            ApplicaDiagnosi(3); // 3 = NORMAL
            return;
        }

        // Preparazione Input per il modello
        double[] inputIA = new double[] { ratioFT, ratioGPU, ratioCPU, ratioMem };
        double[] risultati;

        if (modelloDaUsare == TipoIA.Random_Forest_Precisa)
        {
            risultati = ML.AI_Brain.Score(inputIA);
        }
        else
        {
            risultati = ML.AI_Brain_Fast.Score(inputIA);
        }

        // Interpretazione del risultato
        int indiceVincitore = 0;
        double punteggioMassimo = risultati[0];

        for (int i = 1; i < risultati.Length; i++)
        {
            if (risultati[i] > punteggioMassimo)
            {
                punteggioMassimo = risultati[i];
                indiceVincitore = i;
            }
        }

        ApplicaDiagnosi(indiceVincitore);
    }

    void ApplicaDiagnosi(int indice)
    {
        string prefisso = (modelloDaUsare == TipoIA.Random_Forest_Precisa) ? "[RF] " : "[Tree] ";

        // Ordine alfabetico Scikit-Learn: 0:CPU, 1:GPU, 2:MEMORY, 3:NORMAL, 4:PHYSICS
        switch (indice)
        {
            case 0: statoAttuale = "Stress CPU"; break;
            case 1: statoAttuale = "Stress GPU"; break;
            case 2: statoAttuale = "Memory Leak"; break;
            case 3: statoAttuale = "Performance OK"; break;
            case 4: statoAttuale = "Stress Fisica"; break;
        }

        if (!correzioneAutomatica) return;

        // Gestione Logica Autopilota
        if (indice == 3) // Stato NORMAL
        {
            contatoreNormal++;
            // Ripristina se è stato normale per 15 cicli (7.5 secondi) e il cooldown è finito
            if (contatoreNormal >= tempoPerRipristino && cooldownAttuale <= 0)
            {
                RipristinaGraduale();
                contatoreNormal = 0;
            }
        }
        else // Stato di STRESS
        {
            contatoreNormal = 0;
            if (cooldownAttuale <= 0)
            {
                Intervieni(indice);
            }
        }
    }

    void Intervieni(int tipo)
    {
        cooldownAttuale = tempoDiCooldown;
        string nomeProblema = "";

        switch (tipo)
        {
            case 0: // CPU
                nomeProblema = "CPU";
                QualitySettings.lodBias = 0.1f;
                QualitySettings.maximumLODLevel = 2;
                Debug.Log("<color=orange>[AI]</color> CPU Fix");
                break;
            case 1: // GPU
                nomeProblema = "GPU";
                if (risoluzioneAttuale > risoluzioneMinima)
                {
                    risoluzioneAttuale = Mathf.Max(risoluzioneMinima, risoluzioneAttuale - stepRisoluzione);
                    ScalableBufferManager.ResizeBuffers(risoluzioneAttuale, risoluzioneAttuale);
                    Debug.Log($"<color=red>[AI]</color> GPU Fix: Risoluzione scalata al {Mathf.RoundToInt(risoluzioneAttuale * 100)}%");
                }
                else
                {
                    QualitySettings.shadowDistance = 0f;
                    Debug.Log("<color=red>[AI]</color> Risoluzione minima raggiunta. Ombre disattivate.");
                }
                break;
            case 2: // MEMORY
                nomeProblema = "MEMORY";
                QualitySettings.globalTextureMipmapLimit = 2;
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                Debug.Log("<color=magenta>[AI]</color> Memory Fix");
                break;
            case 4: // PHYSICS
                nomeProblema = "PHYSICS";
                Time.fixedDeltaTime = 0.06f; // Solo ~16 calcoli fisici al secondo (invece di 50)
                Physics.defaultSolverIterations = 1; // Precisione collisioni minima
                Debug.Log("<color=cyan>[AI]</color> Physics Fix");
                break;
        }

        if (heatmapLogger != null)
        {
            heatmapLogger.RecordIssue(nomeProblema, ratio_FrameTime);
        }
    }

    void RipristinaGraduale()
    {
        // Se è già ai valori originali, non fa nulla
        if (Application.targetFrameRate == originalFrameRate &&
            QualitySettings.lodBias == originalLODBias &&
            QualitySettings.shadowDistance == originalShadowDistance &&
            QualitySettings.globalTextureMipmapLimit == originalTextureLimit &&
            risoluzioneAttuale >= 1.0f) return;

        cooldownAttuale = 1.5f; // Mette in pausa per 1.5 secondi per far stabilizzare il frame dopo l'aumento

        // Risoluzione
        if (risoluzioneAttuale < 1.0f)
        {
            risoluzioneAttuale = Mathf.Min(1.0f, risoluzioneAttuale + stepRisoluzione);
            ScalableBufferManager.ResizeBuffers(risoluzioneAttuale, risoluzioneAttuale);
            Debug.Log($"<color=green>[AI]</color> Ripristino Graduale: Risoluzione al {Mathf.RoundToInt(risoluzioneAttuale * 100)}%");
            return; // Esce dalla funzione. Ripristinerà il resto al prossimo giro
        }

        // Ombra
        if (QualitySettings.shadowDistance < originalShadowDistance)
        {
            QualitySettings.shadowDistance = originalShadowDistance;
            Debug.Log("<color=green>[AI]</color> Ripristino Graduale: Ombre riattivate.");
            return;
        }

        // LOD Bias
        if (QualitySettings.lodBias < originalLODBias || QualitySettings.maximumLODLevel > 0)
        {
            // Aumenta a piccoli passi di 0.3
            QualitySettings.lodBias = Mathf.Min(originalLODBias, QualitySettings.lodBias + 0.3f);

            // Se ha raggiunto o superato il valore originale, ripristina anche il livello massimo
            if (QualitySettings.lodBias >= originalLODBias)
            {
                QualitySettings.maximumLODLevel = 0;
            }
            Debug.Log($"<color=green>[AI]</color> Ripristino Graduale: LOD Bias a {QualitySettings.lodBias:F2}");
            return;
        }

        // Fisica
        if (Time.fixedDeltaTime > 0.02f)
        {
            Time.fixedDeltaTime = 0.02f;
            Physics.defaultSolverIterations = 6;
            Debug.Log("<color=green>[AI]</color> Ripristino Graduale: Calcoli fisici normalizzati.");
            return;
        }

        // Texture e framerate
        if (QualitySettings.globalTextureMipmapLimit != originalTextureLimit || Application.targetFrameRate != originalFrameRate)
        {
            QualitySettings.globalTextureMipmapLimit = originalTextureLimit;
            Application.targetFrameRate = originalFrameRate;
            Debug.Log("<color=green>[AI]</color> Ripristino Graduale: Qualità Texture normalizzata. Sistema 100% stabile.");
            return;
        }

        Application.targetFrameRate = originalFrameRate;
        QualitySettings.shadowDistance = originalShadowDistance;
        QualitySettings.lodBias = originalLODBias;
        QualitySettings.globalTextureMipmapLimit = originalTextureLimit;
        QualitySettings.maximumLODLevel = 0;

        Time.fixedDeltaTime = 0.02f;
        Physics.defaultSolverIterations = 6;

        // Ripristino Dynamic Resolution
        risoluzioneAttuale = 1.0f;
        ScalableBufferManager.ResizeBuffers(risoluzioneAttuale, risoluzioneAttuale);

        Debug.Log("<color=green>[AI]</color> Prestazioni tornate normali. Ripristino impostazioni.");
    }
}