//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

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
function onExit() {
   GameGroup.delete();
   MoveMap.delete();

   ServerConnection.delete();
   ServerGroup.delete();

   deleteDataBlocks();
}
