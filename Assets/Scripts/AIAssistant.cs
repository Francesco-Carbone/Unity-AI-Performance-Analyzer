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

    [Header("Sensibilità IA")]
    [Tooltip("Soglia di allarme: 1.1 = Molto Sensibile (+10% lag), 2.0 = Poco Sensibile (+100% lag).")]
    [Range(1.05f, 3f)]
    public float sogliaAllarme = 1.3f;

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
            float cpuTimeMs = GetCPUTime();

            sommaFT += Time.unscaledDeltaTime * 1000f;
            sommaBatches += UnityEditor.UnityStats.batches;
            sommaCPU += cpuTimeMs;
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

        timerAnalisi += Time.unscaledDeltaTime;
        if (timerAnalisi >= intervalloAnalisi)
        {
            EseguiDiagnosiIA();
            timerAnalisi = 0f;
        }
    }

    float GetCPUTime()
    {
        // Se il recorder non è disponibile o non ha dati, usa un calcolo approssimativo
        if (cpuRecorder != null && cpuRecorder.isValid && cpuRecorder.elapsedNanoseconds > 0)
        {
            return cpuRecorder.elapsedNanoseconds / 1000000f;
        }
        // Fallback: se il recorder fallisce, prende il frameTime e togle una stima del rendering
        return Time.unscaledDeltaTime * 1000f * 0.6f;
    }

    void EseguiDiagnosiIA()
    {
        if (!calibrato) return;

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

        // Mappa degli indici (L'ordine alfabetico di Scikit-Learn: CPU_STRESS, GPU_STRESS, NORMAL)
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