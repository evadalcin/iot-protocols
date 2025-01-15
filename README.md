# Corso Protocolli di Comunicazione IoT  
**Autori:** Eva Dal Cin e Giacomo Di Bert

---

## Panoramica

Questo progetto propone un sistema avanzato per il monitoraggio di una casetta dell'acqua, utilizzando una rete IoT basata sul protocollo CoAP per la trasmissione dei dati. 
Il sistema raccoglie informazioni in tempo reale su parametri simulati quali temperatura dell'acqua, livello dell'acqua e stato del filtro. 
Questi dati vengono inviati per l'elaborazione e la memorizzazione in un database time-series (InfluxDB), consentendo analisi dettagliate e gestione remota. 

---

## Architettura

L'architettura è suddivisa in due componenti principali:

1. **Server (Lato Casetta dell'Acqua):**
   - Integra sensori per acquisire parametri ambientali e funzionali.
   - Trasmette i dati raccolti al cloud utilizzando il protocollo CoAP.

2. **Client:**
   - Riceve i dati dal server, li elabora e li inserisce in un database InfluxDB.
---

## Flusso di Lavoro

### 1. Funzionamento del Server (Raccolta Dati e Trasmissione)

- **Inizializzazione:**
  - Avvio dei sensori integrati:
    - **WaterTempSensor:** misura la temperatura dell'acqua.
    - **WaterLevelSensor:** rileva il livello dell'acqua nel serbatoio.
    - **FilterSensor:** monitora lo stato operativo del filtro.

- **Raccolta e Trasmissione Dati:**
  - I sensori simulano i dati.
  - La trasmissione avviene tramite pacchetti CoAP inviati a intervalli regolari (10 secondi).

### 2. Funzionamento del Client (Elaborazione e Memorizzazione Dati)

- **Inizializzazione:**
  - Dopo che il Server avvia il listener CoAP e predispone endpoint dedicati per ogni sensore, il Client riceve i dati.

- **Gestione dei Dati Ricevuti:**
  -Il client COAP invia richieste GET con un path specifico (ad esempio: /sensors/temperature).
  -La richiesta GET include un TokenID.
  -Il server COAP riceve la richiesta GET, esegue l'elaborazione e restituisce i dati corrispondenti in valore numerico intero.

---

## Specifiche Tecniche

### Struttura del Codice

1. **Client:**
   - **`coap.js`**: Modulo l'invio delle richieste GET, rimane in ascolto per eventuali risposte.
   - **`influx.js`**: Gestione delle connessioni al database InfluxDB.

2. **Server:**
   - **`CoAP.cs`**: Modulo per la gestione delle richieste CoAP.
   - **Sensori simulati:** File dedicati per ciascun sensore.

### Endpoint CoAP

- **Dati Sensori:**
  - Path: `/sensors/<sensor_name>`
  - Esempi:
    - `/sensors/temperature`
    - `/sensors/waterLevel`
    - `/sensors/filterStatus`
