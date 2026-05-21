using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq;
using ML;
using UnityEngine.Profiling;

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
    public float ratio_Batches;
    public float ratio_CPU;
    public float ratio_Memory;

    [HideInInspector] public string statoAttuale = "Performance Ok";
    public float OttieniRisoluzioneAttuale => risoluzioneAttuale;

    [Header("Metriche Grezze per la UI")]
    [HideInInspector] public float rawFPS;
    [HideInInspector] public float rawRAM_MB;
    private float smoothedDeltaTime = 0.0f;

    private float baselineFT = 0f;
    private float baselineBatches = 0f;
    private float baselineCPU = 0f;
    private float baselineMemory = 0f;

    private bool calibrato = false;
    private float timerAnalisi = 0f;

    private Recorder cpuRecorder;

    void Awake()
    {
        originalFrameRate = Application.targetFrameRate;
        originalLODBias = QualitySettings.lodBias;
        originalVSync = QualitySettings.vSyncCount;
        originalShadowDistance = QualitySettings.shadowDistance;
        originalTextureLimit = QualitySettings.masterTextureLimit;

    Sampler sampler = Sampler.Get("PlayerLoop");
        if (sampler != null)
        {
            cpuRecorder = sampler.GetRecorder();
            cpuRecorder.enabled = true;
        }
    }

    IEnumerator Start()
    {
        statoAttuale = "AI: Riscaldamento";

        // Aspetta che Unity superi il lag iniziale
        yield return new WaitForSeconds(2f);

        statoAttuale = "AI: Calibrazione hardware in corso";

        float sommaFT = 0;
        float sommaBatches = 0;
        float sommaCPU = 0;
        float sommaMemory = 0;
        int campioni = 0;
        float t = 0;

        // Fase di calibrazione: 4 secondi per mappare le prestazioni standard del PC locale
        while (t < 4f)
        { 
            sommaFT += Time.unscaledDeltaTime * 1000f;
            sommaBatches += UnityEditor.UnityStats.batches;
            sommaCPU += GetCPUTime();
            sommaMemory += System.GC.GetTotalMemory(false) / 1048576f;
            campioni++;
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        baselineFT = sommaFT / campioni;
        baselineBatches = sommaBatches / campioni;
        baselineCPU = sommaCPU / campioni;
        baselineMemory = sommaMemory / campioni;

        if (baselineBatches <= 0) baselineBatches = 1;
        if (baselineCPU <= 0) baselineCPU = 1;

        calibrato = true;
        statoAttuale = "AI: Sistema Pronto";
    }

    void Update()
    {
        smoothedDeltaTime += (Time.unscaledDeltaTime - smoothedDeltaTime) * 0.1f;
        rawFPS = smoothedDeltaTime > 0 ? 1.0f / smoothedDeltaTime : 0f;
        rawRAM_MB = System.GC.GetTotalMemory(false) / 1048576f;

        if (!calibrato) return;

        if (cooldownAttuale > 0) cooldownAttuale -= Time.unscaledDeltaTime;

        timerAnalisi += Time.unscaledDeltaTime;
        if (timerAnalisi >= intervalloAnalisi)
        {
            EseguiDiagnosiIA();
            timerAnalisi = 0f;
        }
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

    void EseguiDiagnosiIA()
    {
        float currentCPU = GetCPUTime();

        double ratioFT = (Time.unscaledDeltaTime * 1000f) / baselineFT;
        double ratioBT = (double)UnityEditor.UnityStats.batches / baselineBatches;
        double ratioCPU = currentCPU / baselineCPU;
        double ratioMem = (System.GC.GetTotalMemory(false) / 1048576f) / baselineMemory;

        // Aggiorna info nell'Inspector
        ratio_FrameTime = (float)ratioFT;
        ratio_Batches = (float)ratioBT;
        ratio_CPU = (float)ratioCPU;
        ratio_Memory = (float)ratioMem;

        // Se le prestazioni sono entro il cap forza NORMAL.
        if (ratioFT < sogliaAllarme)
        {
            ApplicaDiagnosi(3); // 3 = NORMAL
            return;
        }

        // Preparazione Input per il modello
        double[] inputIA = new double[] { ratioFT, ratioBT, ratioCPU, ratioMem };
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
            case 0: 
                statoAttuale = "Stress CPU"; break;
            case 1: 
                statoAttuale = "Stress GPU"; break;
            case 2: 
                statoAttuale = "Memory Leak"; break;
            case 3: 
                statoAttuale = "Performance OK"; break;
            case 4: 
                statoAttuale = "Stress Fisica"; break;
        }

        if (!correzioneAutomatica) return;

        // Gestione Logica Autopilota
        if (indice == 3) // Stato NORMAL
        {
            contatoreNormal++;
            // Ripristina se è stato normale per 15 cicli (7.5 secondi) e il cooldown è finito
            if (contatoreNormal >= tempoPerRipristino && cooldownAttuale <= 0)
            {
                Ripristina();
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
                QualitySettings.masterTextureLimit = 2;
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

    void Ripristina()
    {
        // Se è già ai valori originali, non fa nulla
        if (Application.targetFrameRate == originalFrameRate &&
            QualitySettings.lodBias == originalLODBias &&
            QualitySettings.shadowDistance == originalShadowDistance &&
            QualitySettings.masterTextureLimit == originalTextureLimit &&
            risoluzioneAttuale >= 1.0f) return;

        cooldownAttuale = 2.0f; // Mette in pausa per 2 secondi per stabilizzare

        Application.targetFrameRate = originalFrameRate;
        QualitySettings.shadowDistance = originalShadowDistance;
        QualitySettings.lodBias = originalLODBias;
        QualitySettings.masterTextureLimit = originalTextureLimit;
        QualitySettings.maximumLODLevel = 0;

        Time.fixedDeltaTime = 0.02f;
        Physics.defaultSolverIterations = 6;

        // Ripristino Dynamic Resolution
        risoluzioneAttuale = 1.0f;
        ScalableBufferManager.ResizeBuffers(risoluzioneAttuale, risoluzioneAttuale);

        Debug.Log("<color=green>[AI]</color> Prestazioni tornate normali. Ripristino impostazioni.");
    }
}