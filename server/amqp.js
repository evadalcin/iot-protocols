const amqp = require('amqplib');
const { insertWaterCoolerData } = require('./influx');

async function start() {
    const connection = await amqp.connect(process.env.AMQP_URL);
    const channel = await connection.createChannel();

    const queue = 'sensor_data_queue';
    await channel.assertQueue(queue, { durable: false });

    console.log("Waiting for messages in %s", queue);

    channel.consume(queue, async (msg) => {
        if (msg !== null) {
            const sensorData = JSON.parse(msg.content.toString());
            const { Name, Value } = sensorData;
            const coolerId = 123;

            try {
                await insertWaterCoolerData(coolerId, Name, Value);
                console.log("Data inserted:", sensorData);

                channel.ack(msg);
            } catch (err) {
                console.error('Failed to insert data into InfluxDB', err);
                channel.nack(msg);
            }
        }
    });
}

start().catch(console.error);