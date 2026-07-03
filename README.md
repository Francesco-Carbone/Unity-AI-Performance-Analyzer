# Unity AI Performance Analyzer

[![Unity Version](https://img.shields.io/badge/Unity-6000.4.11f1-black.svg?logo=unity)](#)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

**Unity AI Performance Analyzer** è uno strumento di profilazione, stress testing e analisi predittiva progettato per l'ambiente di sviluppo **Unity 6 (6000.4.11f1)**. 

Il sistema permette agli sviluppatori di individuare in modo completamente automatico l'origine dei colli di bottiglia all'interno dei loro progetti tramite l'uso di modelli basati su Random_Tree_Forest, classificando i cali prestazionali in quattro macro-categorie critiche: **CPU**, **GPU**, **Fisica** e **Memory Leak**.

---

## Funzionalità Core

* **`Dashboard in Real-Time:`** Un'interfaccia UI integrata per monitorare l'andamento dei frame, l'allocazione della memoria (RAM/VRAM) e il responso predittivo dell'IA durante le sessioni di gioco.

* **`Heatmap & Screenshot System:`** Genera mappe di calore tridimensionali visualizzabili sia in Play Mode che direttamente nell'Editor di Unity per tracciare le coordinate spaziali dei rallentamenti. Cattura automaticamente screenshot nei momenti di picco critico per un debug visivo immediato.

* **`Stress Tester con Autopilota:`** Un modulo in grado di indurre artificialmente carichi computazionali pesanti su CPU, GPU, fisica e memoria, sia manualmente che tramite un algoritmo ad autopilota per simulare scenari caotici.

* **`LagHunter (Agente Neurale):`** Sfrutta la libreria **Unity ML-Agents** per addestrare un agente neurale autonomo che esplora lo spazio di gioco comportandosi come un vero player, con l'obiettivo specifico di scovare i punti di stress ambientale naturale.

---

## Architettura del Progetto e Componenti

Il toolkit è suddiviso in moduli logici altamente disaccoppiati:

### 1. Moduli IA e Inferenza (`Assets/Scripts`)
* **`AIAssistant.cs`**: Il manager centrale del sistema. Coordina la telemetria, gestisce i cooldown di analisi e implementa funzioni opzionali di correzione automatica (come la scalabilità dinamica della risoluzione o la mitigazione della fisica).
* **`AI_Brain.cs`**: Trasformazione matematica hardcoded di un modello *Random Forest* (addestrato in Python tramite `scikit-learn`), ottimizzato per l'inferenza real-time a bassissimo costo computazionale senza dipendenze esterne.
* **`AI_Brain_Fast.cs`**: Variante ultra-leggera basata su un singolo albero decisionale (*Decision Tree*) per scenari in cui la frequenza di campionamento dell'inferenza deve essere massima.
* **`LagHunterAgent.cs`**: Estensione della classe `Agent` di ML-Agents. Gestisce gli input dell'agente neurale, calcola le ricompense in base al framerate riscontrato e incentiva l'esplorazione spaziale tramite una griglia di celle visitate.

### 2. Strumenti di Profiling e Visualizzazione
* **`PerformanceLogger.cs`**: Campiona ciclicamente i dati hardware di Unity (FrameTime, CPU Main Thread, GPU Frame Time, oggetti fisici attivi, RAM e VRAM) e genera il file strutturato `PerformanceData.csv` contrassegnando i campioni con etichette di scenario.
* **`PerformanceHeatmap.cs`**: Registra le coordinate spaziali ($X, Y, Z$) dei cali di performance nel file `PerformanceHeatmap.csv` e coordina il salvataggio dei file `.png` nella cartella degli screenshot.
* **`HeatmapViewer.cs`**: Script operante in modalità Editor/Gizmos. Permette di filtrare visivamente i punti critici sulla mappa in base alla gravità e alla categoria del problema (es. mostra solo lag da Fisica).
* **`PerformanceDashboard.cs`**: Gestisce l'aggiornamento dinamico delle stringhe di testo e dei colori di allarme dell'interfaccia utente a schermo.

### 3. Moduli di Stress Testing e Utility
* **`StressTester.cs`**: Consente di attivare artificialmente sub-routine di carico (cicli for intensivi per CPU, istanziazione di mesh e shader complessi per GPU, sfere fisiche ad alta frizione con collisioni continue, allocazioni cicliche di array per Memory Leak). Include la modalità **Autopilota** per alternare i test a intervalli regolari.
* **`LagSource.cs`**: Script posizionabile nello spazio per creare zone di lag artificiale a raggio programmabile, utilizzato per validare la capacità di ricerca del *LagHunter*.
* **`PlayerCharacterController.cs`**: Derivato dal pacchetto standard *3D FPS* di Unity 6, è stato modificato rendendo pubblica la gestione della salute (`health`) per consentire al `LagHunterAgent` di resettare lo stato vitale del player durante i cicli di reset dei passaggi di addestramento.

---

## Sistema di Prefab Pronti all'Uso (`Assets/Prefabs`)

Il progetto include un set di Prefab pre-configurati per l'integrazione immediata in qualsiasi scena:

1.  **`AIManager.prefab`**: Contiene il nucleo logico dell'assistente IA e la UI della Dashboard. Da trascinare all'interno della scena principale.
2.  **`AILogger.prefab`**: Gestisce i componenti di logging isolati per evitare conflitti di gerarchia durante il tracciamento del Player o della Main Camera.
3.  **`PerformanceViewer.prefab`**: Componente delegato alla renderizzazione spaziale tridimensionale della mappa di calore in-game.
4.  **`StressTest Manager.prefab`**: Configura l'ambiente dello Stress Tester con i parametri di calcolo e i materiali pre-assegnati per i carichi GPU.
5.  **`ZonaLag_01.prefab`**: Un volume sferico di esempio configurato con `LagSource` per testare la reattività degli agenti o calibrare l'assistente.

---

## Installazione e Setup

1.  Trascina le cartelle `Scripts` e `Prefabs` all'interno della directory `Assets` del tuo progetto Unity.
2.  Installa il pacchetto **ML-Agents** (`com.unity.ml-agents`) tramite l'Unity Package Manager.
3.  Posiziona i prefab `AIManager`, `AILogger` e `StressTest Manager` nella tua scena di test.
4.  Assicurati che il tuo personaggio principale o il bot abbia il Tag `Player` per consentire il tracciamento automatico della posizione.
5.  *(Opzionale)* Scarica il file di configurazione neurale `.onnx` dalla cartella `training` di questa repository e assegnalo al componente `Behavior Parameters` sul prefab dell'agente.

---

## Workflow di Addestramento e Calibrazione Personalizzata

Il modello incluso è pre-addestrato, ma per massimizzare la precisione sul calcolo del collo di bottiglia specifico del proprio hardware o del proprio parco asset, si consiglia di rigenerare il dataset:

1.  Avvia la scena di Unity attivando il toggle `usaAutopilota` sul componente **StressTester**.
2.  Lascia girare il test per alcuni minuti: lo script spingerà il motore grafico e fisico al limite in modo controllato, associando le etichette corrette al log.
3.  Prendi il file `PerformanceData.csv` generato nella directory principale del tuo progetto e spostalo nella cartella Python contenente `train_model.py`.
4.  Esegui lo script di training Python per estrarre le nuove matrici decisionali e aggiornare i coefficienti all'interno delle classi `AI_Brain`.
