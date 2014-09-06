//-----------------------------------------------------------------------------
// Create a datablock for the observer camera.
datablock CameraData(Observer) {};

//-----------------------------------------------------------------------------
// And a material to give the ground some colour (even if it's just white).
singleton Material(BlankWhite) {
   diffuseColor[0] = "White";
};

//-----------------------------------------------------------------------------
// Called when all datablocks have been transmitted.
function GameConnection::onEnterGame(%this) {
   // Create a camera for the client.
   new Camera(TheCamera) {
      datablock = Observer;
   };
   return;
   TheCamera.setTransform("0 0 2 1 0 0 0");
   // Cameras are not ghosted (sent across the network) by default; we need to
   // do it manually for the client that owns the camera or things will go south
   // quickly.
   TheCamera.scopeToClient(%this);
   // And let the client control the camera.
   %this.setControlObject(TheCamera);
   // Add the camera to the group of objects associated with this connection so
   // it's cleaned up when the client quits.
   %this.add(TheCamera);
}

function GameConnection::onLeaveGame(%this) {
}

//-----------------------------------------------------------------------------
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
   };
}

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onEnd() {
   // Delete the objects we created.
   GameGroup.delete();
}
