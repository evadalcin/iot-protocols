require('dotenv').config();
var mysql = require('mysql2');
var restify = require('restify');

var server = restify.createServer();
server.use(restify.plugins.bodyParser());

var connection = mysql.createConnection({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_DATABASE
});

connection.connect(function (err) {
    if (err) {
        console.error('Error connecting to MySQL: ' + err.stack);
        return;
    }
    console.log('Connected to MySQL as id ' + connection.threadId);
});

server.post('/water_data', function (req, res, next) {
    const { flowRate } = req.body;
    if (flowRate === undefined) {
        res.send(400, 'Invalid data received');
        return next();
    }

    const query = 'INSERT INTO water_data (flow_rate) VALUES (?)';

    connection.execute(query, [flowRate], function (err, results) {
        if (err) {
            console.error('Error inserting data into water_data: ' + err.stack);
            res.send(500, 'Error inserting data');
            return next();
        }

        res.send(200, 'Water data received and inserted successfully');
        return next();
    });
});

server.post('/temperature_data', function (req, res, next) {
    const { waterTemperature } = req.body;
    if (waterTemperature === undefined) {
        res.send(400, 'Invalid data received');
        return next();
    }

    const query = 'INSERT INTO temperature_data (water_temperature) VALUES (?)';

    connection.execute(query, [waterTemperature], function (err, results) {
        if (err) {
            console.error('Error inserting data into temperature_data: ' + err.stack);
            res.send(500, 'Error inserting data');
            return next();
        }

        res.send(200, 'Temperature data received and inserted successfully');
        return next();
    });
});

server.listen(8011, function () {
    console.log('%s listening at %s', server.name, server.url);
});
