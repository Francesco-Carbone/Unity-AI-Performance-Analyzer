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
    public int memoryStepMB = 40;

    [Header("Unity 6 / URP Setup")]
    [Tooltip("Opzionale: Trascina qui un materiale pesante per il test GPU. Se vuoto, ne verrà creato uno URP standard.")]
    public Material materialeGpuStress;

    [Header("Modalità Autopilota")]
    [Tooltip("Attiva l'esecuzione automatica e casuale degli stress test.")]
    public bool usaAutopilota = false;

    [Tooltip("Ogni quanti secondi l'Autopilota scatena uno stress test casuale.")]
    public float intervalloEventi = 20f;

    [Tooltip("Quanto dura lo stress test prima che l'Autopilota lo termini.")]
    public float durataEvento = 7f;

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

    [Header("Configurazione Comandi")]
    [Tooltip("Comandi per attivare stresst test")]
    public KeyCode stressCPU1 = KeyCode.F1;
    public KeyCode stressCPU2 = KeyCode.F2;
    public KeyCode stressCPU3 = KeyCode.F3;
    public KeyCode stressGPU1 = KeyCode.F4;
    public KeyCode stressGPU2 = KeyCode.F5;
    public KeyCode stressGPU3 = KeyCode.F6;
    public KeyCode stressFisica1 = KeyCode.F7;
    public KeyCode stressFisica2 = KeyCode.F8;
    public KeyCode stressMemoria1 = KeyCode.F9;
    public KeyCode reset = KeyCode.Escape;

    private Coroutine routineAutopilota;

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
        GestisciCicloAutopilota();

        // CPU
        if (Input.GetKeyDown(stressCPU1)) AttivaStressCPU(cpuStepPower, "CPU_STRESS");
        if (Input.GetKeyDown(stressCPU2)) AttivaStressCPU(cpuStepPower * 10, "CPU_STRESS");
        if (Input.GetKeyDown(stressCPU3)) AttivaStressCPU(cpuStepPower * 50, "CPU_STRESS");

        // GPU
        if (Input.GetKeyDown(stressGPU1)) AttivaStressGPU(gpuStepObjects, "GPU_STRESS");
        if (Input.GetKeyDown(stressGPU2)) AttivaStressGPU(gpuStepObjects * 6, "GPU_STRESS");
        if (Input.GetKeyDown(stressGPU3)) AttivaStressGPU(gpuStepObjects * 30, "GPU_STRESS");

        // Fisica
        if (Input.GetKeyDown(stressFisica1)) AttivaStressFisica(physicsStepObjects, "PHYSICS_STRESS");
        if (Input.GetKeyDown(stressFisica2)) AttivaStressFisica(physicsStepObjects * 4, "PHYSICS_STRESS");

        // Memoria (Garbage Collector Stutter)
        if (Input.GetKeyDown(stressMemoria1)) AttivaStressMemoria(memoryStepMB, "MEMORY_STRESS"); // Alloca 10MB al frame

        // Reset
        if (Input.GetKeyDown(reset)) FermaTutto();

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

    void GestisciCicloAutopilota()
    {
        if (usaAutopilota && routineAutopilota == null)
        {
            routineAutopilota = StartCoroutine(LoopAutopilota());
        }
        else if (!usaAutopilota && routineAutopilota != null)
        {
            StopCoroutine(routineAutopilota);
            routineAutopilota = null;
            FermaTutto();
            Debug.Log("Autopilota Disattivato.");
        }
    }

    IEnumerator LoopAutopilota()
    {
        Debug.Log("Autopilota Attivo.");

        while (usaAutopilota)
        {
            // Attende il tempo prestabilito tra un attacco e l'altro
            yield return new WaitForSeconds(intervalloEventi);

            if (!usaAutopilota) break;

            // Sceglie un'emergenza a caso tra 9 combinazioni possibili
            int tipoStressCasuale = Random.Range(0, 9);

            switch (tipoStressCasuale)
            {
                case 0: AttivaStressCPU(cpuStepPower, "CPU_STRESS"); break;
                case 1: AttivaStressCPU(cpuStepPower * 10, "CPU_STRESS"); break;
                case 2: AttivaStressCPU(cpuStepPower * 50, "CPU_STRESS"); break;

                case 3: AttivaStressGPU(gpuStepObjects, "GPU_STRESS"); break;
                case 4: AttivaStressGPU(gpuStepObjects * 6, "GPU_STRESS"); break;
                case 5: AttivaStressGPU(gpuStepObjects * 30, "GPU_STRESS"); break;

                case 6: AttivaStressFisica(physicsStepObjects, "PHYSICS_STRESS"); break;
                case 7: AttivaStressFisica(physicsStepObjects * 4, "PHYSICS_STRESS"); break;

                case 8: AttivaStressMemoria(memoryStepMB, "MEMORY_STRESS"); break;
            }

            yield return new WaitForSeconds(durataEvento);

            // Ferma l'evento
            FermaTutto();
            Debug.Log("Autopilota: Evento terminato. Fase di tregua...");
        }
    }

    void AttivaStressCPU(int intensita, string label)
    {
        FermaTutto();
        intensitaCpu = intensita;
        cpuStress = true;
        SetLabel(label);
    }

    void AttivaStressGPU(int numeroOggetti, string label)
    {
        FermaTutto();
        gpuStress = true;
        numOggettiGpu = numeroOggetti;
        SetLabel(label);

        // Lancia la creazione scaglionata per evitare il blocco della CPU
        StartCoroutine(GeneraSfereGradualmente(numeroOggetti));
    }

    private IEnumerator GeneraSfereGradualmente(int numeroOggetti)
    {
        Material matBase = materialeGpuStress;
        if (matBase == null)
        {
            Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
            matBase = new Material(urpShader != null ? urpShader : Shader.Find("Standard"));
            matBase.color = new Color(1f, 0f, 0f, 0.5f); // Rosso semi-trasparente di default
        }

        Transform camTransform = Camera.main != null ? Camera.main.transform : transform;
        int sferePerFrame = 150; // Quante sfere creare per frame senza bloccare la CPU

        for (int i = 0; i < numeroOggetti; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // Genera esattamente al centro dell'inquadratura, ammassate per l'Overdraw
            Vector3 offsetRandom = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(1.5f, 3.5f));
            go.transform.position = camTransform.position + camTransform.TransformDirection(offsetRandom);

            Destroy(go.GetComponent<SphereCollider>());
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();

            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;

            Material uniqueMat = new Material(matBase);
            uniqueMat.renderQueue = matBase.renderQueue + (i % 2000);
            renderer.material = uniqueMat;

            istanzeGPU.Add(go);

            // Ogni volta che arriva a 150 sfere, aspetta il frame successivo
            if (i % sferePerFrame == 0 && i > 0)
            {
                yield return null;
            }
        }
        Debug.Log($"<color=red>Stress GPU completato: {numeroOggetti} sfere caricate!</color>");
    }

    void AttivaStressFisica(int numeroOggetti, string label)
    {
        FermaTutto();
        physicsStress = true;
        numOggettiFisici = numeroOggetti;
        SetLabel(label);

        PhysicsMaterial highFrictionMat = new PhysicsMaterial();
        highFrictionMat.bounciness = 1f;
        highFrictionMat.bounceCombine = PhysicsMaterialCombine.Maximum;

        for (int i = 0; i < numeroOggetti; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            go.transform.position = new Vector3(Random.Range(-2f, 2f), Random.Range(5f, 25f), Random.Range(-2f, 2f));
            go.transform.localScale = Vector3.one * 0.5f;

            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Forza calcoli fisici

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