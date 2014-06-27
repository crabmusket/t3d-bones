// Display a splash window immediately to improve app responsiveness before
// engine is initialized and main window created
displaySplashWindow("splash.bmp");

// Console does something.
setLogMode(2);
// Disable script trace.
trace(false);

//-----------------------------------------------------------------------------
// Load up scripts to initialise subsystems.
exec("sys/main.cs");

// The canvas needs to be initialized before any gui scripts are run since
// some of the controls assume that the canvas exists at load time.
createCanvas("T3Dbones");

// Start rendering and stuff.
initRenderManager();
initLightingSystems("Basic Lighting"); 

// Start PostFX. If you use "Advanced Lighting" above, uncomment this.
//initPostEffects();

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
exec("console/main.cs");

// Load up game code.
exec("game/main.cs");

// Called when we connect to the local game.
function GameConnection::onConnect(%this) {
   %this.transmitDataBlocks(0);
}

// Called when all datablocks from above have been transmitted.
function GameConnection::onDataBlocksDone(%client) {
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

// Allow us to exit the game...
GlobalActionMap.bind("keyboard", "escape", "quit");

// Start game-specific scripts.
onStart();

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onExit() {
   // Clean up game objects and so on.
   onEnd();

   // Delete server-side objects and datablocks.
   ServerGroup.delete();
   deleteDataBlocks();
}
