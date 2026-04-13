import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import classification_report
import m2cgen as m2c
import os

# Configurazione Percorsi
cartella_training = os.path.dirname(os.path.abspath(__file__))
percorso_csv = os.path.join(cartella_training, "PerformanceData.csv")

percorso_csharp_folder = os.path.join(cartella_training, "..", "Assets", "Scripts")
percorso_csharp_file = os.path.join(percorso_csharp_folder, "AI_Brain.cs")

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
if base_batches == 0: base_batches = 1

# 3. Trasformazione in Delta e filtraggio
df_train = df[df['Label'] != 'CALIBRATION'].copy()
df_train['Delta_FT'] = df_train['FrameTime_ms'] / base_ft
df_train['Delta_Batches'] = df_train['Batches_DrawCalls'] / base_batches

# 4. Selezione Feature e Target
X = df_train[['Delta_FT', 'Delta_Batches']]
y = df_train['Label']

# 5. Divisione in training e test set (80% training, 20% test)
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# 6. Addestramento
model = RandomForestClassifier(n_estimators=100, max_depth=5, random_state=42)
model.fit(X_train, y_train)

# 7. Verifica come si comporta sui dati che non ha mai visto
y_pred = model.predict(X_test)
print("\n--- Report di Validazione ---")
print(classification_report(y_test, y_pred))

# 8. Esportazione per Unity
print("\nGenerazione AI_Brain.cs...")
codice_csharp = m2c.export_to_c_sharp(model, class_name="AI_Brain")

with open(percorso_csharp_file, "w") as f:
    f.write(codice_csharp)

print(f"Script C# generato in: {percorso_csharp_file}")