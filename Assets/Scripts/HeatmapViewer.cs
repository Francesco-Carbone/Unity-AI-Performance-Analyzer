using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class HeatmapViewer : MonoBehaviour
{
    [Header("Configurazione")]
    public string nomeFile = "PerformanceHeatmap.csv";
    public float raggioSfera = 0.5f;
    [Tooltip("Se TRUE, mostra i Gizmos nell'Editor anche a heatmap spenta. Consigliato: FALSE.")]
    public bool mostraSempre = false;

    [Header("Unity 6 / URP Setup")]
    [Tooltip("Crea un materiale trasparente nel Project (URP Lit o Unlit) e trascinalo qui.")]
    public Material materialePersonalizzato;

    [Header("Filtri Categoria (Toggles)")]
    public bool mostraCPU = true;
    public bool mostraGPU = true;
    public bool mostraFisica = true;
    public bool mostraMemoria = true;
    public bool mostraSconosciuti = true;

    [Header("Filtro Gravità")]
    [Tooltip("Mostra solo i problemi con un ratio di stress superiore a questo valore (es. 1.5)")]
    public float gravitaMinima = 0f;

    [Header("Visualizzazione In-Game")]
    [Tooltip("Tasto per accendere/spegnere gli ologrammi della heatmap durante il gioco")]
    public KeyCode toggleInGame = KeyCode.M;
    private bool inGameAttiva = false;
    private GameObject contenitoreInGame; // Raggruppa tutte le sfere

    private struct DatiPunto
    {
        public Vector3 posizione;
        public string tipoProblema;
        public float gravita;
    }

    private List<DatiPunto> puntiDaDisegnare = new List<DatiPunto>();
    private MaterialPropertyBlock propBlock;

    void Update()
    {
        if (Input.GetKeyDown(toggleInGame))
        {
            inGameAttiva = !inGameAttiva;

            if (inGameAttiva)
            {
                MostraHeatmapInGame();
            }
            else
            {
                NascondiHeatmapInGame();
            }
        }
    }

    [ContextMenu("Carica Dati Heatmap")]
    public void CaricaDati()
    {
        puntiDaDisegnare.Clear();
        string rootPath = Directory.GetParent(Application.dataPath).FullName;
        string logPath = Path.Combine(rootPath, nomeFile);

        if (!File.Exists(logPath))
        {
            Debug.LogWarning("File Heatmap non trovato!");
            return;
        }

        string[] linee = File.ReadAllLines(logPath);

        for (int i = 1; i < linee.Length; i++)
        {
            string[] colonne = linee[i].Split(',');
            if (colonne.Length < 6) continue;

            try
            {
                DatiPunto punto = new DatiPunto();
                float x = float.Parse(colonne[1], CultureInfo.InvariantCulture);
                float y = float.Parse(colonne[2], CultureInfo.InvariantCulture);
                float z = float.Parse(colonne[3], CultureInfo.InvariantCulture);

                punto.posizione = new Vector3(x, y, z);
                punto.tipoProblema = colonne[4];

                punto.gravita = float.Parse(colonne[5], CultureInfo.InvariantCulture);

                puntiDaDisegnare.Add(punto);
            }
            catch { 
            }
        }
        Debug.Log($"Heatmap caricata: {puntiDaDisegnare.Count} punti rilevati.");
    }

    public void MostraHeatmapInGame()
    {
        NascondiHeatmapInGame(); // Pulisce la scena
        CaricaDati();

        if (puntiDaDisegnare.Count == 0) return;

        // Crea una cartella vuota nella scena per raggruppare i punti
        contenitoreInGame = new GameObject("Heatmap_InGame_Container");

        // Crea un materiale base (Trasparente e senza ombre)
        Material matBase = materialePersonalizzato;
        if (matBase == null)
        {
            Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
            if (urpShader == null) urpShader = Shader.Find("Standard"); // Fallback se non si usa URP

            matBase = new Material(urpShader);

            if (urpShader.name.Contains("Universal Render Pipeline"))
            {
                matBase.SetFloat("_Surface", 1); // 1 = Transparent
                matBase.SetFloat("_Blend", 0);   // 0 = Alpha Blend
                matBase.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                matBase.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                matBase.SetInt("_ZWrite", 0);
                matBase.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                matBase.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                matBase.SetFloat("_Mode", 3); // Fallback standard
                matBase.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                matBase.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                matBase.SetInt("_ZWrite", 0);
                matBase.renderQueue = 3000;
            }
        }

        // Inizializza il blocco proprietà ed identifica l'ID del canale colore
        if (propBlock == null) propBlock = new MaterialPropertyBlock();
        int colorPropertyID = matBase.HasProperty("_BaseColor") ? Shader.PropertyToID("_BaseColor") : Shader.PropertyToID("_Color");

        foreach (var punto in puntiDaDisegnare)
        {
            if (punto.gravita < gravitaMinima) continue;

            Color colorePunto = Color.white;
            if (punto.tipoProblema.Contains("GPU")) { if (!mostraGPU) continue; colorePunto = Color.red; }
            else if (punto.tipoProblema.Contains("CPU")) { if (!mostraCPU) continue; colorePunto = new Color(1f, 0.5f, 0f); }
            else if (punto.tipoProblema.Contains("PHYSICS")) { if (!mostraFisica) continue; colorePunto = Color.cyan; }
            else if (punto.tipoProblema.Contains("MEMORY")) { if (!mostraMemoria) continue; colorePunto = Color.magenta; }
            else { if (!mostraSconosciuti) continue; colorePunto = Color.yellow; }

            // Rende il colore semi-trasparente (Alpha al 60%)
            colorePunto.a = 0.6f;

            // Spawna la sfera
            GameObject sfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sfera.transform.SetParent(contenitoreInGame.transform);
            sfera.transform.position = punto.posizione;

            sfera.transform.localScale = Vector3.one * (raggioSfera * 2f);

            // Distrugge il collider
            Destroy(sfera.GetComponent<Collider>());

            // Disattiva le ombre sulla singola sfera
            MeshRenderer renderer = sfera.GetComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sharedMaterial = matBase;

            renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(colorPropertyID, colorePunto);
            renderer.SetPropertyBlock(propBlock);
        }

        Debug.Log("<color=cyan>[Heatmap]</color> Visualizzatore In-Game ATTIVATO.");
    }

    public void NascondiHeatmapInGame()
    {
        if (contenitoreInGame != null)
        {
            Destroy(contenitoreInGame);
            contenitoreInGame = null;
        }

        GameObject residuo = GameObject.Find("Heatmap_InGame_Container");
        if (residuo != null)
        {
            DestroyImmediate(residuo);
        }
        puntiDaDisegnare.Clear();

        Debug.Log("<color=cyan>[Heatmap]</color> Visualizzatore In-Game DISATTIVATO.");
    }

    private void OnDrawGizmos()
    {
        if (!mostraSempre) return;
        DisegnaMappa();
    }

    private void OnDrawGizmosSelected()
    {
        if (mostraSempre) return;
        DisegnaMappa();
    }

    void DisegnaMappa()
    {
        if (puntiDaDisegnare.Count == 0) return;

        foreach (var punto in puntiDaDisegnare)
        {
            // Se lo stress registrato è minore della soglia impostata, salta il disegno
            if (punto.gravita < gravitaMinima) continue;

            // Applicazione filtri categoria e colore
            if (punto.tipoProblema.Contains("GPU"))
            {
                if (!mostraGPU) continue;
                Gizmos.color = Color.red;
            }
            else if (punto.tipoProblema.Contains("CPU"))
            {
                if (!mostraCPU) continue;
                Gizmos.color = new Color(1f, 0.5f, 0f);
            }
            else if (punto.tipoProblema.Contains("PHYSICS"))
            {
                if (!mostraFisica) continue;
                Gizmos.color = Color.cyan;
            }
            else if (punto.tipoProblema.Contains("MEMORY"))
            {
                if (!mostraMemoria) continue;
                Gizmos.color = Color.magenta;
            }
            else
            {
                if (!mostraSconosciuti) continue;
                Gizmos.color = Color.yellow;
            }

            Gizmos.DrawSphere(punto.posizione, raggioSfera);
        }
    }
}