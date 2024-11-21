require('dotenv').config();
const mqtt = require('mqtt');
const { insertWaterCoolerData } = require('./influx');

const mqttClient = mqtt.connect(`mqtt://${process.env.MQTT_HOST}`);

mqttClient.on('connect', () => {
    console.log('Connesso al broker MQTT');
    mqttClient.subscribe('iot2025/water_coolers/+/sensor/');
    mqttClient.subscribe('iot2025/water_coolers/+/command/');
});

let nightLightStatus = false;
let maintenanceModeStatus = false;

function executeCommand(command, status) {
    switch (command) {
        case 'light_on':
            if (status && !nightLightStatus) {
                turnOnNightLight();
                nightLightStatus = true;
            } else if (!status && nightLightStatus) {
                turnOffNightLight();
                nightLightStatus = false;
            }
            break;
        case 'maintenance_mode':
            if (status && !maintenanceModeStatus) {
                enableMaintenanceMode();
                maintenanceModeStatus = true;
            } else if (!status && maintenanceModeStatus) {
                disableMaintenanceMode();
                maintenanceModeStatus = false;
            }
            break;
        default:
            console.log('Comando sconosciuto');
    }
}

function turnOnNightLight() {
    console.log('Luce notturna accesa');
}

function turnOffNightLight() {
    console.log('Luce notturna spenta');
}

function enableMaintenanceMode() {
    console.log('Mod. manutenzione attivata');
}

function disableMaintenanceMode() {
    console.log('Mod. manutenzione disattivata');
}



mqttClient.on('message', async (topic, message) => {
    console.log('Messaggio ricevuto:', message.toString());

    try {
        if (topic.startsWith('iot2025/water_coolers') && topic.includes('/sensor')) {
            const sensorData = JSON.parse(message.toString());
            console.log('Dati JSON parsati:', sensorData);
            const { Name, Value } = sensorData;
            const topicParts = topic.split('/');
            const coolerId = parseInt(topicParts[2], 10);

            await insertWaterCoolerData(coolerId, Name, Value);
        }
        else if (topic.startsWith('iot2025/water_coolers') && topic.includes('/command')) {
            const commandData = JSON.parse(message.toString());
            if (commandData.command && commandData.status !== undefined) {
                executeCommand(commandData.command, commandData.status);
            } else {
                console.log('Dati comando non validi');
            }
        }
    } catch (err) {
        console.error('Errore nel parsing del messaggio JSON:', err);
    }
});
