using UnityEngine;
using System.Collections.Generic;

public class StressTester : MonoBehaviour
{
    [Header("Stress CPU (Calcoli Pesanti)")]
    public bool stressCPU = false;
    public int intensitaCalcolo = 1000000;

    [Header("Stress GPU (Rendering Pesante)")]
    public bool stressGPU = false;
    public GameObject oggettoPesante;
    private List<GameObject> istanzeGPU = new List<GameObject>();

    void Update()
    {
        // Simulazione stress CPU
        if (stressCPU)
        {
            // Esegue calcoli matematici inutili per occupare il thread principale
            for (int i = 0; i < intensitaCalcolo; i++)
            {
                float valore = Mathf.Sqrt(Random.value) * Mathf.Exp(Random.value);
            }
        }

        // Simulazione stress GPU
        if (stressGPU && istanzeGPU.Count < 10000)
        {
            // Crea velocemente oggetti complessi per aumentare Draw Calls e Vertici
            for (int i = 0; i < 10; i++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = Random.insideUnitSphere * 10;
                istanzeGPU.Add(go);
            }
        }
        else if (!stressGPU && istanzeGPU.Count > 0)
        {
            // Pulizia quando disattiva il test
            foreach (GameObject obj in istanzeGPU) Destroy(obj);
            istanzeGPU.Clear();
        }
    }
}