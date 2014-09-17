exec("lib/metrics/main.cs");

// Don't slow down when the window is unfocused.
$platform::backgroundSleepTime = 0;

// Called when the engine has been initialised.
function onStart() {
   // Connect to the dedicated server.
   new GameConnection(ServerConnection);
   if($game::argc > 2) {
      %port = $game::argv[2];
   } else {
      setRandomSeed(getRealTime());
      %port = getRandom($client::portMin, $client::portMax);
   }
   setNetPort(%port);
   ServerConnection.connect($server::host);
}

// Called when the server assigns us a control object for the first time.
function GameConnection::initialControlSet(%this) {
   closeSplashWindow();
   Canvas.showWindow();

   // Activate HUD which allows us to see the game. This should technically be
   // a commandToClient, but since the client and server are on the same
   // machine...
   PlayGui.noCursor = true;
   Canvas.setContent(PlayGui);
   activateDirectInput();
   toggleNetGraph();

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

// Called when our request to join a server is accepted.
function GameConnection::onConnectionAccepted(%this) {
}

// Called if there's an error...?
function GameConnection::onConnectionError(%this, %error) {
   error(goodbye SPC %error);
}

function GameConnection::setLagIcon(%this, %val) {
   error(%val);
}

// Called when the engine is shutting down.
function onEnd() {
   ServerConnection.delete();
}

// And a material to give the ground some colour (even if it's just white).
singleton Material(BlankWhite) {
   diffuseColor[0] = "White";
   mapTo = "BlankWhite";
};

singleton Material(PlayerMaterial) {
   diffuseColor[0] = "Red";
   mapTo = "PlayerTexture";
};

// Load up our main GUI which lets us see the game.
exec("./playGui.gui");
