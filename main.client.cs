// Display a splash window immediately to improve app responsiveness before
// engine is initialized and main window created
displaySplashWindow("splash.bmp");

// Load up scripts to initialise subsystems.
exec("sys/main.cs");

// The canvas needs to be initialized before any gui scripts are run since
// some of the controls assume that the canvas exists at load time.
createCanvas("T3Dbones dedicated client");

// Start rendering and stuff.
initRenderManager();
initLightingSystems("Advanced Lighting");

// Start audio.
sfxStartup();

// Provide stubs so we don't get console errors. If you actually want to use
// any of these functions, be sure to remove the empty definition here.
function onDatablockObjectReceived() {}
function onGhostAlwaysObjectReceived() {}
function onGhostAlwaysObjectsReceived() {}
function onGhostAlwaysStarted() {}

// Load console.
exec("lib/console/main.cs");

// Load up game code.
exec("tutorials/client_server/config.cs");
exec("tutorials/client_server/client.cs");

// Allow us to exit the game...
GlobalActionMap.bind("keyboard", "escape", "quit");

// Start game-specific scripts.
onStart();

// Called when the engine is shutting down.
function onExit() {
   // Clean up game objects and so on.
   onEnd();
}
