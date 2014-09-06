// Open a console window since we won't have a canvas.
enableWinConsole(true);
setLogMode(2);

// Script trace.
trace(true);

// Load up game code.
exec("tutorials/dedicated/config.cs");
exec("tutorials/dedicated/server.cs");

function GameConnection::onConnectRequest(%this, %addr, %name) {
   return "";
}

// Called when we connect to the local game.
function GameConnection::onConnect(%this) {
   %this.transmitDataBlocks(0);
}

// Called when all datablocks from above have been transmitted.
function GameConnection::onDataBlocksDone(%this) {
   // Start sending ghosts to the client.
   %this.activateGhosting();
   %this.onEnterGame();
}

function GameConnection::onDrop(%this, %reason) {
   %this.onLeaveGame();
}

// Create a local game server and connect to it.
new SimGroup(ServerGroup);

// Start game-specific scripts.
setNetPort(28000);
allowConnections(true);
onStart();

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onExit() {
   allowConnections(false);

   // Clean up game objects and so on.
   onEnd();

   // Delete server-side objects and datablocks.
   ServerGroup.delete();
   deleteDataBlocks();
}
