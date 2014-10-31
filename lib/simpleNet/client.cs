new ScriptObject(SimpleNetClient) {
   lightingMode = "Advanced Lighting";
   windowTitle = "t3d-bones";
   port = 7001;
};

function SimpleNetClient::connectTo(%this, %host, %port) {
   %this.disconnect();
   %this.connection = new GameConnection();

   if (%host $= self) {
      %this.connection.connectLocal();
   } else {
      setNetPort(%this.port);
      %this.connection.connect(%host @ (%port !$= "" ? (":" @ %port) : ""));
   }

   return %this;
}

function SimpleNetClient::disconnect(%this) {
   if(isObject(%this.connection)) {
      %this.connection.delete();
   }
}

function SimpleNetClient::destroy(%this) {
   %this.disconnect();
   %this.delete();
}
