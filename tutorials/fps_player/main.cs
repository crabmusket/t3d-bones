exec("./playGui.gui");

datablock CameraData(Observer) {};

singleton Material(PlayerMaterial) {
   diffuseColor[0] = "Red";
   mapTo = "PlayerTexture";
};

singleton Material(Tee) {
   detailMap[0] = "./tee";
   detailScale[0] = "20 20";
};

datablock PlayerData(BoxPlayer) {
   shapeFile = "./candide.dae";
   cameraMaxDist = 5;
   jumpDelay = 0;
};

//-----------------------------------------------------------------------------
function GameConnection::onEnterGame(%client) {
   new Player(ThePlayer) {
      datablock = BoxPlayer;
      position = "0 0 1";
   };
   %client.setControlObject(ThePlayer);
   GameGroup.add(ThePlayer);

   Canvas.setContent(PlayGui);
   activateDirectInput();
}

//-----------------------------------------------------------------------------
function onStart() {
   new SimGroup(GameGroup) {
      new LevelInfo(TheLevelInfo) {
         canvasClearColor = "0 0 0";
      };
      new GroundPlane(TheGround) {
         position = "Black";
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
   MoveMap.bindCmd("keyboard", "w", "$mvForwardAction = 1;",  "$mvForwardAction = 0;");
   MoveMap.bindCmd("keyboard", "s", "$mvBackwardAction = 1;", "$mvBackwardAction = 0;");
   MoveMap.bindCmd("keyboard", "a", "$mvLeftAction = 1;",     "$mvLeftAction = 0;");
   MoveMap.bindCmd("keyboard", "d", "$mvRightAction = 1;",    "$mvRightAction = 0;");
   MoveMap.bindCmd("keyboard", "space", "$mvTriggerCount2++;", "$mvTriggerCount2++;");
   MoveMap.bindCmd("keyboard", "tab", "ServerConnection.setFirstPerson(!ServerConnection.isFirstPerson());", "");
   MoveMap.bind("mouse", "xaxis", "yaw");
   MoveMap.bind("mouse", "yaxis", "pitch");
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
