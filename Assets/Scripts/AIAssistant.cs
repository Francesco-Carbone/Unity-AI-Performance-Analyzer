using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq;
using ML;

public class AIAssistant : MonoBehaviour
{
    public TextMeshProUGUI testoAssistente;

    [Header("Configurazione Analisi")]
    public float intervalloAnalisi = 0.5f;

    private float baselineFT = 0f;
    private float baselineBatches = 0f;
    private float baselineCPU = 0f;

    private bool calibrato = false;
    private float timerAnalisi = 0f;

    private float lastFrameStartTime = 0f;
    private float currentCpuTime = 0f;

    void Awake()
    {
        lastFrameStartTime = Time.realtimeSinceStartup;
    }

    IEnumerator Start()
    {
        MostraMessaggio("AI: Riscaldamento motore...", Color.yellow);

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
            sommaCPU += currentCpuTime;
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
        float frameStart = Time.realtimeSinceStartup;
        currentCpuTime = (frameStart - lastFrameStartTime) * 1000f;
        lastFrameStartTime = frameStart;

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
        double ratioCPU = (double)currentCpuTime / baselineCPU;

        // Se le prestazioni sono entro il 30% forza NORMAL.
        if (ratioFT < 1.3)
        {
            ApplicaDiagnosi(2); // 2 = NORMAL
            return;
        }

        // Preparazione Input per il modello [Delta_FT, Delta_Batches]
        double[] inputIA = new double[] { ratioFT, ratioBT, ratioCPU };
        double[] risultati = AI_Brain.Score(inputIA);

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
        switch (indice)
        {
            case 0: // CPU_STRESS
                MostraMessaggio("AI: Rilevato stress CPU (Logica/Fisica pesante)!", new Color(1f, 0.5f, 0f));
                break;
            case 1: // GPU_STRESS
                MostraMessaggio("AI: Rilevato stress GPU (Troppi poligoni/drawcalls)!", Color.red);
                break;
            case 2: // NORMAL
                MostraMessaggio("AI: Performance ottimali.", Color.green);
                break;
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