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

    private struct DatiPunto
    {
        public Vector3 posizione;
        public string tipoProblema;
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
            // Assegna il colore in base al tipo di problema
            if (punto.tipoProblema.Contains("GPU")) Gizmos.color = Color.red;
            else if (punto.tipoProblema.Contains("CPU")) Gizmos.color = new Color(1f, 0.5f, 0f); // Arancione
            else if (punto.tipoProblema.Contains("PHYSICS")) Gizmos.color = Color.cyan;
            else if (punto.tipoProblema.Contains("MEMORY")) Gizmos.color = Color.magenta;
            else Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(punto.posizione, raggioSfera);
        }
    }
}