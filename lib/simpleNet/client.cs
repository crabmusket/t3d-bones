new ScriptObject(SimpleNetClient) {
   lightingMode = "Advanced Lighting";
   windowTitle = "t3d-bones";
   port = 7001;
};

function SimpleNetClient::init(%this) {
   // Load up scripts to initialise subsystems. Must be done before creating
   // the canvas, since it creates profiles which the canvas needs.
   exec("lib/sys/main.cs");

   // The canvas needs to be initialized before any gui scripts are run since
   // some of the controls assume that the canvas exists at load time.
   createCanvas(%this.windowTitle);

   // Start rendering and stuff. Must be done after creating the canvas.
   initRenderManager();
   initLightingSystems(%this.lightingMode);
   sfxStartup();

   return %this;
}

function SimpleNetClient::connectTo(%this, %host, %port) {
   %this.connection = new GameConnection();

   if (%host $= self) {
      %this.connection.connectLocal();
   } else {
      setNetPort(%this.port);
      %this.connection.connect(%host @ (%port !$= "" ? (":" @ %port) : ""));
   }

   return %this;
}

function SimpleNetClient::destroy(%this) {
   if (%this.connection) {
      %this.connection.delete();
   }
   %this.delete();
}
