import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.tree import DecisionTreeClassifier
from sklearn.metrics import classification_report
import m2cgen as m2c
import os

# Configurazione Percorsi
cartella_training = os.path.dirname(os.path.abspath(__file__))
percorso_csv = os.path.join(cartella_training, "PerformanceData.csv")

percorso_csharp_folder = os.path.join(cartella_training, "..", "Assets", "Scripts")
percorso_csharp_file_rf = os.path.join(percorso_csharp_folder, "AI_Brain.cs")
percorso_csharp_file_tree = os.path.join(percorso_csharp_folder, "AI_Brain_Fast.cs")

if not os.path.exists(percorso_csharp_folder):
    os.makedirs(percorso_csharp_folder)
    print(f"Creata cartella: {percorso_csharp_folder}")

# Caricamento con controllo errore
try:
    df = pd.read_csv(percorso_csv)
    print(f"File caricato con successo da: {percorso_csv}")
except FileNotFoundError:
    print(f"Errore: file non trovato in {cartella_training}")
    exit()

df.columns = df.columns.str.strip()
print("Classi trovate nel file:", df['Label'].unique())

# 2. Calcolo Baseline (Calibrazione)
calibration_data = df[df['Label'] == 'CALIBRATION']
if calibration_data.empty:
    print("Errore: Fase CALIBRATION non trovata!")
    exit()

base_ft = calibration_data['FrameTime_ms'].median()
base_batches = calibration_data['Batches_DrawCalls'].median()
base_cpu = calibration_data['MainThreadTime_ms'].median()
base_mem = calibration_data['Memory_MB'].median()

if base_batches == 0: base_batches = 1
if base_cpu == 0: base_cpu = 1

# Trasformazione in Delta e filtraggio
df_train = df[df['Label'] != 'CALIBRATION'].copy()
df_train['Delta_FT'] = df_train['FrameTime_ms'] / base_ft
df_train['Delta_Batches'] = df_train['Batches_DrawCalls'] / base_batches
df_train['Delta_CPU'] = df_train['MainThreadTime_ms'] / base_cpu
df_train['Delta_Mem'] = df_train['Memory_MB'] / base_mem

# Selezione Feature e Target
X = df_train[['Delta_FT', 'Delta_Batches', 'Delta_CPU', 'Delta_Mem']]
y = df_train['Label']

# Divisione in training e test set (80% training, 20% test)
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Addestramento
print("\n--- Addestramento Modello 1: Random Forest ---")
model_rf = RandomForestClassifier(n_estimators=30, max_depth=5, random_state=42)
model_rf.fit(X_train, y_train)

# Verifica come si comporta sui dati che non ha mai visto
y_pred_rf = model_rf.predict(X_test)
print("\n--- Report di Validazione ---")
print(classification_report(y_test, y_pred_rf))

# Esportazione per Unity
print("\nGenerazione AI_Brain.cs (Random Forest)...")
codice_csharp_rf = m2c.export_to_c_sharp(model_rf, class_name="AI_Brain")

with open(percorso_csharp_file_rf, "w") as f:
    f.write(codice_csharp_rf)

# Seconda versione addestramento
print("\n--- Addestramento Modello 2: Singolo Albero ---")
model_tree = DecisionTreeClassifier(max_depth=5, random_state=42)
model_tree.fit(X_train, y_train)

y_pred_tree = model_tree.predict(X_test)
print(classification_report(y_test, y_pred_tree))

print("Generazione AI_Brain_Fast.cs (Decision Tree)...")
codice_csharp_tree = m2c.export_to_c_sharp(model_tree, class_name="AI_Brain_Fast")

with open(percorso_csharp_file_tree, "w") as f:
    f.write(codice_csharp_tree)

print(f"Script C# generati in: {percorso_csharp_file_rf} e {percorso_csharp_file_tree}")