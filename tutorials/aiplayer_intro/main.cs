exec("game/playGui.gui");

//-----------------------------------------------------------------------------
datablock CameraData(Observer) {};

datablock PlayerData(BoxPlayer) {
   shapeFile = "./player.dts";
};

//-----------------------------------------------------------------------------
singleton Material(BlankWhite) {
   diffuseColor[0] = "1 1 1";
};

singleton Material(PlayerMaterial) {
   diffuseColor[0] = "1 0 0";
   mapTo = "PlayerTexture";
};

//-----------------------------------------------------------------------------
function GameConnection::onEnterGame(%client) {
   new Camera(TheCamera) {
      datablock = Observer;
   };
   // Rotate around x axis by PI/2 to face the ground.
   TheCamera.setTransform("0 0 20 1 0 0 1.571");
   // Nicer FOV for small debugging windows.
   setFOV(45);

   TheCamera.scopeToClient(%client);
   %client.setControlObject(TheCamera);
   GameGroup.add(TheCamera);

   // Create a player for the client.
   new AIPlayer(ThePlayer) {
      datablock = BoxPlayer;
      position = "0 0 0";
   };
   // And make sure it's cleaned up!
   GameGroup.add(ThePlayer);

   Canvas.setContent(PlayGui);
   activateDirectInput();
}

//-----------------------------------------------------------------------------
// Called when you click anywhere in the play area.
function PlayGui::onMouseDown(%this, %screenPos, %worldPos, %vector) {
   // Construct the endpoint of our raycast by adding the camera's vector.
   %targetPos = VectorAdd(%worldPos, VectorScale(%vector, 1000));
   // Perform the raycast, hitting only static objects.
   %hit = ContainerRayCast(%worldPos, %targetPos, $TypeMasks::StaticObjectType);
   // ContainerRayCast returns "" if it doesn't hit anything.
   if(%hit) {
      // Extract the hit position from the result string.
      %hitPos = getWords(%hit, 1, 3);
      // Make the AIPlayer go to that location.
      ThePlayer.setMoveDestination(%hitPos);
   }
}

// Called when the right mouse button is clicked on the play area.
function PlayGui::onRightMouseDown(%this, %screenPos, %worldPos, %vector) {
   %targetPos = VectorAdd(%worldPos, VectorScale(%vector, 1000));
   %hit = ContainerRayCast(%worldPos, %targetPos,
      $TypeMasks::StaticObjectType | $TypeMasks::ShapeBaseObjectType);
   if(%hit) {
      // The first word in a raycast result is the object that was hit.
      %hitObj = getWord(%hit, 0);
      %hitPos = getWords(%hit, 1, 3);
      // We want to test whether we've clicked the player - so check the object's
      // name.
      if(%hitObj.name $= ThePlayer) {
         ThePlayer.clearAim();
      } else {
         ThePlayer.setAimLocation(%hitPos);
      }
   }
}

//-----------------------------------------------------------------------------
function onStart() {
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

   GlobalActionMap.bind("keyboard", "escape", "quit");
}

//-----------------------------------------------------------------------------
function onEnd() {
   ServerConnection.delete();
   GameGroup.delete();
}
