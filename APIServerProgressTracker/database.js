module.exports = function() {
    const fs = require('fs');
    const mysql = require('mysql');

    path = require('path'),    
    filePath = path.join(__dirname, 'auth.key');

    split = fs.readFileSync(filePath, {encoding: "utf8"}).split(";");
    const SERVER = split[0];
    const DATABASE = split[1];
    const UID = split[2];
    const PASSWORD = split[3];
    const DBPORT = split[4];

    var connection = mysql.createConnection({
        host     : SERVER,
        user     : UID,
        password : PASSWORD,
        database : DATABASE,
        port: DBPORT
    });
    connection.connect();
    return connection;
}