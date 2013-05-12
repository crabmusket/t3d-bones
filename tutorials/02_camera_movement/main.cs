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

exec("game/hudlessGui.gui");

datablock CameraData(Observer) {};

singleton Material(BlankWhite) {
   detailMap[0] = "game/white";
   detailScale[0] = "20 20";
   mapTo = "white";
};

//-----------------------------------------------------------------------------
function GameConnection::onEnterGame(%client) {
   new Camera(TheCamera) {
      datablock = Observer;
   };
   TheCamera.setTransform("0 0 2 1 0 0 0");
   TheCamera.setMovementMode("Fixed");

   TheCamera.scopeToClient(%client);
   %client.setControlObject(TheCamera);
   GameGroup.add(TheCamera);

   Canvas.setContent(HudlessPlayGui);
   activateDirectInput();
}

//-----------------------------------------------------------------------------
// Swap between the three movement modes of the camera.
function toggleCameraMode() {
   switch$(TheCamera.mode) {
      case "Fly":
         TheCamera.setMovementMode("Orbit");
      case "Orbit":
         TheCamera.setMovementMode("Fixed");
      case "Fixed":
         TheCamera.setMovementMode("Fly");
   }
}

//-----------------------------------------------------------------------------
// Apply properties to the camera for a given movement mode.
function Camera::setMovementMode(%camera, %mode) {
   %camera.mode = %mode;
   switch$(%mode) {
      case "Fixed":
         TheCamera.setFlyMode();
         TheCamera.setTransform("0 0 2 1 0 0 0");

      case "Fly":
         TheCamera.setFlyMode();

      case "Orbit":
         TheCamera.setOrbitMode(0, "0 0 2 1 0 0 0", 5, 20, 10);
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

   GlobalActionMap.bind("keyboard", "tilde", "toggleConsole");
   GlobalActionMap.bind("keyboard", "escape", "quit");

   GlobalActionMap.bind("keyboard", "space", "toggleCameraMode");
   GlobalActionMap.bindCmd("keyboard", "w", "$mvForwardAction = 1;",  "$mvForwardAction = 0;");
   GlobalActionMap.bindCmd("keyboard", "s", "$mvForwardAction = -1;", "$mvForwardAction = 0;");
   GlobalActionMap.bindCmd("keyboard", "a", "$mvLeftAction = 1;",     "$mvLeftAction = 0;");
   GlobalActionMap.bindCmd("keyboard", "d", "$mvRightAction = 1;",    "$mvRightAction = 0;");
   GlobalActionMap.bind("mouse", "xaxis", "yaw");
   GlobalActionMap.bind("mouse", "yaxis", "pitch");
}

function yaw(%amount) {
   $mvYaw += %amount * 0.01;
}

function pitch(%amount) {
   $mvPitch += %amount * 0.01;
}

//-----------------------------------------------------------------------------
function onExit() {
   // Delete the objects we created.
   GameGroup.delete();

   // Delete the connection if it's still there.
   ServerConnection.delete();
   ServerGroup.delete();

   // Delete all the datablocks...
   deleteDataBlocks();
}
