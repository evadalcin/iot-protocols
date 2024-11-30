require('dotenv').config();
const mqtt = require('mqtt');
const { insertWaterCoolerData } = require('./influx');
const mqttClient = mqtt.connect(`mqtt://${process.env.MQTT_HOST}`);

mqttClient.on('connect', () => {
    console.log('Connesso al broker MQTT');
    mqttClient.subscribe('iot2025/water_coolers/+/sensor/#');
});


mqttClient.on('message', async (topic, message) => {
    console.log('Messaggio ricevuto:', message.toString());
    try {
        const sensorData = JSON.parse(message.toString());
        const { Name, Value } = sensorData;
        const topicParts = topic.split('/');
        const coolerId = parseInt(topicParts[2], 10);

        await insertWaterCoolerData(coolerId, Name, Value);
    } catch (err) {
        console.error('Errore nel parsing del messaggio JSON:', err);
    }
});