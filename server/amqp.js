const amqp = require('amqplib');
const { insertWaterCoolerData } = require('./influx');

async function start() {
    const connection = await amqp.connect(process.env.AMQP_URL);
    const channel = await connection.createChannel();

    const exchangeName = 'sensor_data_exchange';
    await channel.assertExchange(exchangeName, 'topic', { durable: false });

    const queue = await channel.assertQueue('', { exclusive: true });

    console.log(' [*] Waiting for logs. To exit press CTRL+C');

    const routingKeyPattern = 'water_coolers.*.sensor.#';

    await channel.bindQueue(queue.queue, exchangeName, routingKeyPattern);

    channel.consume(queue.queue, async (msg) => {
        if (msg !== null) {
            const routingKey = msg.fields.routingKey;
            const sensorData = JSON.parse(msg.content.toString());
            const { Name, Value } = sensorData;

            const parts = routingKey.split('.');
            const coolerId = parts[1]; // ID della casetta

            try {
                await insertWaterCoolerData(coolerId, Name, Value);
                console.log("Routing key:", msg.fields.routingKey, "- Data inserted: ", sensorData);

                channel.ack(msg);
            } catch (err) {
                console.error('Failed to insert data into InfluxDB', err);

                channel.nack(msg);
            }
        }
    });
}

start().catch(console.error);