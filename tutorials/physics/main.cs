exec("game/playGui.gui");

physicsInit();

//-----------------------------------------------------------------------------
datablock CameraData(Observer) {};

//-----------------------------------------------------------------------------
singleton Material(BlankWhite) {
   diffuseColor[0] = "White";
   mapTo = "BlankWhite";
};

singleton Material(CubeMat) {
   diffuseColor[0] = "Gold";
   mapTo = "CubeMat";
};

datablock PhysicsShapeData(PSCube)
{
   shapeName = "./cube.dae";

   mass = 0.5;
   massCenter = "0 0 0";      // Center of mass for rigid body
   massBox = "0 0 0";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2;                // Drag coefficient
   bodyFriction = 0.2;
   bodyRestitution = 0.1;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 5;       // Play SoftImpact Sound
   hardImpactSpeed = 15;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance
   minRollSpeed = 10;

   maxDrag = 0.5;
   minDrag = 0.01;

   triggerDustHeight = 1;
   dustHeight = 10;

   dragForce = 0.05;
   vertFactor = 0.05;

   normalForce = 0.05;
   restorativeForce = 0.05;
   rollForce = 0.05;
   pitchForce = 0.05;

   friction = 0.4;
   linearDamping = 0.1;
   angularDamping = 0.2;
   buoyancyDensity = 0.9;
   staticFriction = 0.5;
   restitution = 0.3;
};

//-----------------------------------------------------------------------------
function GameConnection::onEnterGame(%client) {
   new Camera(TheCamera) {
      datablock = Observer;
   };
   // Rotate around x axis by PI/2 to face the ground.
   TheCamera.setTransform("0 0 2 1 0 0 0");
   setFOV(70);

   TheCamera.scopeToClient(%client);
   %client.setControlObject(TheCamera);
   GameGroup.add(TheCamera);

   Canvas.setContent(PlayGui);
   activateDirectInput();
}

//-----------------------------------------------------------------------------
function onStart() {
   physicsInitWorld("server");
   physicsInitWorld("client");
   physicsStartSimulation("server");
   physicsStartSimulation("client");

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

   for(%i = 0; %i < 10; %i++) {
      GameGroup.add(new PhysicsShape() {
         datablock = PSCube;
         position = getRandom(-10, 10)/10
                SPC getRandom(-10, 10)/10 + 5
                SPC 2 * %i + 5;
      });
   }

   GlobalActionMap.bind("keyboard", "escape", "quit");
}

//-----------------------------------------------------------------------------
function onEnd() {
   physicsStopSimulation("client");
   ServerConnection.delete();
   physicsDestroyWorld("client");

   physicsStopSimulation("server");
   GameGroup.delete();
   physicsDestroyWorld("server");

   physicsDestroy();
}
