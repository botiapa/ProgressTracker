const webSocket = require("nodejs-websocket");
const authenticated = [];
module.exports = {
    ws : null,
    init : function(db){
        this.ws = webSocket.createServer(function(conn) {
            var hash = conn.path.slice(1);
            if(hash.length == 1) {
                conn.close();
            }
            else{
                checkIfLoggedIn(db, conn, hash, function(author) {
                    authenticated.push({conn : conn, author : author})
                });
            }
        });

        this.ws.listen(8001);
    },
    messageUpdate : function(message, type) {
        authenticated.forEach(elem => {
            elem.conn.sendText(JSON.stringify({type : type, message : message}));
        });
    }
}

function checkIfAuthenticated(conn) {

}

function checkIfLoggedIn(db, wsConn, hash, callback) {
    if(!hash) {
        wsConn.close();
        return;
    }
    db.query("SELECT * FROM authors WHERE hash = ?", [hash], function(error, results, fields) {
        if(results.length == 1) {
            callback(results[0]);
            return;
        }
        else {
            wsConn.close();
        }
    });
}