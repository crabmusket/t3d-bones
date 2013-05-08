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

// Display a splash window immediately to improve app responsiveness before
// engine is initialized and main window created
displaySplashWindow("art/images/splash.bmp");

// Console does something.
setLogMode(2);
// Disable script trace.
trace(0);

//-----------------------------------------------------------------------------
// Load up scripts to initialise subsystems.
exec("sys/main.cs");

// We need some of the default GUI profiles in order to get the canvas and
// other aspects of the GUI system ready.  
exec("gui/profiles.cs");

// Initialization of the various subsystems requires some of the preferences
// to be loaded... so do that first.  
exec("scripts/globals.cs");

// The canvas needs to be initialized before any gui scripts are run since
// some of the controls assume that the canvas exists at load time.
createCanvas("T3Dbones");

// Start rendering and stuff.
initRenderManager();
initLightingSystems("Basic Lighting"); 

// Start PostFX. If you use "Advanced Lighting" above, uncomment this.
//initPostEffects();

// Start audio.
sfxStartup();

// Load UI profiles and scripts.
exec("gui/console.gui");
exec("gui/hudlessGui.gui");

//-----------------------------------------------------------------------------
// Called when we connect to the local game.
function GameConnection::onConnect(%client) {
   %client.transmitDataBlocks(0);
}

// Called when all datablocks from above have been transmitted.
function GameConnection::onDataBlocksDone(%client) {
   // Start sending ghosts to the client.
   %client.activateGhosting();
   // Create a camera for the client.
   %c = spawnObject(Camera, Observer);
   GameCleanup.add(%c);
   %c.scopeToClient(%client);
   %client.setControlObject(%c);
   // Activate HUD which allows us to see the game.
   Canvas.setContent(HudlessPlayGui);
   activateDirectInput();
}

//-----------------------------------------------------------------------------
// Create a datablock for the observer camera.
datablock CameraData(Observer) {
   mode = "Observer";
};

// Create a local game server and connect to it.
new SimGroup(ServerGroup);
new GameConnection(ServerConnection);
// This calls GameConnection::onConnect.
ServerConnection.connectLocal();

// Create objects in the game!
new SimGroup(GameCleanup);
new SimGroup(GameGroup) {
   singleton Material(BlankWhite) {
      diffuseMap[0] = "art/images/white";
      mapTo = "white";
   };
   new LevelInfo(theLevelInfo) {
      canvasClearColor = "0 0 0";
   };
   new GroundPlane(theGround) {
      Position = "0 0 0";
      Material = "BlankWhite";
   };
   new Sun(theSun) {
      azimuth = "230.396";
      elevation = "45";
      color = "0.968628 0.901961 0.901961 1";
      ambient = "0.078431 0.113725 0.156863 1";
      castShadows = "1";
   };
};

// Create some keybinds for the console and to exit.
GlobalActionMap.bind(keyboard, "tilde", toggleConsole);
GlobalActionMap.bind(keyboard, "escape", quit);

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onExit() {
   echo("GOODBYE.");

   // Delete the objects we created.
   GameCleanup.delete();
   GameGroup.delete();

   // Delete the connection if it's still there.
   ServerConnection.delete();
   ServerGroup.delete();

   // Delete all the connections:
   while (ClientGroup.getCount()) {
      %client = ClientGroup.getObject(0);
      %client.delete();
   }

   // Delete all the data blocks...
   deleteDataBlocks();
}
