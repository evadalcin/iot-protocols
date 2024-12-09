require('dotenv').config();
const restify = require('restify');
const { insertWaterCoolerData } = require('./influx');

const server = restify.createServer();
server.use(restify.plugins.bodyParser());

server.post('/water_coolers/:id', function (req, res, next) {
    const coolerId = req.params.id;
    const { Name, Value } = req.body;

    try {
        insertWaterCoolerData(coolerId, Name, Value);
        res.send(200, { status: 'Dati ricevuti e salvati' });
    } catch (err) {
        console.error('Errore durante il salvataggio dei dati:', err);
        res.send(500, { status: 'Errore nel salvataggio dei dati' });
    }

    return next(); 
});

server.listen(8011, () => {
    console.log('%s in ascolto su %s', server.name, server.url);
});
