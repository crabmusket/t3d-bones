//-----------------------------------------------------------------------------
// Load up our main GUI which lets us see the game.
exec("./playGui.gui");

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
function GameConnection::onEnterGame(%client) {
   // Create a camera for the client.
   new Camera(TheCamera) {
      datablock = Observer;
   };
   TheCamera.setTransform("0 0 2 1 0 0 0");
   // Cameras are not ghosted (sent across the network) by default; we need to
   // do it manually for the client that owns the camera or things will go south
   // quickly.
   TheCamera.scopeToClient(%client);
   // And let the client control the camera.
   %client.setControlObject(TheCamera);
   // Add the camera to the group of game objects so that it's cleaned up when
   // we close the game.
   GameGroup.add(TheCamera);
   // Activate HUD which allows us to see the game. This should technically be
   // a commandToClient, but since the client and server are on the same
   // machine...
   Canvas.setContent(PlayGui);
   activateDirectInput();
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

   // Allow us to exit the game...
   GlobalActionMap.bind("keyboard", "escape", "quit");
}

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onEnd() {
   // Delete the objects we created.
   GameGroup.delete();
}
