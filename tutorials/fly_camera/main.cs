displaySplashWindow("splash.bmp");

// Initialise audio, GFX, etc.
exec("lib/sys/main.cs");
Sys.init();

// Needed because we'll be acting as a local server, so we need both server
// and client functions defined.
exec("lib/simpleNet/client.cs");
exec("lib/simpleNet/server.cs");

singleton Material(BlankWhite) {
   detailMap[0] = "tutorials/fly_camera/white";
   detailScale[0] = "20 20";
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

function GameConnection::onEnterGame(%this) {
   %camera = new Camera() {
      datablock = Observer;
   };
   TheCamera.setTransform("0 0 2 1 0 0 0");
   %camera.scopeToClient(%this);
   %this.setControlObject(%camera);
   %this.add(%camera);
}

// Load console.
exec("lib/console/main.cs");

// Load HUD.
exec("tutorials/fly_camera/playGui.gui");

// Called on the client-side when we are first assigned a control object on the
// server (i.e. our camera).
function GameConnection::initialControlSet(%this) {
   // Activate HUD which allows us to see the game.
   PlayGui.noCursor = true;
   Canvas.setContent(PlayGui);
   activateDirectInput();

   // Replace the splash screen with the main game window.
   closeSplashWindow();
   Canvas.showWindow();

   new ActionMap(MoveMap);
   MoveMap.bindCmd("keyboard", "w", "$mvForwardAction = 1;",  "$mvForwardAction = 0;");
   MoveMap.bindCmd("keyboard", "s", "$mvBackwardAction = 1;", "$mvBackwardAction = 0;");
   MoveMap.bindCmd("keyboard", "a", "$mvLeftAction = 1;",     "$mvLeftAction = 0;");
   MoveMap.bindCmd("keyboard", "d", "$mvRightAction = 1;",    "$mvRightAction = 0;");
   MoveMap.bind("mouse", "xaxis", "yaw");
   MoveMap.bind("mouse", "yaxis", "pitch");
   MoveMap.push();
}

// Start playing the game!
SimpleNetClient.connectTo(self);

// Allow us to exit the game...
GlobalActionMap.bind("keyboard", "escape", "quit");

function yaw(%amount) {
   $mvYaw += %amount * 0.01;
}

function pitch(%amount) {
   $mvPitch += %amount * 0.01;
}

// Called when the engine is shutting down.
function onExit() {
   SimpleNetClient.disconnect();
   GameGroup.delete();
}
