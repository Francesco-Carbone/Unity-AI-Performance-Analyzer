using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StressTester : MonoBehaviour
{
    [Header("Riferimenti")]
    public PerformanceLogger logger;

    [Header("Controlli Tastiera")]
    public KeyCode tastoCPU = KeyCode.Alpha1; // Tasto '1'
    public KeyCode tastoGPU = KeyCode.Alpha2; // Tasto '2'

    [Header("Stato Test (Sola Lettura)")]
    public bool stressCPU = false;
    public bool stressGPU = false;

    [Header("Parametri")]
    public int intensitaCalcolo = 1000000;
    private List<GameObject> istanzeGPU = new List<GameObject>();
    private bool calibrazioneFinita = false;

    IEnumerator Start()
    {
        if (logger != null) logger.scenarioLabel = "CALIBRATION";
        yield return new WaitForSeconds(6f); // Aspetta la calibrazione dell'IA

        calibrazioneFinita = true;
        if (logger != null) logger.scenarioLabel = "NORMAL";
        Debug.Log("SISTEMA PRONTO: Premi '1' per CPU Stress, '2' per GPU Stress");
    }

    void Update()
    {
        if (!calibrazioneFinita) return;

        // Toggle Stress CPU con tasto 1
        if (Input.GetKeyDown(tastoCPU))
        {
            stressCPU = !stressCPU;
            AggiornaLabel();
        }

        // Toggle Stress GPU con tasto 2
        if (Input.GetKeyDown(tastoGPU))
        {
            stressGPU = !stressGPU;
            AggiornaLabel();
        }

        EseguiStress();
    }

    void AggiornaLabel()
    {
        if (logger == null) return;

        if (stressGPU) logger.scenarioLabel = "GPU_STRESS";
        else if (stressCPU) logger.scenarioLabel = "CPU_STRESS";
        else logger.scenarioLabel = "NORMAL";
    }

    void EseguiStress()
    {
        // Logica CPU
        if (stressCPU)
        {
            for (int i = 0; i < intensitaCalcolo; i++)
            {
                float valore = Mathf.Sqrt(Random.value) * Mathf.Exp(Random.value);
            }
        }

        // Logica GPU
        if (stressGPU && istanzeGPU.Count < 10000)
        {
            for (int i = 0; i < 100; i++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = Random.insideUnitSphere * 20;
                istanzeGPU.Add(go);
            }
        }
        else if (!stressGPU && istanzeGPU.Count > 0)
        {
            foreach (GameObject obj in istanzeGPU) Destroy(obj);
            istanzeGPU.Clear();
        }
    }
}