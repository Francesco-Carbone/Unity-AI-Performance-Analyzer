# Unity AI Performance Analyzer - Legacy System

[![Unity Version](https://img.shields.io/badge/Unity-2019.4.1f1-lightgrey.svg?logo=unity)](#)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

Questa versione del progetto è stata specificamente calibrata e ottimizzata per **Unity 2019.4.1f1 LTS**. È pensata per gli sviluppatori che lavorano su progetti meno recenti e necessitano di uno strumento di profilazione IA leggero, senza dipendenze da pacchetti moderni.

---

## Differenze rispetto al Branch Principale

Se provieni dal branch `main`, ecco le differenze fondamentali di questa versione:

1. **Assenza del LagHunter (No ML-Agents):** In questo branch **non è presente** il modello a reti neurali per l'esplorazione autonoma. Questo elimina la dipendenza dal pacchetto `com.unity.ml-agents`, rendendo il tool molto più rapido da importare in progetti vecchi e riducendo il peso complessivo.
2. **Calibrazione 2019.4:** I modelli ad albero decisionale (`AI_Brain.cs` e `AI_Brain_Fast.cs`) sono stati addestrati su dataset raccolti in Unity 2019.4, tenendo conto delle pipeline di rendering standard dell'epoca (pre-URP maturo) e delle metriche hardware tipiche di quella versione del motore.

---

## Funzionalità Incluse

Anche in questa versione più snella, il sistema offre strumenti potenti per scovare i colli di bottiglia (CPU, GPU, Fisica, Memory Leak):

* **Dashboard Telemetrica IA:** Interfaccia real-time per monitorare FPS, RAM/VRAM e la classificazione in tempo reale del collo di bottiglia calcolata dal modello di Machine Learning hardcoded.

* **Heatmap Spaziale:** Registrazione automatica delle coordinate dei cali di frame in un file `.csv` e generazione visiva dei punti critici tramite Gizmos nell'editor (`PerformanceViewer`).

* **Stress Tester:** Generatore di carico artificiale (script `StressTester.cs`) con modalità **Autopilota**, utile per testare la stabilità del proprio gioco sotto sforzi computazionali estremi o per rigenerare un dataset di addestramento.

---

## Setup Veloce

L'installazione in questo branch è immediata, poiché non richiede pacchetti esterni:

1. Trascina le cartelle `Scripts` e `Prefabs` nella directory `Assets` del tuo progetto Unity 2019.
2. Posiziona i prefab `AIManager`, `AILogger` e `StressTest Manager` nella tua scena principale.
3. Avvia il gioco e osserva la Dashboard per analizzare le performance in tempo reale.

*(Nota: Per aggiornare la logica predittiva con il tuo hardware, puoi usare la modalità autopilota dello Stress Tester per generare un nuovo `PerformanceData.csv` e usare gli script Python inclusi per addestrare un nuovo albero decisionale).*
