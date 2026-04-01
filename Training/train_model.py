import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import classification_report
import joblib
import os
import m2cgen as m2c

# Configurazione Percorsi
cartella_training = os.path.dirname(os.path.abspath(__file__))
percorso_csv = os.path.join(cartella_training, "PerformanceData.csv")

percorso_csharp_folder = os.path.join(cartella_training, "..", "Assets", "Scripts")
percorso_csharp_file = os.path.join(percorso_csharp_folder, "AI_Brain.cs")

if not os.path.exists(percorso_csharp_folder):
    os.makedirs(percorso_csharp_folder)
    print(f"Creata cartella: {percorso_csharp_folder}")

# Logica

# Caricamento con controllo errore
try:
    df = pd.read_csv(percorso_csv)
    print(f"File caricato con successo da: {percorso_csv}")
except FileNotFoundError:
    print(f"Errore: file non trovato in {cartella_training}")
    exit()

df.columns = df.columns.str.strip()
print("Classi trovate nel file:", df['Label'].unique())

# Selezione Feature (Input) e Target (Output)
X = df[['FrameTime_ms', 'Batches_DrawCalls']]
y = df['Label']

# Split Training e Test set
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

model = RandomForestClassifier(n_estimators=100)
model.fit(X_train, y_train)

# Verifica accuratezza
y_pred = model.predict(X_test)
print(classification_report(y_test, y_pred))

# Salva il modello Python
joblib.dump(model, os.path.join(cartella_training, 'ai_performance_model.pkl'))
print(f"Modello .pkl salvato in {cartella_training}")

# Esportazione C#
print("Conversione IA in C#...")
codice_csharp = m2c.export_to_c_sharp(model, class_name="AI_Brain")

with open(percorso_csharp_file, "w") as f:
    f.write(codice_csharp)

print(f"Script C# generato in: {percorso_csharp_file}")