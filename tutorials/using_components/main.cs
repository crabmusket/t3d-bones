exec("./playGui.gui");

singleton Material(Crosses) {
   detailMap[0] = "./white";
   detailScale[0] = "20 20";
};

singleton BehaviorTemplate(CameraTemplate) {
   networked = true;
};

function CameraTemplate::onAdd(%inst) {
   %inst.owner.scopeToClient(%inst.client);
   %inst.scopeToClient(%inst.client);
   %inst.client.setCameraObject(%inst.owner);
}

//-----------------------------------------------------------------------------
function GameConnection::onEnterGame(%client) {
   new Entity(TheCamera) {
      position = "0 0 2";
      new CameraBehaviorInstance() {
         template = CameraTemplate;
         client = %client;
      };
   };
   GameGroup.add(TheCamera);

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
         material = Crosses;
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
