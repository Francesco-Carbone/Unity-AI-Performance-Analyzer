using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StressTester : MonoBehaviour
{
    [Header("Riferimento")]
    public PerformanceLogger logger;

    [Header("Stato Stress Attuale")]
    public bool cpuStressAttivo = false;
    public int intensitaCpu = 0;

    public bool gpuStressAttivo = false;
    public int numeroOggettiGpu = 0;
    private List<GameObject> istanzeGPU = new List<GameObject>();

    public bool physicsStressAttivo = false;
    public int numeroOggettiFisici = 0;
    private List<GameObject> istanzeFisica = new List<GameObject>();

    public bool memoryStressAttivo = false;
    public int mbDaAllocarePerFrame = 0;

    IEnumerator Start()
    {
        // Fase iniziale: Calibrazione
        if (logger != null) logger.scenarioLabel = "CALIBRATION";
        Debug.Log("Fase di CALIBRAZIONE attiva per 6 secondi...");

        yield return new WaitForSeconds(6f);

        // Dopo 6 secondi passa
        if (logger != null) logger.scenarioLabel = "NORMAL";
        Debug.Log("SISTEMA PRONTO: Usa F1-F3 (CPU), F4-F6 (GPU), F7-F8 (Fisica), F9 (Memoria)");
    }

    void Update()
    {
        // CPU
        if (Input.GetKeyDown(KeyCode.F1)) AttivaStressCPU(100000, "CPU_STRESS");
        if (Input.GetKeyDown(KeyCode.F2)) AttivaStressCPU(1000000, "CPU_STRESS");
        if (Input.GetKeyDown(KeyCode.F3)) AttivaStressCPU(5000000, "CPU_STRESS");

        // GPU
        if (Input.GetKeyDown(KeyCode.F4)) AttivaStressGPU(500, "GPU_STRESS");
        if (Input.GetKeyDown(KeyCode.F5)) AttivaStressGPU(3000, "GPU_STRESS");
        if (Input.GetKeyDown(KeyCode.F6)) AttivaStressGPU(15000, "GPU_STRESS");

        // Fisica
        if (Input.GetKeyDown(KeyCode.F7)) AttivaStressFisica(200, "PHYSICS_STRESS");
        if (Input.GetKeyDown(KeyCode.F8)) AttivaStressFisica(800, "PHYSICS_STRESS");

        // Memoria (Garbage Collector Stutter)
        if (Input.GetKeyDown(KeyCode.F9)) AttivaStressMemoria(10, "MEMORY_STRESS"); // Alloca 10MB al frame

        // Reset
        if (Input.GetKeyDown(KeyCode.Space)) FermaTutto();

        if (cpuStressAttivo)
        {
            double dummy = 0;
            for (int i = 0; i < intensitaCpu; i++) { dummy += Mathf.Sqrt(i); }
        }

        if (memoryStressAttivo)
        {
            // Alloca array enormi ogni frame per forzare il Garbage Collector
            byte[] spazzatura = new byte[1024 * 1024 * mbDaAllocarePerFrame];
        }
    }

    void AttivaStressCPU(int intensita, string label)
    {
        FermaTutto();
        intensitaCpu = intensita;
        cpuStressAttivo = true;
        SetLabel(label);
        //Debug.Log($"Attivato {label} - Intensità: {intensita}");
    }

    void AttivaStressGPU(int numeroOggetti, string label)
    {
        FermaTutto();
        gpuStressAttivo = true;
        numeroOggettiGpu = numeroOggetti;
        SetLabel(label);

        Material mat = new Material(Shader.Find("Standard"));
        for (int i = 0; i < numeroOggetti; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = Random.insideUnitSphere * 15f;
            Destroy(go.GetComponent<SphereCollider>());
            go.GetComponent<MeshRenderer>().material = mat;
            istanzeGPU.Add(go);
        }
        //Debug.Log($"Attivato {label} - Oggetti: {numeroOggetti}");
    }

    void AttivaStressFisica(int numeroOggetti, string label)
    {
        FermaTutto();
        physicsStressAttivo = true;
        numeroOggettiFisici = numeroOggetti;
        SetLabel(label);

        Material mat = new Material(Shader.Find("Standard"));
        for (int i = 0; i < numeroOggetti; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(10f, 50f), Random.Range(-5f, 5f));
            go.AddComponent<Rigidbody>(); // Aggiunge gravità e collisioni
            go.GetComponent<MeshRenderer>().material = mat;
            istanzeFisica.Add(go);
        }
    }

    void AttivaStressMemoria(int mb, string label)
    {
        FermaTutto();
        memoryStressAttivo = true;
        mbDaAllocarePerFrame = mb;
        SetLabel(label);
    }

    void SetLabel(string label)
    {
        if (logger != null) logger.scenarioLabel = label;
        Debug.Log($"Attivato: {label}");
    }

    void FermaTutto()
    {
        cpuStressAttivo = false; gpuStressAttivo = false; physicsStressAttivo = false; memoryStressAttivo = false;
        SetLabel("NORMAL");
        foreach (var clone in istanzeGPU)
        {
            if (clone != null) Destroy(clone);
        }
        foreach (var clone in istanzeFisica)
        {
            if (clone != null) Destroy(clone);
        }
        istanzeGPU.Clear();
        istanzeFisica.Clear();
    }
}