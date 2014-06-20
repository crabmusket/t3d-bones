exec("./playGui.gui");
exec("./convex.cs");

setRandomSeed(getRealTime());

datablock CameraData(Observer) {};

singleton Material(BlankWhite) {
   diffuseColor[0] = "1 1 1";
};

function GameConnection::onEnterGame(%client) {
   new Camera(TheCamera) {
      datablock = Observer;
   };

   TheCamera.setTransform("0 0 4 1 0 0 0");
   setFOV(45);

   TheCamera.scopeToClient(%client);
   %client.setControlObject(TheCamera);
   GameGroup.add(TheCamera);

   Canvas.setContent(PlayGui);
   activateDirectInput();
}

//-----------------------------------------------------------------------------
// Called when the engine has been initialised.
function onStart() {
   // Create basic objects.
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

   // Make some shapes!
   %s = 20;
   for(%x = -%s; %x < %s; %x++) {
      for(%y = -%s; %y < %s; %y++) {
         // Calculate height using a sinusoidal noise function.
         %factor = getRandom(50)/100 + 1.8;
         %height = mAbs(mSin(%x/%factor) + mCos(%y/%factor));
         // Need to add the block to the GameGroup so it's cleaned up.
         GameGroup.add(block(
            // Add the block at the current coordinates.
            %x SPC %y SPC "0",
            // 1 metre per side.
            "1 1" SPC %height
         ));
      }
   }

   // Allow us to exit the game...
   GlobalActionMap.bind("keyboard", "escape", "quit");

   // Camera movement binds.
   new ActionMap(MoveMap);
   MoveMap.bindCmd("keyboard", "w", "$mvForwardAction = 1;",  "$mvForwardAction = 0;");
   MoveMap.bindCmd("keyboard", "s", "$mvBackwardAction = 1;", "$mvBackwardAction = 0;");
   MoveMap.bindCmd("keyboard", "a", "$mvLeftAction = 1;",     "$mvLeftAction = 0;");
   MoveMap.bindCmd("keyboard", "d", "$mvRightAction = 1;",    "$mvRightAction = 0;");
   MoveMap.bind("mouse", "xaxis", "yaw");
   MoveMap.bind("mouse", "yaxis", "pitch");
   MoveMap.push();
}

function yaw(%amount) { $mvYaw += %amount * 0.01; }
function pitch(%amount) { $mvPitch += %amount * 0.01; }

function onGhostAlwaysObjectReceived() {}

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onEnd() {
   ServerConnection.delete();
   // Delete the objects we created.
   GameGroup.delete();
}
