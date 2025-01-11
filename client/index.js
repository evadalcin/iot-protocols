const coap = require('coap')
const { insertWaterCoolerData } = require('./influx')

URL = process.env.URL
TOKEN = process.env.TOKEN
BUCKET = process.env.BUCKET
COAP_HOST = process.env.COAP_HOST
BUCKET = process.env.BUCKET


function fetchSensorData() {
    const req = coap.request({
        host: COAP_HOST,
        BUCKET: BUCKET,
        pathname: '/water_coolers',
        method: 'GET'
    })

    req.on('response', async (res) => {
        try {
            const payload = res.payload.toString()
            const sensors = JSON.parse(payload)

            for (const sensor of sensors) {
                try {
                    await insertWaterCoolerData(WATER_COOLER_ID, sensor.Name, sensor.Value)
                    console.log(`Dati salvati per ${sensor.Name}: ${sensor.Value}`)
                } catch (err) {
                    console.error(`Errore nel salvare i dati per ${sensor.Name}:`, err)
                }
            }
        } catch (err) {
            console.error('Errore nel processare la risposta:', err)
        }
    })

    req.on('error', (err) => {
        console.error('Errore nella richiesta CoAP:', err)
    })

    req.end()
}

console.log('Client CoAP avviato. Polling dei dati dalla casetta dell\'acqua...')
setInterval(fetchSensorData, 1000)