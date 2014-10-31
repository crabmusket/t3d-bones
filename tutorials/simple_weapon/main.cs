displaySplashWindow("splash.bmp");

// Initialise audio, GFX, etc.
exec("lib/sys/main.cs");
Sys.init();

// Needed because we'll be acting as a local server, so we need both server
// and client functions defined.
exec("lib/simpleNet/client.cs");
exec("lib/simpleNet/server.cs");

singleton Material(BlankWhite) {
   mapTo = "BlankWhite";
   diffuseColor[0] = "White";
};

singleton Material(PlayerMaterial) {
   diffuseColor[0] = "Red";
   mapTo = "PlayerTexture";
};

singleton Material(Tee) {
   detailMap[0] = "tutorials/fps_player/tee";
   detailScale[0] = "20 20";
};

singleton Material(GunMetal) {
   mapTo = "GunMetal";
   diffuseColor[0] = "Gray";
};

singleton Material(ProjectileMat) {
   mapTo = "ProjectileMat";
   diffuseColor[0] = "Green";
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
   //shapeFile = "tutorials/fps_player/player.dts";
   cameraMaxDist = 5;
   jumpDelay = 0;
};

datablock ShapeBaseImageData(Gun) {
   shapeFile = "tutorials/simple_weapon/gun.dae";
   eyeOffset = "0.5 0.5 -0.5";

   stateName[0] = "ready";
   stateTransitionOnTriggerDown[0] = "fire";

   stateName[1] = "fire";
   stateFire[1] = true;
   stateScript[1] = "onFire";
   stateTimeoutValue[1] = 0.25;
   stateTransitionOnTimeout[1] = "ready";
};

datablock ProjectileData(GunProjectile) {
   projectileShapeName = "./projectile.dae";
   armingDelay = 5000;
   isBallistic = true;
   gravityMod = 0.1;
};

function Gun::onFire(%this, %obj) {
   GameGroup.add(new Projectile() {
      datablock = GunProjectile;
      initialPosition = %obj.getMuzzlePoint(0);
      initialVelocity = VectorScale(%obj.getMuzzleVector(0), 5);
      sourceObject = %obj;
      sourceSlot = 0;
   });
}

function GunProjectile::onCollision(%this, %obj, %col) {
   error(%col.getClassName());
}

// When a client enters the game, the server assigns them a camera.
function GameConnection::onEnterGame(%this) {
   %player = new Player() {
      datablock = BoxPlayer;
      position = "0 0 1";
   };
   %this.setControlObject(%player);
   GameGroup.add(%player);

   // Give the player a weapon. Attach an image of Gun to slot 0.
   %player.mountImage(Gun, 0);
}

// Load console.
exec("lib/console/main.cs");

// Load HUD.
exec("tutorials/fps_player/playGui.gui");

// Called on the client-side when we are first assigned a control object on the
// server (i.e. our camera).
function GameConnection::initialControlSet(%this) {
   // Activate HUD which allows us to see the game.
   Canvas.setContent(PlayGui);
   activateDirectInput();

   // Replace the splash screen with the main game window.
   closeSplashWindow();
   Canvas.showWindow();

   new ActionMap(MoveMap);

   // Movement binds.
   MoveMap.bindCmd("keyboard", "w", "$mvForwardAction = 1;",  "$mvForwardAction = 0;");
   MoveMap.bindCmd("keyboard", "s", "$mvBackwardAction = 1;", "$mvBackwardAction = 0;");
   MoveMap.bindCmd("keyboard", "a", "$mvLeftAction = 1;",     "$mvLeftAction = 0;");
   MoveMap.bindCmd("keyboard", "d", "$mvRightAction = 1;",    "$mvRightAction = 0;");
   MoveMap.bindCmd("keyboard", "space", "$mvTriggerCount2++;", "$mvTriggerCount2++;");
   MoveMap.bindCmd("keyboard", "tab", "ServerConnection.setFirstPerson(!ServerConnection.isFirstPerson());", "");

   // Mouse binds.
   MoveMap.bind("mouse", "xaxis", "yaw");
   MoveMap.bind("mouse", "yaxis", "pitch");
   MoveMap.bindCmd("mouse", "button0", "$mvTriggerCount0++;", "$mvTriggerCount0++;");

   MoveMap.push();
}

function yaw(%amount) {
   $mvYaw += %amount * 0.01;
}

function pitch(%amount) {
   $mvPitch += %amount * 0.01;
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
