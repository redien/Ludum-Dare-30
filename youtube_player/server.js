
var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);

app.get('/', function(req, res){
    res.sendFile(__dirname + '/public/index.html');
});

var sockets = [];

io.on('connection', function(socket){
    console.log('a user connected');
    sockets.push(socket);

    socket.on('disconnect', function () {
        var index = sockets.indexOf(socket);
        sockets.splice(index, 1);
    });
    
    socket.on('type', function(type){
        console.log('User is of type: ' + type);
        socket.userType = type;
    });
    
    var sendCommandToMedia = function (command) {
        sockets.forEach(function (socket) {
            if (socket.userType === "media") {
                socket.emit(command);
                console.log('sent ' + command);
            }
        });
    };

    socket.on('stop', function(){
        console.log('received stop');
        sendCommandToMedia('stop');
    });

    socket.on('play', function(){
        console.log('received play');
        sendCommandToMedia('play');
    });

    socket.on('previous', function(){
        console.log('received previous');
        sendCommandToMedia('previous');
    });

    socket.on('next', function(){
        console.log('received next');
        sendCommandToMedia('next');
    });
});

http.listen(3000, function(){
    console.log('listening on *:3000');
});
