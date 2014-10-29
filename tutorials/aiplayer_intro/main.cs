displaySplashWindow("splash.bmp");

// Needed for client-side stuff like the GUI.
exec("lib/simpleNet/client.cs");
SimpleNetClient.init();

// Needed because we'll be acting as a local server, so we need some server
// functions defined.
exec("lib/simpleNet/server.cs");
SimpleNetServer.init();

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

datablock PlayerData(BoxPlayer) {
   shapeFile = "tutorials/aiplayer_intro/player.dts";
};

singleton Material(PlayerMaterial) {
   diffuseColor[0] = "Red";
   mapTo = "PlayerTexture";
};

datablock CameraData(Observer) {};

function GameConnection::onEnterGame(%this) {
   %camera = new Camera() {
      datablock = Observer;
   };
   // Look downwards.
   %camera.setTransform("0 0 20 1 0 0 1.571");
   // Nicer FOV for small debugging windows.
   setFOV(45);

   %camera.scopeToClient(%this);
   %this.setControlObject(%camera);
   %this.add(%camera);

   // Create a player for the client. We give the object a name because we're
   // not going to worry about multiplayer, and assume only one person will ever
   // play this at a time - so we can give the object a unique name.
   %player = new AIPlayer(ThePlayer) {
      datablock = BoxPlayer;
      position = "0 0 0";
   };
   // And make sure it's cleaned up!
   %this.add(%player);
}

// Load console.
exec("lib/console/main.cs");

// Load HUD.
exec("tutorials/aiplayer_intro/playGui.gui");

// Called on the client-side when we are first assigned a control object on the
// server (i.e. our camera).
function GameConnection::initialControlSet(%this) {
   // Activate HUD which allows us to see the game.
   Canvas.setContent(PlayGui);
   activateDirectInput();

   // Replace the splash screen with the main game window.
   closeSplashWindow();
   Canvas.showWindow();
}

// Start playing the game!
SimpleNetClient.connectTo(self);

// Allow us to exit the game...
GlobalActionMap.bind("keyboard", "escape", "quit");

// Called when the engine is shutting down.
function onExit() {
   GameGroup.delete();
   SimpleNetClient.destroy();
   SimpleNetServer.destroy();
}

//-----------------------------------------------------------------------------
singleton Material(BlankWhite) {
   diffuseColor[0] = "White";
};

//-----------------------------------------------------------------------------
// Called when you click anywhere in the play area.
function PlayGui::onMouseDown(%this, %screenPos, %worldPos, %vector) {
   // Construct the endpoint of our raycast by adding the camera's vector.
   %targetPos = VectorAdd(%worldPos, VectorScale(%vector, 1000));
   // Perform the raycast, hitting only static objects.
   %hit = ContainerRayCast(%worldPos, %targetPos, $TypeMasks::StaticObjectType);
   // ContainerRayCast returns "" if it doesn't hit anything.
   if (%hit) {
      // Extract the hit position from the result string.
      %hitPos = getWords(%hit, 1, 3);
      // Make the AIPlayer go to that location. Note that this isn't
      // multiplayer-safe.
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

   // Allow us to exit the game...
   GlobalActionMap.bind("keyboard", "escape", "quit");
}

//-----------------------------------------------------------------------------
function onEnd() {
   GameGroup.delete();
}
