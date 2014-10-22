new ScriptObject(SimpleNetServer) {
   port = 28001;
};

function SimpleNetServer::init(%this) {
   return %this;
}

function SimpleNetServer::initDedicated(%this) {
   // Open a console window and create a null GFX device since we won't be
   // rendering a usual game canvas.
   enableWinConsole(true);
   GFXInit::createNullDevice();

   return %this;
}

function SimpleNetServer::host(%this) {
   setNetPort(%this.port);
   allowConnections(true);
   return %this;
}

function SimpleNetServer::stop(%this) {
   allowConnections(false);
   return %this;
}

function SimpleNetServer::destroy(%this) {
   deleteDatablocks();
   %this.delete();
}

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

//function updateTSShapeLoadProgress() {}
