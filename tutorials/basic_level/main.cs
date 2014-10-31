displaySplashWindow("splash.bmp");

// Initialise audio, GFX, etc.
exec("lib/sys/main.cs");
Sys.init();

// Needed because we'll be acting as a local server, so we need both server
// and client functions defined.
exec("lib/simpleNet/client.cs");
exec("lib/simpleNet/server.cs");

singleton Material(BlankWhite) {
   diffuseColor[0] = "White";
};

new SimGroup(GameGroup) {
   new LevelInfo(TheLevelInfo) {
      canvasClearColor = "0 0 0";
   };
   new GroundPlane(TheGround) {
      position = "0 0 0";
      material = BlankWhite;
   };
   new Sun(TheSun) {
      azimuth = 230;
      elevation = 45;
      color = "1 1 1";
      ambient = "0.1 0.1 0.1";
      castShadows = true;
   };
};

datablock CameraData(Observer) {};

// When a client enters the game, the server assigns them a camera.
function GameConnection::onEnterGame(%this) {
   new Camera(TheCamera) {
      datablock = Observer;
   };
   TheCamera.setTransform("0 0 2 1 0 0 0");
   // Cameras are not ghosted (sent across the network) by default; we need to
   // do it manually for the client that owns the camera or things will go south
   // quickly.
   TheCamera.scopeToClient(%this);
   // And let the client control the camera.
   %this.setControlObject(TheCamera);
   // Add the camera to the group of game objects so that it's cleaned up when
   // we close the game.
   GameGroup.add(TheCamera);
}

// Load console.
exec("lib/console/main.cs");

// Load HUD.
exec("tutorials/basic_level/playGui.gui");

// Called on the client-side when we are first assigned a control object on the
// server (i.e. our camera).
function GameConnection::initialControlSet(%this) {
   // Activate HUD which allows us to see the game.
   Window.canvas.setContent(PlayGui);
   activateDirectInput();

   // Replace the splash screen with the main game window.
   closeSplashWindow();
   Window.canvas.showWindow();
}

// Start playing the game!
SimpleNetClient.connectTo(self);

// Allow us to exit the game...
GlobalActionMap.bind("keyboard", "escape", "quit");

// Called when the engine is shutting down.
function onExit() {
   SimpleNetClient.disconnect();
   GameGroup.delete();
}
