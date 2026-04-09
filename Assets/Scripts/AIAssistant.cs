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
    private bool calibrato = false;
    private float timerAnalisi = 0f;

    IEnumerator Start()
    {
        MostraMessaggio("AI: Calibrazione hardware in corso...", Color.white);

        float sommaFT = 0;
        float sommaBatches = 0;
        int campioni = 0;
        float t = 0;

        // Fase di calibrazione: 5 secondi per mappare le prestazioni "normali" del PC locale
        while (t < 5f)
        {
            sommaFT += Time.unscaledDeltaTime * 1000f;
            sommaBatches += UnityEditor.UnityStats.batches;
            campioni++;
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        baselineFT = sommaFT / campioni;
        baselineBatches = sommaBatches / campioni;
        if (baselineBatches == 0) baselineBatches = 1;

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

    void EseguiDiagnosiIA()
    {
        // Rilevamento dati attuali
        float currentFT = Time.unscaledDeltaTime * 1000f;
        float currentBatches = UnityEditor.UnityStats.batches;

        // Calcolo dei Delta (Rapporti relativi rispetto alla calibrazione)
        double deltaFT = (double)(currentFT / baselineFT);
        double deltaBatches = (double)(currentBatches / baselineBatches);

        // Se le prestazioni sono entro il 20% della norma (Delta tra 0.8 e 1.2), è NORMAL.
        if (deltaFT < 1.2 && deltaBatches < 1.2)
        {
            ApplicaDiagnosi(2); // Forza NORMAL
            return;
        }

        // Preparazione Input per il modello [Delta_FT, Delta_Batches]
        double[] inputIA = new double[] { deltaFT, deltaBatches };

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