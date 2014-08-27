
var command = process.argv[2];

var socket = require('socket.io-client')('http://localhost:3000');

socket.on('connect', function(){
    socket.emit(command);
    console.log('sent ' + command);
    setTimeout(function () {
        socket.disconnect();
    }, 100);
});
