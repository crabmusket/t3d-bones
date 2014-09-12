// Display a splash window immediately to improve app responsiveness before
// engine is initialized and main window created
displaySplashWindow("splash.bmp");

//-----------------------------------------------------------------------------
// Load up scripts to initialise subsystems.
exec("sys/main.cs");

// The canvas needs to be initialized before any gui scripts are run since
// some of the controls assume that the canvas exists at load time.
createCanvas("T3Dbones");

// Start rendering and stuff.
initRenderManager();
initLightingSystems("Basic Lighting"); 

// Start audio.
sfxStartup();

// Provide stubs so we don't get console errors. If you actually want to use
// any of these functions, be sure to remove the empty definition here.
function onDatablockObjectReceived() {}
function onGhostAlwaysObjectReceived() {}
function onGhostAlwaysStarted() {}
function updateTSShapeLoadProgress() {}

//-----------------------------------------------------------------------------
// Load console.
exec("lib/console/main.cs");

// Load up game code.
exec("game/main.cs");

// Called when we connect to the local game.
function GameConnection::onConnect(%this) {
   %this.transmitDataBlocks(0);
}

// Called when all datablocks from above have been transmitted.
function GameConnection::onDataBlocksDone(%this) {
   closeSplashWindow();
   Canvas.showWindow();

   // Start sending ghosts to the client.
   %this.activateGhosting();
   %this.onEnterGame();
}

// Create a local game server and connect to it.
new SimGroup(ServerGroup);
new GameConnection(ServerConnection);
// This calls GameConnection::onConnect.
ServerConnection.connectLocal();

// Start game-specific scripts.
onStart();

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onExit() {
   // Clean up ghosts.
   ServerConnection.delete();

   // Clean up game objects and so on.
   onEnd();

   // Delete server-side objects and datablocks.
   ServerGroup.delete();
   deleteDataBlocks();
}
