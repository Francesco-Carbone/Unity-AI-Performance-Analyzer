using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq;
using ML;

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

    private float baselineFT = 0f;
    private float baselineBatches = 0f;
    private float baselineCPU = 0f;

    private bool calibrato = false;
    private float timerAnalisi = 0f;

    private double startTimeFrame;
    private float cpuTimeMS;

    IEnumerator Start()
    {
        MostraMessaggio("AI: Riscaldamento...", Color.yellow);

        // Aspetta che Unity superi il lag iniziale
        yield return new WaitForSeconds(2f);

        MostraMessaggio("AI: Calibrazione hardware in corso...", Color.white);

        float sommaFT = 0;
        float sommaBatches = 0;
        float sommaCPU = 0;
        int campioni = 0;
        float t = 0;

        // Fase di calibrazione: 4 secondi per mappare le prestazioni standard del PC locale
        while (t < 4f)
        {
            sommaFT += Time.unscaledDeltaTime * 1000f;
            sommaBatches += UnityEditor.UnityStats.batches;
            sommaCPU += cpuTimeMS;
            campioni++;
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        baselineFT = sommaFT / campioni;
        baselineBatches = sommaBatches / campioni;
        baselineCPU = sommaCPU / campioni;

        if (baselineBatches <= 0) baselineBatches = 1;
        if (baselineCPU <= 0) baselineCPU = baselineFT * 0.5f;

        calibrato = true;
        MostraMessaggio("AI: Sistema Pronto.", Color.green);
    }

    void Update()
    {
        startTimeFrame = (double)Time.realtimeSinceStartup;
    }

    void LateUpdate()
    {
        cpuTimeMS = (float)((double)Time.realtimeSinceStartup - startTimeFrame) * 1000f;

        if (!calibrato) return;

        timerAnalisi += Time.unscaledDeltaTime;
        if (timerAnalisi >= intervalloAnalisi)
        {
            EseguiDiagnosiIA();
            timerAnalisi = 0f;
        }
    }

    void EseguiDiagnosiIA()
    {
        if (!calibrato) return;

        double ratioFT = (Time.unscaledDeltaTime * 1000f) / baselineFT;
        double ratioBT = (double)UnityEditor.UnityStats.batches / baselineBatches;
        double ratioCPU = (double)cpuTimeMS / baselineCPU;

        // Aggiorna info nell'Inspector
        ratio_FrameTime = (float)ratioFT;
        ratio_Batches = (float)ratioBT;
        ratio_CPU = (float)ratioCPU;

        // Se le prestazioni sono entro il cap forza NORMAL.
        if (ratioFT < sogliaAllarme)
        {
            ApplicaDiagnosi(2); // 2 = NORMAL
            return;
        }

        // Preparazione Input per il modello
        double[] inputIA = new double[] { ratioFT, ratioBT, ratioCPU };
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

        switch (indice)
        {
            case 0: MostraMessaggio(prefisso + "Stress CPU!", new Color(1f, 0.5f, 0f)); break;
            case 1: MostraMessaggio(prefisso + "Stress GPU!", Color.red); break;
            case 2: MostraMessaggio(prefisso + "Performance OK", Color.green); break;
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