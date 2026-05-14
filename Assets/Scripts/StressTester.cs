using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StressTester : MonoBehaviour
{
    [Header("Riferimento")]
    public PerformanceLogger logger;

    [Header("Parametri Test (Configurabili)")]
    [Tooltip("Intensità del calcolo matematico per ogni livello CPU")]
    public int cpuStepPower = 100000;

    [Tooltip("Numero di sfere cariche di shader per ogni livello GPU")]
    public int gpuStepObjects = 500;

    [Tooltip("Numero di sfere fisiche con collisioni per ogni livello Fisica")]
    public int physicsStepObjects = 200;

    [Tooltip("Quanti MB di memoria allocare ad ogni frame per il test RAM")]
    public int memoryStepMB = 10;

    [Header("Stato Stress Attuale")]
    public bool cpuStress = false;
    public int intensitaCpu = 0;

    public bool gpuStress = false;
    public int numOggettiGpu = 0;
    private List<GameObject> istanzeGPU = new List<GameObject>();

    public bool physicsStress = false;
    public int numOggettiFisici = 0;
    private List<GameObject> istanzeFisica = new List<GameObject>();

    public bool memoryStress = false;
    public int mbAllocataFrame = 0;

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
        if (Input.GetKeyDown(KeyCode.F1)) AttivaStressCPU(cpuStepPower, "CPU_STRESS");
        if (Input.GetKeyDown(KeyCode.F2)) AttivaStressCPU(cpuStepPower * 10, "CPU_STRESS");
        if (Input.GetKeyDown(KeyCode.F3)) AttivaStressCPU(cpuStepPower * 50, "CPU_STRESS");

        // GPU
        if (Input.GetKeyDown(KeyCode.F4)) AttivaStressGPU(gpuStepObjects, "GPU_STRESS");
        if (Input.GetKeyDown(KeyCode.F5)) AttivaStressGPU(gpuStepObjects * 6, "GPU_STRESS");
        if (Input.GetKeyDown(KeyCode.F6)) AttivaStressGPU(gpuStepObjects * 30, "GPU_STRESS");

        // Fisica
        if (Input.GetKeyDown(KeyCode.F7)) AttivaStressFisica(physicsStepObjects, "PHYSICS_STRESS");
        if (Input.GetKeyDown(KeyCode.F8)) AttivaStressFisica(physicsStepObjects * 4, "PHYSICS_STRESS");

        // Memoria (Garbage Collector Stutter)
        if (Input.GetKeyDown(KeyCode.F9)) AttivaStressMemoria(memoryStepMB, "MEMORY_STRESS"); // Alloca 10MB al frame

        // Reset
        if (Input.GetKeyDown(KeyCode.Escape)) FermaTutto();

        if (cpuStress)
        {
            double dummy = 0;
            for (int i = 0; i < intensitaCpu; i++) { dummy += Mathf.Sqrt(i); }
        }

        if (memoryStress)
        {
            // Alloca array enormi ogni frame per forzare il Garbage Collector
            byte[] spazzatura = new byte[1024 * 1024 * mbAllocataFrame];
        }
    }

    void AttivaStressCPU(int intensita, string label)
    {
        FermaTutto();
        intensitaCpu = intensita;
        cpuStress = true;
        SetLabel(label);
        //Debug.Log($"Attivato {label} - Intensità: {intensita}");
    }

    void AttivaStressGPU(int numeroOggetti, string label)
    {
        FermaTutto();
        gpuStress = true;
        numOggettiGpu = numeroOggetti;
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
        physicsStress = true;
        numOggettiFisici = numeroOggetti;
        SetLabel(label);

        PhysicMaterial highFrictionMat = new PhysicMaterial();
        highFrictionMat.bounciness = 1f;
        highFrictionMat.bounceCombine = PhysicMaterialCombine.Maximum;

        for (int i = 0; i < numeroOggetti; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            go.transform.position = new Vector3(Random.Range(-2f, 2f), Random.Range(5f, 25f), Random.Range(-2f, 2f));
            go.transform.localScale = Vector3.one * 0.5f;

            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Forza calcoli fisici estremi

            go.GetComponent<SphereCollider>().sharedMaterial = highFrictionMat;

            istanzeFisica.Add(go);
        }
    }

    void AttivaStressMemoria(int mb, string label)
    {
        FermaTutto();
        memoryStress = true;
        mbAllocataFrame = mb;
        SetLabel(label);
    }

    void SetLabel(string label)
    {
        if (logger != null) logger.scenarioLabel = label;
        Debug.Log($"Attivato: {label}");
    }

    void FermaTutto()
    {
        cpuStress = false; gpuStress = false; physicsStress = false; memoryStress = false;
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