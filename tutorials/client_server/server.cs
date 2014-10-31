exec("lib/simpleNet/server.cs");
if ($Game::argv[1] $= server) {
   SimpleNetServer.initDedicated().host(28001);
}

function createLevel() {
   new SimGroup(GameGroup) {
      new LevelInfo(TheLevelInfo) {
         canvasClearColor = "Black";
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
}

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
   // Add the camera sa s sub-object of this connection so it gets cleaned up
   // when we leave the game.
   %this.add(TheCamera);
}

function GameConnection::onLeaveGame(%this) {
}
