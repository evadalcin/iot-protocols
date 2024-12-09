# Sistema di Monitoraggio della Casetta dell'Acqua - Protocollo AMQP

**Autori**  
Dal Cin Eva e Di Bert Giacomo

---

### Panoramica

Il progetto implementa un sistema di monitoraggio della casetta dell'acqua, raccogliendo dati da sensori (come temperatura dell'acqua, livello dell'acqua e stato del filtro) e inviandoli a un server per l'elaborazione e l'archiviazione in un database InfluxDB. La comunicazione tra il client (casetta) e il server è realizzata tramite il protocollo AMQP (Advanced Message Queuing Protocol), utilizzando RabbitMQ come broker di messaggi.

---

### Cos'è AMQP?

AMQP è un protocollo di messaggistica aperto e standardizzato che consente la comunicazione tra diverse applicazioni in modo asincrono. Fornisce un meccanismo affidabile per inviare e ricevere messaggi, garantendo che i dati vengano consegnati correttamente anche in caso di errori temporanei nella rete. In un'architettura basata su AMQP, i messaggi vengono inviati a una coda in un broker di messaggi (come RabbitMQ), e poi i consumatori (come il server) prelevano i messaggi da quella coda per l'elaborazione.

---

### Architettura e Funzionamento del Sistema

Il sistema di monitoraggio è suddiviso in due componenti principali:

1. **Client (Lato Casetta dell'Acqua)**:  
   Il client raccoglie i dati dai sensori e li invia al server tramite RabbitMQ usando il protocollo AMQP. Ogni sensore invia i propri dati come messaggi serializzati in formato JSON.

2. **Server (Lato Server)**:  
   Il server si connette al broker AMQP e riceve i messaggi inviati dal client. Una volta ricevuti, i dati vengono elaborati e archiviati in un database InfluxDB per ulteriori analisi.

### Dettaglio del Protocollo AMQP

#### **Componente Client (Lato Casetta dell'Acqua)**

Il client utilizza la libreria `RabbitMQ.Client` per comunicare con il broker RabbitMQ. Quando il client è pronto, invia i dati dei sensori a una coda definita, che in questo caso è chiamata `sensor_data_queue`. Ogni sensore invia il proprio valore (ad esempio, temperatura, livello dell'acqua, stato del filtro) come messaggio al server, tramite il canale AMQP.

#### **Componente Server (Lato Server)**

Il server si iscrive alla coda `sensor_data_queue` e riceve i messaggi. Quando un messaggio viene ricevuto, il server lo elabora, estrae i dati (come il nome del sensore e il valore) e li inserisce nel database InfluxDB. Il server utilizza la libreria `amqplib` per interagire con RabbitMQ in modo asincrono.

### Dettaglio della Classe `Amqp` (Client)

La classe `Amqp` è responsabile della gestione della connessione con il broker RabbitMQ e dell'invio dei messaggi. Di seguito sono descritti i dettagli principali della classe:

#### 1. **Campi della Classe**

- **`_hostname`**:  
  Questo campo rappresenta l'URL del server RabbitMQ al quale la classe si connetterà. L'URL è fornito come variabile d'ambiente (`AMQP_URL`) quando il client è avviato.

- **`QUEUE_NAME`**:  
  È il nome della coda RabbitMQ in cui i messaggi verranno inviati. In questo caso, è chiamata `sensor_data_queue`. La coda è configurata come non durevole, non esclusiva e non auto-eliminante.

- **`_connection`**:  
  Rappresenta la connessione aperta al broker RabbitMQ. La connessione viene gestita tramite l'oggetto `IConnection`.

- **`_channel`**:  
  Rappresenta il canale di comunicazione tramite cui i messaggi vengono inviati. I canali sono leggeri e vengono utilizzati per inviare o ricevere messaggi da una coda.

#### 2. **Costruttore `Amqp`**

Il costruttore accetta un parametro `hostname`, che definisce l'URL del server RabbitMQ al quale la classe si connetterà. Viene utilizzato per creare una `ConnectionFactory` che inizializza la connessione con il broker.

#### 3. **Metodo `InitializeAmqpClient`**

Questo metodo gestisce la creazione della connessione e del canale per la comunicazione con RabbitMQ. Ecco i passaggi principali:

- **Creazione della Connessione**:  
  Una `ConnectionFactory` viene utilizzata per creare una connessione asincrona al broker RabbitMQ, utilizzando l'`_hostname`.

- **Creazione del Canale**:  
  Dopo aver stabilito la connessione, viene creato un canale di comunicazione (`_channel`). Questo canale è utilizzato per inviare i messaggi alla coda.

- **Dichiarazione della Coda**:  
  Viene dichiarata la coda `sensor_data_queue` usando il metodo `QueueDeclareAsync`. Questo garantisce che la coda esista prima di inviare i messaggi.

#### 4. **Metodo `Send`**

Il metodo `Send` invia i messaggi alla coda RabbitMQ. I passaggi principali includono:

- **Conversione del Messaggio**:  
  Il dato da inviare (un oggetto JSON serializzato) viene convertito in un array di byte usando la codifica UTF-8.

- **Invio del Messaggio**:  
  Il messaggio viene pubblicato nella coda utilizzando il metodo `BasicPublishAsync`. Il messaggio è identificato dal tipo di sensore (ad esempio, temperatura, livello dell'acqua, stato del filtro).

- **Log del Messaggio**:  
  Una volta inviato il messaggio, viene stampato un log sulla console che indica che il dato è stato inviato con successo.

#### 5. **Metodo `Close`**

Questo metodo chiude sia la connessione che il canale di RabbitMQ, pulendo correttamente le risorse prima di terminare l'applicazione.

### Comportamento Complessivo

In sintesi, il client:

1. Si connette a un server RabbitMQ.
2. Inizializza una connessione e un canale di comunicazione.
3. Invio dei messaggi (contenenti i dati dei sensori) alla coda `sensor_data_queue`.
4. Chiude la connessione e il canale una volta terminato il processo.

### Potenziali Utilizzi del Protocollo AMQP

Questo tipo di implementazione è particolarmente utile in scenari in cui diverse applicazioni (come i sensori e il server di raccolta dati) devono comunicare tra loro in modo asincrono e scalabile. L'uso di un broker di messaggi come RabbitMQ consente di:

- **Garantire la consegna dei messaggi**: AMQP assicura che i messaggi vengano ricevuti e processati anche in caso di errori temporanei nella comunicazione.
- **Gestire flussi di dati in tempo reale**: AMQP è ideale per scenari in tempo reale, come il monitoraggio remoto di sensori.

### Conclusioni

Il protocollo AMQP, attraverso RabbitMQ, fornisce una soluzione scalabile e affidabile per la gestione della comunicazione tra diverse componenti di un sistema. In questo caso, il sistema di monitoraggio della casetta dell'acqua utilizza RabbitMQ per garantire una trasmissione sicura e efficiente dei dati dei sensori, che vengono poi elaborati e archiviati nel database InfluxDB per future analisi.