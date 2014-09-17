// Open a console window and create a null GFX device since we won't be
// rendering a usual game vanvas.
enableWinConsole(true);
GFXInit::createNullDevice();

// This function is called on the server when a client on another machine
// requests to connect to our game. Return "" to accept the connection, or
// anything else to reject it.
function GameConnection::onConnectRequest(%this, %addr) {
   return "";
}

// Called when a client is allowed to connect to the game. We start transmitting
// currently loaded datablocks to the client.
function GameConnection::onConnect(%this) {
   %this.transmitDataBlocks(0);
}

// Called when all datablocks have been transmitted. At this point we can start
// ghosting objects to the client, and perform server-side setup for them.
function GameConnection::onDataBlocksDone(%this) {
   %this.activateGhosting();
   %this.onEnterGame();
}

// When the client drops from the game, we clean up after them.
function GameConnection::onDrop(%this, %reason) {
   %this.onLeaveGame();
}

function updateTSShapeLoadProgress() {}

// Load up game code.
exec("tutorials/client_server/config.cs");
exec("tutorials/client_server/server.cs");

// Create a local game server and connect to it.
new SimGroup(ServerGroup);

// Start game-specific scripts.
setNetPort($server::port);
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
