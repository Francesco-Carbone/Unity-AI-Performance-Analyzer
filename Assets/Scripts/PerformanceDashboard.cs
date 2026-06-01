using UnityEngine;
using UnityEngine.UI;

public class PerformanceDashboard : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public GameObject pannelloDashboard;
    public Text testoFPS;
    public Text testoRAM;
    public Text testoStatoIA;
    public Text testoRatiosTelemetria;

    [Header("Connessione Sistemi")]
    public AIAssistant aiAssistant;

    void Start()
    {
        if (pannelloDashboard != null) pannelloDashboard.SetActive(false);
    }

    void Update()
    {
        // Attivazione/Disattivazione
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (pannelloDashboard != null) pannelloDashboard.SetActive(!pannelloDashboard.activeSelf);
        }

        if (pannelloDashboard == null || !pannelloDashboard.activeSelf || aiAssistant == null) return;

        float fps = aiAssistant.rawFPS;
        testoFPS.text = string.Format("FPS: {0:0.}", fps);

        if (fps >= 60) testoFPS.color = Color.green;
        else if (fps >= 30) testoFPS.color = Color.yellow;
        else testoFPS.color = Color.red;

        testoRAM.text = string.Format("RAM (Script): {0:0.} MB\nVRAM (GPU): {1:0.} MB",
                                   aiAssistant.rawRAM_MB,
                                   aiAssistant.rawVRAM_MB);

        string modello = aiAssistant.modelloDaUsare == AIAssistant.TipoIA.Random_Forest_Precisa ? "Random Forest" : "Decision Tree";
        string risoluzioneScalata = string.Format("{0:0}%", aiAssistant.OttieniRisoluzioneAttuale * 100f);

        testoStatoIA.text = $"Modello: {modello}\n" +
                            $"Diagnosi: {aiAssistant.statoAttuale}\n" +
                            $"Risoluzione DRS: {risoluzioneScalata}";

        // Colora il testo dello stato in base alla gravità
        if (aiAssistant.statoAttuale == "Performance OK") testoStatoIA.color = Color.green;
        else if (aiAssistant.statoAttuale == "Stress CPU") testoStatoIA.color = new Color(1f, 0.5f, 0f);
        else if (aiAssistant.statoAttuale == "Stress GPU") testoStatoIA.color = Color.red;
        else if (aiAssistant.statoAttuale == "Stress Fisica") testoStatoIA.color = Color.cyan;
        else if (aiAssistant.statoAttuale == "Memory Leak") testoStatoIA.color = Color.magenta;
        else if (aiAssistant.statoAttuale == "Riscaldamento") testoStatoIA.color = Color.yellow;
        else if (aiAssistant.statoAttuale == "Calibrazione in corso") testoStatoIA.color = Color.white;
        else if (aiAssistant.statoAttuale == "Sistema Pronto") testoStatoIA.color = Color.green;

        // Mostra i ratio di telemetria
        testoRatiosTelemetria.text = string.Format(
            "--- Moltiplicatori Lag ---\n" +
            "Ratio FrameTime: {0:F2}x\n" +
            "Ratio DrawCalls: {1:F2}x\n" +
            "Ratio CPU Time : {2:F2}x\n" +
            "Ratio Memory   : {3:F2}x\n" +
            "Soglia Allarme : {4:F2}x",
            aiAssistant.ratio_FrameTime,
            aiAssistant.ratio_Batches,
            aiAssistant.ratio_CPU,
            aiAssistant.ratio_Memory,
            aiAssistant.sogliaAllarme
        );
    }
}