const crypto = require('crypto');
const multer  = require('multer')
const upload = multer({ dest: 'uploads/', fileFilter: FileFilter})
const fs = require('fs');


module.exports = function(app, ws, db) {
    app.get("/message", (req, res) => {
        db.query('SELECT * from messages', function (error, msgResults, fields) {
            if(error == null) 
			{
				gatherAllMessagesWithAuthor(db, msgResults, [], function(error, results) 
				{
					if(error == null) 
					{
						res.json(results);
					}
					else
						res.sendStatus(500);
				})
				
			}
			else {
				console.log(error);
				res.sendStatus(500);
			}
        });
    });
    app.post("/message/create", function(req, res) {
        checkIfLoggedIn(req, res, function(author) {
            var contents = req.body.Contents || "";
            var progress = req.body.Progress || 0;
            var ID = crypto.randomBytes(18).toString('base64');
            db.query("INSERT INTO messages(ID, Title, Contents, Progress, Author) VALUES(?,?,?,?,?)", [ID, req.body.Title, contents, progress, author.ID], function(error, results, fields) {
                if(error == null) {
                    res.sendStatus(200);
                    db.query("SELECT * FROM messages WHERE ID = ?", [ID], function(error, results, fields) { // Update the new message on all connected clients
                        db.query("SELECT ID,Name,ImageUrl FROM authors WHERE ID = ?", [results[0].Author], function(error, authors, fields) 
						{
							let authorObject = {ID : authors[0].ID, Name : authors[0].Name, ImageUrl : authors[0].ImageUrl};
							ws.messageUpdate({ID : ID, Title : results[0].Title, Contents : results[0].Contents, Progress : results[0].Progress, Author : authorObject, LastModified : results[0].Last_Modified}, "CREATE");
						})
                    })
                }
                else {
                    console.log(error);
                    res.sendStatus(500);
                }
            });
        });
    });	
    app.post("/message/update", function(req, res) {
        checkIfLoggedIn(req, res, function(author) {
            if(!req.body.ID) {
                req.sendStatus(400);
                return;
            }
            db.query("SELECT * FROM messages WHERE ID = ?", [req.body.ID], function(error, results, fields) {
                if(error == null && results.length == 1 && author.ID == results[0].Author) {
                    let title = req.body.Title || results[0].Title;
                    let contents = req.body.Contents || results[0].Contents;
                    let progress = req.body.Progress || results[0].Progress;
                    db.query("UPDATE messages SET Title = ?, Contents = ?, Progress = ? WHERE ID = ?", [title, contents, progress, req.body.ID], function(error, __results, fields) { // Update the new message on all connected clients
						if(error == null) {
							res.sendStatus(200);
							db.query("SELECT * FROM messages WHERE ID = ?", [req.body.ID], function(error, results, fields) {
								db.query("SELECT ID,Name,ImageUrl FROM authors WHERE ID = ?", [results[0].Author], function(error, authors, fields) 
								{
									let authorObject = {ID : authors[0].ID, Name : authors[0].Name, ImageUrl : authors[0].ImageUrl};
									ws.messageUpdate({ID : results[0].ID, Title : results[0].Title, Contents : results[0].Contents, Progress : results[0].Progress, Author : authorObject, LastModified : results[0].Last_Modified}, "UPDATE");
								})
							})
						}
						else {
							console.log(error);
							res.sendStatus(500);
							return;
						}
                    })
					
                    
                }
				else if(error == null && author.ID != results[0].ID) 
				{
					res.sendStatus(403);
				}
                else if(error == null) {
                    res.sendStatus(404);
                }
                else {
                    console.log(error);
                    res.sendStatus(500);
                }
            })
        });
    });
    app.post("/message/delete", function(req, res) {
        checkIfLoggedIn(req, res, function(author) {
            if(!req.body.ID) {
                req.sendStatus(400);
                return;
            }
            db.query("SELECT * FROM messages WHERE ID = ?", [req.body.ID], function(error, results, fields) {
                if(error == null && results.length == 1) {
                    if(author.ID == results[0].Author) {
                        db.query("DELETE FROM messages WHERE ID = ?", [req.body.ID])
                        ws.messageUpdate({ID : results[0].ID}, "DELETE");
                        res.sendStatus(200);
                    }
                    else {
                        res.sendStatus(403);
                    }
                }
                else if(error == null) {
                    res.sendStatus(404);
                }
                else {
                    console.log(error);
                    res.sendStatus(500);
                }
            })
        });
    });
    app.post("/account/login", function(req, res) {
        if(!req.body.username || !req.body.password)
        {
            res.sendStatus(400);
            return;
        }
        var loginHash = getRandomHash();
        db.query("SELECT * FROM authors WHERE Name = ?", [req.body.username], function(error, results, fields) {
            if(error == null) 
			{
				if(results.length == 1) {
					var hashedPassword = hashPasswordWithSalt(req.body.password, results[0].Salt);
					db.query("SELECT * FROM authors WHERE Password = ? AND ID = ?", [hashedPassword[1], results[0].ID], function(error, result, fields) {
						if(results.length == 1) { // GOOD PASSWORD
							db.query("UPDATE authors SET hash = ? WHERE Password = ? AND ID = ?", [loginHash, hashedPassword[1], results[0].ID]);
							res.send(loginHash);
						}
						else { // BAD PASSWORD
							res.sendStatus(401);
						}
					});
				}
				else if(results.length > 1) { // THERE IS MORE THAN ONE USER WITH THE SAME NAME
					res.sendStatus(500);
				}
				else { // NAME NOT FOUND
					res.sendStatus(401);
				}
			}
			else 
			{
				console.log(error);
				res.sendStatus(500);
			}
        });
    });
    app.post("/account/register", function(req, res) {
        if(!req.body.username || !req.body.password)
        {
            res.sendStatus(400);
            return;
        }
        var username = req.body.username;
        var hashedPassword = hashPassword(req.body.password);
        var imageUrl = req.body.imageurl || "";

        db.query("SELECT * FROM authors WHERE Name=?", username, function(error, results, fields) {
            if (error) throw error;
            if(results.length == 0) {
                var ID = randomValueBase64(18);
                db.query("INSERT INTO authors(ID, Name, Password, Salt, ImageUrl) VALUES(?,?,?,?,?);", [ID, username, hashedPassword[1], hashedPassword[0], imageUrl], function(error, results, fields) {
                    if (error) throw error;
                    res.sendStatus(200);
                    return;
                });
            }
            else {
                res.sendStatus(400);
                return;
            }
        });
    });

    app.post("/account/info", function(req, res) {
        checkIfLoggedIn(req, res, function(author) {
            res.send(JSON.stringify({ID : author.ID, Name : author.Name, ImageUrl : author.ImageUrl}))
        });
    });

    app.post("/account/uploadimage", upload.single('image'), function(req, res) {
        checkIfLoggedIn(req, res, function(author) {
            fs.rename(req.file.path, "uploads/avatar_" + author.ID, function(error) {
                if(error == null)
                    res.sendStatus(200);
                else {
                    console.log(error);
                    res.sendStatus(500);
                }
            });
        });
    });

    function checkIfLoggedIn(req, res, callback) {
        if(!req.body.hash) {
            res.sendStatus(401);
            return;
        }
        db.query("SELECT * FROM authors WHERE hash = ?", [req.body.hash], function(error, results, fields) {
            if(error || results === undefined) 
			{
				res.sendStatus(401);
				return;
			} 
			if(results.length == 1) {
                callback(results[0]);
                return;
            }
            else if(results.length > 1) {
                res.sendStatus(500);
            }
            else {
                res.sendStatus(401);
            }
        });
    }
};

