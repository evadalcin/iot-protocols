require('dotenv').config();
const mqtt = require('mqtt');
const { insertWaterCoolerData } = require('./influx'); 

const mqttClient = mqtt.connect(`mqtt://${process.env.MQTT_HOST}`); 

mqttClient.on('connect', () => {
    console.log('Connesso al broker MQTT');
    mqttClient.subscribe('iot2025/water_coolers/#'); 
});

mqttClient.on('message', async (topic, message) => {
    console.log('Messaggio ricevuto:', message.toString());
    try {
        const sensorData = JSON.parse(message.toString()); 
        console.log('Dati JSON parsati:', sensorData);
        const { Name, Value } = sensorData;
        if (Name === undefined || Value === undefined) {
            console.log('Dati incompleti ricevuti. Non posso inserire nel database.');
            return;
        }
        const topicParts = topic.split('/');
        const coolerId = parseInt(topicParts[2], 10); 

        console.log('Inserisco questi dati nel DB InfluxDB:', {
            coolerId,
            Name,
            Value
        });

        await insertWaterCoolerData(coolerId, Name, Value);
        console.log('Dati salvati nel database InfluxDB');
    } catch (err) {
        console.error('Errore nel parsing del messaggio JSON:', err);
    }
});
