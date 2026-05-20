using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class HeatmapViewer : MonoBehaviour
{
    [Header("Configurazione")]
    public string nomeFile = "PerformanceHeatmap.csv";
    public float raggioSfera = 0.5f;
    public bool mostraSempre = true;

    [Header("Filtri Categoria (Toggles)")]
    public bool mostraCPU = true;
    public bool mostraGPU = true;
    public bool mostraFisica = true;
    public bool mostraMemoria = true;
    public bool mostraSconosciuti = true;

    [Header("Filtro Gravità")]
    [Tooltip("Mostra solo i problemi con un ratio di stress superiore a questo valore (es. 1.5)")]
    public float gravitaMinima = 0f;

    private struct DatiPunto
    {
        public Vector3 posizione;
        public string tipoProblema;
        public float gravita;
    }

    private List<DatiPunto> puntiDaDisegnare = new List<DatiPunto>();

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