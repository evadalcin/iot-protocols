
# Sistema di Monitoraggio della Casetta dell'Acqua - Protocollo AMQP

**Autori**  
Dal Cin Eva e Di Bert Giacomo

## Panoramica

Il progetto implementa un sistema di monitoraggio per la **casetta dell'acqua**, raccogliendo dati da sensori (ad esempio, temperatura dell'acqua, livello dell'acqua e stato del filtro) e inviandoli a un server per l'elaborazione e l'archiviazione in un database **InfluxDB**. La comunicazione tra il client (casetta) e il server avviene tramite il protocollo **AMQP (Advanced Message Queuing Protocol)**, utilizzando **RabbitMQ** come broker di messaggi. L'architettura è basata su un **exchange di tipo topic**, che consente una comunicazione più flessibile e mirata tra i vari componenti.

**AMQP** è un protocollo di messaggistica aperto e standardizzato che facilita la comunicazione asincrona tra diverse applicazioni. Esso fornisce un meccanismo affidabile per l'invio e la ricezione di messaggi, garantendo la corretta consegna anche in caso di problemi temporanei di rete. In un'architettura basata su AMQP, i messaggi vengono inviati a una coda (o a un **exchange**) nel broker di messaggi, e i consumatori prelevano i messaggi per l'elaborazione.

## Architettura e Funzionamento del Sistema

Il sistema di monitoraggio si compone di due principali componenti:

### Client (Lato Casetta dell'Acqua)

Il client raccoglie i dati dai sensori e li invia al server tramite RabbitMQ usando il protocollo AMQP. I dati vengono inviati all'exchange **sensor_data_exchange** di tipo **topic**, dove ogni messaggio è associato a una **routing key** specifica, che facilita il routing dei messaggi verso le giuste code in base al tipo di sensore e alla casetta dell'acqua.
### Server (Lato Server)

Il server si connette al broker RabbitMQ e riceve i messaggi inviati dal client. Una volta ricevuti, i dati vengono elaborati e archiviati nel database InfluxDB per ulteriori analisi. Il server ascolta l'exchange **sensor_data_exchange** e si iscrive alla coda corrispondente tramite il pattern di routing **water_coolers.*.sensor.#**, che consente di gestire dinamicamente i dati provenienti da più casette.

## Dettaglio dell'Exchange e della Routing Key

### Componente Client (Lato Casetta dell'Acqua)

Il client utilizza l'exchange **sensor_data_exchange** di tipo **topic** per inviare i dati dei sensori. La routing key è formata come segue:

```
water_coolers.<cooler_id>.sensor.<sensor_name>
```

Dove:

- `cooler_id` è l'ID della casetta dell'acqua.
- `sensor_name` è il tipo di sensore (ad esempio, temperatura, livello dell'acqua, stato del filtro).

Il messaggio inviato è un oggetto JSON serializzato contenente i dati del sensore.

### Componente Server (Lato Server)

Il server si iscrive alla coda associata all'exchange tramite un pattern di routing, ad esempio:

```
water_coolers.*.sensor.#
```

Il server riceve i messaggi, estrae i dati del sensore e li inserisce nel database InfluxDB per ulteriori analisi.

