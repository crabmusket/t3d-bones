// Create a datablock for the observer camera.
datablock CameraData(Observer) {};

datablock PlayerData(BoxPlayer) {
   shapeFile = "tutorials/fps_player/player.dts";
   cameraMaxDist = 5;
   jumpDelay = 0;
};

function CubeDAE::onLoad(%this)
{
   %this.addSequence("ambient", "rise", 0, 156);
   %this.setSequenceCyclic("rise", true);
}

singleton TSShapeConstructor(CubeDAE)
{
   baseShape = "./cube.dae";
   forceUpdateMaterials = false;
};

datablock StaticShapeData(CubeShape) {
   shapeFile = "./cube.dae";
};

function CubeShape::onAdd(%this, %obj) {
   error(playing SPC %obj.playThread(0, "ambient"));
}

// Called by the mainfile when a client has connected to the game and is ready
// to take part!
function GameConnection::onEnterGame(%this) {
   // Create a camera for the client.
   %camera = new Player() {
      datablock = BoxPlayer;
   };
   %camera.setTransform("0 0 2 1 0 0 0");

   // Cameras are not ghosted (sent across the network) by default; we need to
   // do it manually for the client that owns the camera or things will go south
   // quickly.
   //%camera.scopeToClient(%this);
   // And let the client control the camera.
   %this.setControlObject(%camera);

   // Add the camera to the group of objects associated with this connection so
   // it's cleaned up when the client quits.
   %this.add(%camera);
}

// Clean stuff up, notify other clients that the client has left, etc. We don't
// need to delete the camera because we added it as a sub-object of the
// GameConnection, so it will be deleted when this connection is.
function GameConnection::onLeaveGame(%this) {
}

// Called when the engine has been initialised.
function onStart() {
   // Create objects!
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
      new StaticShape(Cube) {
         datablock = CubeShape;
         position = "0 5 1";
      };
   };
}

// Called when the engine is shutting down.
function onEnd() {
   // Delete the objects we created.
   GameGroup.delete();
}
