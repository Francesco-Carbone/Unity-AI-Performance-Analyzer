using UnityEngine;
using TMPro; // Serve per usare i testi a schermo

public class AIAssistant : MonoBehaviour
{
    [Header("Interfaccia Utente")]
    public TextMeshProUGUI testoAssistente; // Testo

    [Header("Soglie di Riferimento (Regole IA)")]
    public float intervalloAnalisi = 1.0f;
    public float sogliaLag_ms = 33.3f; // Circa 30 FPS
    public int sogliaBatchesGPU = 5000;

    private float timer = 0f;

    void Update()
    {
        timer += Time.unscaledDeltaTime;

        // Esegue l'analisi a intervalli regolari per non appesantire il gioco
        if (timer >= intervalloAnalisi)
        {
            AnalizzaPrestazioni();
            timer = 0f;
        }
    }

    void AnalizzaPrestazioni()
    {
        // 1. Legge i dati attuali
        float frameTimeCorrente = Time.unscaledDeltaTime * 1000f;

        int batchesCorrenti = UnityEditor.UnityStats.batches;

        // 2. L'Albero Decisionale dell'IA (Le regole che abbiamo scoperto dai grafici)
        if (frameTimeCorrente > sogliaLag_ms)
        {
            // C'è del lag! Scopriamo il colpevole.
            if (batchesCorrenti > sogliaBatchesGPU)
            {
                // Regola 1: Alto FrameTime + Alti Batches = Problema GPU
                MostraMessaggio("AI: Troppi oggetti in scena o luci attive! (Collo di bottiglia GPU)", Color.red);
            }
            else
            {
                // Regola 2: Alto FrameTime + Batches Normali = Problema CPU
                MostraMessaggio("AI: Troppi calcoli fisici o script pesanti! (Collo di bottiglia CPU)", new Color(1f, 0.5f, 0f)); // Arancione
            }
        }
        else
        {
            // Regola 3: FrameTime basso = Tutto ok
            MostraMessaggio("AI: Prestazioni stabili. Tutto ok.", Color.green);
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