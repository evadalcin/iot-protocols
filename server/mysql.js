require('dotenv').config();
const mysql = require('mysql2');

const connection = mysql.createConnection({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME
});

connection.connect((err) => {
    if (err) {
        console.error('Error connecting to MySQL:', err);
    } else {
        console.log('Connected to MySQL');
    }
});

function insertWaterCoolerData(coolerId, temperature, availableWaterAmount, filterCondition) {
    return new Promise((resolve, reject) => {
        const query = `
            INSERT INTO http_protocol (cooler_id, temperature, available_water_amount, filter_condition, received_at)
            VALUES (?, ?, ?, ?, NOW())
        `;

        connection.query(query, [coolerId, temperature, availableWaterAmount, filterCondition], (err, result) => {
            if (err) {
                console.error('Error inserting data:', err);
                reject(err);
            } else {
                console.log('Data inserted successfully:', result);
                resolve(result);
            }
        });
    });
}

module.exports = {
    insertWaterCoolerData
};
