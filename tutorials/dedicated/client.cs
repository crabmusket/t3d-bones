//-----------------------------------------------------------------------------
// Load up our main GUI which lets us see the game.
exec("./playGui.gui");

//-----------------------------------------------------------------------------
// And a material to give the ground some colour (even if it's just white).
singleton Material(BlankWhite) {
   diffuseColor[0] = "White";
};

//-----------------------------------------------------------------------------
// Called when all datablocks have been transmitted.
function GameConnection::initialControlSet(%this) {
   closeSplashWindow();
   Canvas.showWindow();

   // Activate HUD which allows us to see the game. This should technically be
   // a commandToClient, but since the client and server are on the same
   // machine...
   Canvas.setContent(PlayGui);
   activateDirectInput();
}

//-----------------------------------------------------------------------------
// Called when the engine has been initialised.
function onStart() {
   // Connect to the dedicated server.
   new GameConnection(ServerConnection);
   setNetPort($server::port);
   ServerConnection.connect($server::host);
}

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onEnd() {
   ServerConnection.delete();
}
