exec("tutorials/fps_player/playGui.gui");

datablock CameraData(Observer) {};

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

datablock PlayerData(BoxPlayer) {
   shapeFile = "tutorials/fps_player/player.dts";
   cameraMaxDist = 5;
   jumpDelay = 0;
};

datablock ShapeBaseImageData(Gun) {
   shapeFile = "./gun.dae";
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
};

function Gun::onFire(%this, %obj) {
   GameGroup.add(new Projectile() {
      datablock = GunProjectile;
      initialPosition = %obj.getMuzzlePoint(0);
      initialVelocity = VectorScale(%obj.getMuzzleVector(0), 5);
      sourceObject = %obj;
   });
}

function GunProjectile::onCollision(%this, %obj, %col) {
   error(%col.getClassName());
}

//-----------------------------------------------------------------------------
function GameConnection::onEnterGame(%client) {
   new Player(ThePlayer) {
      datablock = BoxPlayer;
      position = "0 0 1";
   };
   %client.setControlObject(ThePlayer);
   GameGroup.add(ThePlayer);

   // Give the player a weapon. Attach an image of Gun to slot 0.
   ThePlayer.mountImage(Gun, 0);

   Canvas.setContent(PlayGui);
   activateDirectInput();
}

//-----------------------------------------------------------------------------
function onStart() {
   new SimGroup(GameGroup) {
      new LevelInfo(TheLevelInfo) {
         canvasClearColor = "Black";
      };
      new GroundPlane(TheGround) {
         position = "0 0 0";
         material = Tee;
      };
      new Sun(TheSun) {
         azimuth = 230;
         elevation = 45;
         color = "White";
         ambient = "0.1 0.1 0.1";
         castShadows = true;
      };
   };

   // Allow us to exit the game...
   GlobalActionMap.bind("keyboard", "escape", "quit");

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

//-----------------------------------------------------------------------------
function onEnd() {
   GameGroup.delete();
   MoveMap.delete();
}
