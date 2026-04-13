using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StressTester : MonoBehaviour
{
    [Header("Riferimento")]
    public PerformanceLogger logger; // Trascina qui l'oggetto che ha il PerformanceLogger

    [Header("Stato")]
    [SerializeField] private bool cpuStressAttivo = false;
    [SerializeField] private int intensitaCpu = 0;
    private List<GameObject> istanzeGPU = new List<GameObject>();

    IEnumerator Start()
    {
        // Fase iniziale: Calibrazione
        if (logger != null) logger.scenarioLabel = "CALIBRATION";
        Debug.Log("Fase di CALIBRAZIONE attiva per 6 secondi...");

        yield return new WaitForSeconds(6f);

        // Dopo 6 secondi passa
        if (logger != null) logger.scenarioLabel = "NORMAL";
        Debug.Log("SISTEMA PRONTO: Ora puoi usare i tasti 1-6");
    }

    void Update()
    {
        // --- LIVELLI CPU ---
        if (Input.GetKeyDown(KeyCode.Alpha1)) AttivaStressCPU(100000, "CPU_STRESS");
        if (Input.GetKeyDown(KeyCode.Alpha2)) AttivaStressCPU(1000000, "CPU_STRESS");
        if (Input.GetKeyDown(KeyCode.Alpha3)) AttivaStressCPU(5000000, "CPU_STRESS");

        // --- LIVELLI GPU ---
        if (Input.GetKeyDown(KeyCode.Alpha4)) AttivaStressGPU(500, "GPU_STRESS");
        if (Input.GetKeyDown(KeyCode.Alpha5)) AttivaStressGPU(3000, "GPU_STRESS");
        if (Input.GetKeyDown(KeyCode.Alpha6)) AttivaStressGPU(15000, "GPU_STRESS");

        // --- RESET ---
        if (Input.GetKeyDown(KeyCode.Space)) FermaTutto();

        if (cpuStressAttivo)
        {
            double dummy = 0;
            for (int i = 0; i < intensitaCpu; i++) { dummy += Mathf.Sqrt(i); }
        }
    }

    void AttivaStressCPU(int intensita, string label)
    {
        FermaTutto();
        intensitaCpu = intensita;
        cpuStressAttivo = true;
        if (logger != null) logger.scenarioLabel = label;
        Debug.Log($"Attivato {label} - Intensità: {intensita}");
    }

    void AttivaStressGPU(int numeroOggetti, string label)
    {
        FermaTutto();
        if (logger != null) logger.scenarioLabel = label;

        Material mat = new Material(Shader.Find("Standard"));
        for (int i = 0; i < numeroOggetti; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = Random.insideUnitSphere * 15f;
            Destroy(go.GetComponent<SphereCollider>());
            go.GetComponent<MeshRenderer>().material = mat;
            istanzeGPU.Add(go);
        }
        Debug.Log($"Attivato {label} - Oggetti: {numeroOggetti}");
    }

    void FermaTutto()
    {
        cpuStressAttivo = false;
        intensitaCpu = 0;
        if (logger != null) logger.scenarioLabel = "NORMAL";

        foreach (var clone in istanzeGPU) { if (clone != null) Destroy(clone); }
        istanzeGPU.Clear();
        Debug.Log("Reset: Ritorno a NORMAL");
    }
}