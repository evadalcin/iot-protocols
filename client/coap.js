const coap = require('coap');
const { insertWaterCoolerData } = require('./influx');

const COAP_HOST = '127.0.0.1';
const COAP_PORT = 5683;
const WATER_COOLER_ID = '123';
const POLLING_INTERVAL = 10000;
const sensors = ['temperature', 'filterStatus', 'waterLevel'];

async function fetchSensorData(sensor) {
    return new Promise((resolve, reject) => {
        const req = coap.request({
            host: COAP_HOST,
            port: COAP_PORT,
            pathname: `/sensors/${sensor}`,
            method: 'GET',
            confirmable: true
        });

        const timeout = setTimeout(() => {
            req.cancel();
            reject(new Error(`Timeout: ${sensor}`));
        }, 5000);

        req.on('response', async (res) => {
            clearTimeout(timeout);
            try {
                const rawValue = res.payload.toString();
                //console.log(`Raw value for ${sensor}:`, rawValue);
                const value = parseInt(rawValue);
                //console.log(`Parsed value for ${sensor}:`, value);
                await insertWaterCoolerData(WATER_COOLER_ID, sensor, value);
                resolve(value);
            } catch (err) {
                reject(err);
            }
        });


        req.on('error', (err) => {
            clearTimeout(timeout);
            reject(err);
        });

        req.end();
    });
}

async function startPolling() {
    try {
        while (true) {
            await Promise.all(sensors.map(sensor =>
                fetchSensorData(sensor).catch(err =>
                    console.error(`Error ${sensor}:`, err))
            ));
            await new Promise(resolve => setTimeout(resolve, POLLING_INTERVAL));
        }
    } catch (err) {
        console.error('Fatal error:', err);
        process.exit(1);
    }
}

startPolling();