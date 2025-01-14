var { InfluxDB, Point } = require('@influxdata/influxdb-client')
require('dotenv').config();

const url = process.env.URL
const token = process.env.TOKEN
const bucket = process.env.BUCKET
const org = process.env.ORG

const client = new InfluxDB({ url, token })

function insertWaterCoolerData(id, key, value) {
    return new Promise(function (resolve, reject) {

        try {
            let writeApi = client.getWriteApi(org, bucket, 'ns')

            let point = new Point(`water_coolers#${id}`)
                .intField(key, value)

            writeApi.writePoint(point);
            writeApi.flush(); 

            resolve('Success');
        } catch (err) {
            reject(err);
        }
    });
}

module.exports = { insertWaterCoolerData };