function hashPassword(password) {
    var salt = crypto.randomBytes(255).toString('latin1');
    var iterations = 100000;
    var hash = crypto.pbkdf2Sync(password, salt, iterations, 64, "sha512").toString('base64');
    return [salt, hash]
}

function hashPasswordWithSalt(password, salt) {
    var iterations = 100000;
    var hash = crypto.pbkdf2Sync(password, salt, iterations, 64, "sha512").toString('base64');
    return [salt, hash]
}

function getRandomHash() {
    var current_date = (new Date()).valueOf().toString();
    var random = Math.random().toString();
    return crypto.createHash('sha256').update(current_date + random).digest("base64");
}

function FileFilter(req, file, cb) {

    if(file.mimetype.includes("jpg") || file.mimetype.includes("png")) {
        cb(null, true)
    }
    else {
        cb(null, false)
    }
}

function randomValueBase64(byteSize) {
    return crypto
      .randomBytes(byteSize)
      .toString('base64') // convert to base64 format
      .replace(/\+/g, '0') // replace '+' with '0'
      .replace(/\//g, '0') // replace '/' with '0'
}

function gatherAllMessagesWithAuthor(db, messages, sofar, cb) 
{
	var msg = messages.shift();
	
	if(!msg)
		cb(null, sofar);
	else 
	{
		db.query('SELECT ID,Name,ImageUrl FROM authors WHERE ID = ?', [msg.Author], function(error, results, fields) 
		{
			if(error == null) 
			{
				let authorObject = {ID : results[0].ID, Name : results[0].Name, ImageUrl : results[0].ImageUrl};
				sofar.push({
						"ID": msg.ID,
						"Title": msg.Title,
						"Contents": msg.Contents,
						"Progress": msg.Progress,
						"Author": authorObject});
				gatherAllMessagesWithAuthor(db, messages, sofar, cb);
			}
			else
				cb(error);
		})
	}
}