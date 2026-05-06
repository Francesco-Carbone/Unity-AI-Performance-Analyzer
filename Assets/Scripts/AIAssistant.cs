using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq;
using ML;
using UnityEngine.Profiling;

public class AIAssistant : MonoBehaviour
{
    public TextMeshProUGUI testoAssistente;

    public enum TipoIA { Random_Forest_Precisa, Decision_Tree_Veloce }

    [Header("Configurazione")]
    public TipoIA modelloDaUsare = TipoIA.Decision_Tree_Veloce;
    public float intervalloAnalisi = 0.5f;
    [Tooltip("Soglia di allarme: 1.1 = Molto Sensibile (+10% lag), 2.0 = Poco Sensibile (+100% lag).")]
    [Range(1.05f, 3f)]
    public float sogliaAllarme = 1.3f;

    [Header("Autopilota Prestazioni")]
    public bool correzioneAutomatica = true;
    public float tempoDiCooldown = 5.0f; // Secondi tra un intervento e l'altro
    public int tempoPerRipristino = 15; // Quanti cicli "Normal" servono per rialzare la qualità

    private float cooldownAttuale = 0f;
    private int contatoreNormal = 0;

    // Variabili per salvare lo stato originale del progetto
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
        MostraMessaggio("AI: Riscaldamento...", Color.yellow);

        // Aspetta che Unity superi il lag iniziale
        yield return new WaitForSeconds(2f);

        MostraMessaggio("AI: Calibrazione hardware in corso...", Color.white);

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
        MostraMessaggio("AI: Sistema Pronto.", Color.green);
    }

    void Update()
    {
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
            case 0: MostraMessaggio(prefisso + "Stress CPU!", new Color(1f, 0.5f, 0f)); break;
            case 1: MostraMessaggio(prefisso + "Stress GPU!", Color.red); break;
            case 2: MostraMessaggio(prefisso + "Memory Leak!", Color.magenta); break;
            case 3: MostraMessaggio(prefisso + "Performance OK", Color.green); break;
            case 4: MostraMessaggio(prefisso + "Stress Fisica!", Color.cyan); break;
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

        switch (tipo)
        {
            case 0: // CPU
                QualitySettings.lodBias = 0.1f;
                QualitySettings.maximumLODLevel = 2;
                Debug.Log("<color=orange>[AI]</color> CPU Fix");
                break;
            case 1: // GPU
                QualitySettings.shadowDistance = 0f; // Disabilita totalmente le ombre
                QualitySettings.masterTextureLimit = 1; // Dimezza risoluzione texture
                Debug.Log("<color=red>[AI]</color> GPU Fix");
                break;
            case 2: // MEMORY
                QualitySettings.masterTextureLimit = 2;
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                Debug.Log("<color=magenta>[AI]</color> Memory Fix");
                break;
            case 4: // PHYSICS
                Time.fixedDeltaTime = 0.06f; // Solo ~16 calcoli fisici al secondo (invece di 50)
                Physics.defaultSolverIterations = 1; // Precisione collisioni minima
                Debug.Log("<color=cyan>[AI]</color> Physics Fix");
                break;
        }
    }

    void Ripristina()
    {
        // Se è già ai valori originali, non fa nulla
        if(Application.targetFrameRate == originalFrameRate && QualitySettings.lodBias == originalLODBias &&
            QualitySettings.shadowDistance == originalShadowDistance && QualitySettings.masterTextureLimit == originalTextureLimit) return;

        cooldownAttuale = 2.0f; // Mette in pausa per 2 secondi per stabilizzare

        Application.targetFrameRate = originalFrameRate;
        QualitySettings.shadowDistance = originalShadowDistance;
        QualitySettings.lodBias = originalLODBias;
        QualitySettings.masterTextureLimit = originalTextureLimit;
        QualitySettings.maximumLODLevel = 0;

        Time.fixedDeltaTime = 0.02f;
        Physics.defaultSolverIterations = 6;

        Debug.Log("<color=green>[AI]</color> Prestazioni tornate normali. Ripristino impostazioni.");
    }

    void MostraMessaggio(string messaggio, Color colore)
    {
        if (testoAssistente != null)
        {
            testoAssistente.text = messaggio;
            testoAssistente.color = colore;
        }
    }
}