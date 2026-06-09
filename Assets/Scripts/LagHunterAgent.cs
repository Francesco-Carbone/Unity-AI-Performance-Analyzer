using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

public class LagHunterAgent : Agent
{
    [Header("Connessioni Sistemi")]
    public AIAssistant aiAssistant;
    private CharacterController characterController;
    private PlayerCharacterController playerCharacterController;

    [Header("Parametri Movimento Bot")]
    public float velocitaMovimento = 8f;
    public float velocitaRotazione = 150f;

    [Header("Limiti Episodio")]
    [Tooltip("Numero massimo di passi prima che il round si resetti (es. 3000 passi = circa 1 minuto)")]
    public int passiMassimiPerEpisodio = 7000;

    private Vector3 posizioneIniziale;

    // Sistema Anti-Incastro e Esplorazione
    private HashSet<Vector3Int> celleVisitate = new HashSet<Vector3Int>();
    private float dimensioneCellaGriglia = 1.5f; // Blocchi di 3x3 metri

    public override void Initialize()
    {
        characterController = GetComponent<CharacterController>();
        playerCharacterController = GetComponent<PlayerCharacterController>();
        posizioneIniziale = transform.position;

        MaxStep = passiMassimiPerEpisodio;

        if (aiAssistant == null)
        {
            aiAssistant = FindObjectOfType<AIAssistant>();
        }
    }

    // Viene chiamato all'inizio di ogni "round" di addestramento (Reset)
    public override void OnEpisodeBegin()
    {
        // Resetta lo stato di vita e cura il giocatore prima di muoverlo
        if (playerCharacterController != null)
        {
            playerCharacterController.ResuscitaECura();
        }

        // Resetta il bot alla posizione iniziale
        characterController.enabled = false;
        transform.position = posizioneIniziale;
        transform.rotation = Quaternion.identity;
        characterController.enabled = true;

        // Pulisce la memoria dei posti già visitati per il nuovo round
        celleVisitate.Clear();
    }

    void FixedUpdate()
    {
        // Controllo morte
        if (playerCharacterController != null && playerCharacterController.isDead)
        {
            AddReward(-3.0f);
            EndEpisode(); // Forza il reset immediato dell'episodio
            return;
        }

        // Controllo caduta fuori dalla mappa
        if (transform.position.y < -20f)
        {
            AddReward(-2.0f);
            EndEpisode(); // Forza il reset immediato
            return;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // La velocità attuale del bot nello spazio
        sensor.AddObservation(characterController.velocity);

        // Il moltiplicatore di lag attuale fornito dall'AIAssistant
        float lagAttuale = (aiAssistant != null) ? aiAssistant.ratio_FrameTime : 1.0f;
        sensor.AddObservation(lagAttuale);
    }

    // Cosa fa l'IA e come viene ricompensata
    public override void OnActionReceived(float[] vectorAction)
    {
        if (playerCharacterController != null && playerCharacterController.isDead)
        {
            return;
        }

        // Riceve i comandi continui da Python (valori da -1 a +1)
        float inputMovimentoAvanti = vectorAction[0]; // Avanti / Indietro
        float inputRotazione = vectorAction[1];       // Ruota la visuale

        transform.Rotate(Vector3.up * inputRotazione * velocitaRotazione * Time.fixedDeltaTime);
        Vector3 direzione = transform.forward * inputMovimentoAvanti;

        // Applica la gravità base
        direzione.y = Physics.gravity.y * Time.fixedDeltaTime;

        characterController.Move(direzione * velocitaMovimento * Time.fixedDeltaTime);

        // Algoritmo di apprendimento
        float lagAttuale = (aiAssistant != null) ? aiAssistant.ratio_FrameTime : 1.0f;

        // Obiettivo principale
        if (lagAttuale > 1.3f) // Se supera la soglia di allarme
        {
            AddReward(lagAttuale * 0.1f);
        }

        // Incentivo all'Esplorazione
        Vector3Int cellaAttuale = new Vector3Int(
            Mathf.FloorToInt(transform.position.x / dimensioneCellaGriglia),
            Mathf.FloorToInt(transform.position.y / dimensioneCellaGriglia),
            Mathf.FloorToInt(transform.position.z / dimensioneCellaGriglia)
        );

        if (!celleVisitate.Contains(cellaAttuale))
        {
            celleVisitate.Add(cellaAttuale);
            AddReward(2.0f);
        }

        // Punizione temporale evita che l'IA rimanga ferma a fare nulla
        AddReward(-0.005f);
    }

    // Permette di guidare il bot manualmente con le frecce della tastiera per test
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Vertical");
        actionsOut[1] = Input.GetAxis("Horizontal");
    }
}