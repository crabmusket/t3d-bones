displaySplashWindow("splash.bmp");

// Needed for client-side stuff like the GUI.
exec("lib/simpleNet/client.cs");
SimpleNetClient.init();

singleton Material(BlankWhite) {
   diffuseColor[0] = "White";
};

// Called on the client-side when we are first assigned a control object on the
// server (i.e. our camera).
function GameConnection::initialControlSet(%this) {
   // Activate HUD which allows us to see the game.
   Canvas.setContent(PlayGui);
   activateDirectInput();
}

// Load console.
exec("lib/console/main.cs");

// Load HUD.
exec("tutorials/client_server/playGui.gui");
exec("tutorials/client_server/mainMenuGui.gui");

Canvas.setContent(MainMenuGui);
closeSplashWindow();
Canvas.showWindow();

function play() {
   // Start playing the game!
   createLevel();
   SimpleNetClient.connectTo(self);
}

function connect() {
   SimpleNetClient.connectTo(localhost, 28001);
}

function endGame() {
   GameGroup.delete();
   SimpleNetClient.disconnect();
   Canvas.setContent(MainMenuGui);
}

function disconnect() {
   SimpleNetClient.disconnect();
   Canvas.setContent(MainMenuGui);
}

// Allow us to exit the game...
GlobalActionMap.bind("keyboard", "escape", "exit");

// Called when the engine is shutting down.
function exit() {
   SimpleNetClient.destroy();
   quit();
}
