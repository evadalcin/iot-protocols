require('dotenv').config();
const restify = require('restify');
const { insertWaterCoolerData } = require('./mysql');

const server = restify.createServer();
server.use(restify.plugins.bodyParser());

server.post('/water_coolers/:id', async (req, res) => {
    const coolerId = req.params.id;
    const { waterTemperature, availableWaterAmount, isFilterOperational } = req.body;

    try {
        await insertWaterCoolerData(coolerId, waterTemperature, availableWaterAmount, isFilterOperational);
        res.send(200, { status: 'Data received and saved' });
    } catch (err) {
        console.error('Error saving data:', err);
        res.send(500, { status: 'Error saving data' });
    }
});

server.listen(8011, () => {
    console.log('%s listening at %s', server.name, server.url);
});
