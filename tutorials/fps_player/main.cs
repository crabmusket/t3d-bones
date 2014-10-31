displaySplashWindow("splash.bmp");

// Initialise audio, GFX, etc.
exec("lib/sys/main.cs");
Sys.init();

// Needed because we'll be acting as a local server, so we need both server
// and client functions defined.
exec("lib/simpleNet/client.cs");
exec("lib/simpleNet/server.cs");

datablock CameraData(Observer) {};

singleton Material(PlayerMaterial) {
   diffuseColor[0] = "Red";
   mapTo = "PlayerTexture";
};

singleton Material(Tee) {
   detailMap[0] = "tutorials/fps_player/tee";
   detailScale[0] = "20 20";
};

datablock PlayerData(BoxPlayer) {
   shapeFile = "tutorials/fps_player/player.dts";
   cameraMaxDist = 5;
   jumpDelay = 0;
};

new SimGroup(GameGroup) {
   new LevelInfo(TheLevelInfo) {
      canvasClearColor = "0 0 0";
   };
   new GroundPlane(TheGround) {
      position = "0 0 0";
      material = Tee;
   };
   new Sun(TheSun) {
      azimuth = 230;
      elevation = 45;
      color = "1 1 1";
      ambient = "0.1 0.1 0.1";
      castShadows = true;
   };
};

//-----------------------------------------------------------------------------
function GameConnection::onEnterGame(%client) {
   new Player(ThePlayer) {
      datablock = BoxPlayer;
      position = "0 0 1";
   };
   %client.setControlObject(ThePlayer);
   GameGroup.add(ThePlayer);
}

exec("lib/console/main.cs");

exec("tutorials/fps_player/playGui.gui");

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
   MoveMap.bindCmd("keyboard", "space", "$mvTriggerCount2++;", "$mvTriggerCount2++;");
   MoveMap.bindCmd("keyboard", "tab", "ServerConnection.setFirstPerson(!ServerConnection.isFirstPerson());", "");
   MoveMap.bind("mouse", "xaxis", "yaw");
   MoveMap.bind("mouse", "yaxis", "pitch");
   MoveMap.push();
}

function yaw(%amount) {
   $mvYaw += %amount * 0.01;
}

function pitch(%amount) {
   $mvPitch += %amount * 0.01;
}

// Start playing the game!
SimpleNetClient.connectTo(self);

// Allow us to exit the game...
GlobalActionMap.bind("keyboard", "escape", "quit");

function onExit() {
   SimpleNetClient.disconnect();
   GameGroup.delete();
}
