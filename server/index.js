
var restify = require('restify');
var mysql = require('mysql2');

const db = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: 'Vmware1!',
    database: 'iot'   
});

db.connect((err) => {
    if (err) {
        console.error('Errore di connessione al database:', err);
        return;
    }
    console.log('Connesso al database MySQL');
});

var server = restify.createServer();
server.use(restify.plugins.bodyParser());

server.post('/water_coolers/:id', function (req, res, next) {
    const coolerId = req.params.id;
    const data = req.body;

    console.log(`Dati ricevuti per cooler ${coolerId}:`, data);

    const temperature = data.waterTemperature;
    const availableWaterAmount = data.availableWaterAmount;
    const filterCondition = data.isFilterOperational;

    const query = `
        INSERT INTO http_protocol (cooler_id, temperature, available_water_amount, filter_condition, received_at)
        VALUES (?, ?, ?, ?, NOW())
    `;

    db.query(query, [coolerId, temperature, availableWaterAmount, filterCondition], (err, result) => {
        if (err) {
            console.error('Errore durante l\'inserimento dei dati:', err);
            res.send(500, { status: 'Errore durante l\'inserimento dei dati' });
            return next();
        }

        console.log('Dati inseriti con successo:', result);
        res.send(200, { status: 'Dati ricevuti e salvati' });
        return next();
    });
});

server.listen(8011, function () {
    console.log('%s listening at %s', server.name, server.url);
});
