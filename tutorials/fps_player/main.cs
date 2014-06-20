exec("./playGui.gui");

datablock CameraData(Observer) {};

singleton Material(PlayerMaterial) {
   diffuseColor[0] = "1 0 0";
   mapTo = "PlayerTexture";
};

singleton Material(BlankWhite) {
   detailMap[0] = "./white";
   detailScale[0] = "20 20";
};

datablock PlayerData(BoxPlayer) {
   shapeFile = "./player.dts";
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
   ServerConnection.delete();
   GameGroup.delete();
   MoveMap.delete();
}
