using UnityEngine;

public class LagSource : MonoBehaviour
{
    [Header("Impostazioni Area di Lag")]
    [Tooltip("Il raggio (in metri) entro cui l'agente subirà il calo di frame")]
    public float raggioAttivazione = 8f;

    [Tooltip("Quanti calcoli matematici eseguire ad ogni frame.")]
    public int intensitaCarico = 200000;

    [Header("Riferimenti")]
    [Tooltip("L'agente da tracciare")]
    public Transform agente;

    [Header("Visualizzazione Editor")]
    public Color coloreArea = new Color(1f, 0f, 0f, 0.2f);

    private void Start()
    {
        // Se non ha assegnato l'agente nell'inspector, lo cerca automaticamente
        if (agente == null)
        {
            LagHunterAgent agentScript = Object.FindAnyObjectByType<LagHunterAgent>();
            if (agentScript != null)
            {
                agente = agentScript.transform;
            }
        }
    }

    private void Update()
    {
        if (agente == null) return;

        // Calcola la distanza tra questa sorgente e l'agente
        float distanza = Vector3.Distance(transform.position, agente.position);

        // Se l'agente entra nel raggio d'azione inizia a stressare la CPU
        if (distanza <= raggioAttivazione)
        {
            GeneraCaricoCPU();
        }
    }

    private void GeneraCaricoCPU()
    {
        // Un ciclo for intensivo costringe il thread principale di Unity a rallentare,
        // simulando un collo di bottiglia reale
        double calcoloInutile = 0;
        for (int i = 0; i < intensitaCarico; i++)
        {
            calcoloInutile += Mathf.Sin(i) * Mathf.Cos(i);
        }
    }

    // Disegna una sfera colorata nell'editor di Unity per identificare la zona
    private void OnDrawGizmos()
    {
        Gizmos.color = coloreArea;
        Gizmos.DrawSphere(transform.position, raggioAttivazione);
    }
}