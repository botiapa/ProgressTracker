const webSocket = require("nodejs-websocket");
const authenticated = [];
module.exports = {
    ws : null,
    init : function(db){
        this.ws = webSocket.createServer(function(conn) {
			console.log("New WS connection");
            var hash = conn.path.slice(1);
            if(hash.length == 1) {
                conn.close();
            }
            else{
                checkIfLoggedIn(db, conn, hash, function(author) {
                    authenticated.push({conn : conn, author : author})
					conn.on('close', function(code, reason) 
					{
						authenticated.splice(authenticated.indexOf({conn : conn, author : author}), 1);
						console.log("WS client disconnected peacefully");
					});
					conn.on('error', function(err) 
					{
						authenticated.splice(authenticated.indexOf({conn : conn, author : author}), 1);
						console.log("WS client disconnected with error");
					});
                });
            }
        });

		this.ws.on("error", function(error) 
		{
			console.log(error);
		});
        this.ws.listen(8001);
    },
    messageUpdate : function(message, type) {
        authenticated.forEach(elem => {
			if(elem.readyState == elem.OPEN) 
			{
				elem.conn.sendText(JSON.stringify({type : type, message : message}));
			}
        });
    }
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