# Load a basic level

This tutorial covers the very basics of creating objects in a level once the engine has started up.
This includes datablocks, materials, and as a bonus, key binds.
The contents of this tutorial are all in [main.cs][], and I advise following along.

 [main.cs]: ./main.cs

## The game script

For this tutorial, we'll be working in one script file, called `main.cs`.
It is the main script run when the engine starts up.
The engine executable actually looks for a `main.cs` file in the same directory it is - notice that this directory is two folders beneath the `Torque 3D.exe` executable file.
_That_ file contains a single line, which you can modify to run different tutorials - or you can copy and paste the `main.cs` file in _this_ directory over the top of that one.

Either way, the first thing the engine does when it starts is run this script, so we get to control everything that happens.
What we'll do in this tutorial is write a script that creates a basic world and displays it.
We'll use some simple libraries that come with t3d-bones to make this really easy.

Let's begin at the beginning.

## The splash window

Because the engine itself takes a few seconds to initialise, the first thing we'll do in `main.cs` is open a 'splash window', which just displays a single image.
It opens up really quickly and gives the user a fast response so they know something's happening.

    displaySplashWindow("splash.bmp");

Notice we give it the filename of the BMP image that's also in the game root directory, next to the executable.
If not prefixed with a `.`, paths in TorqueScript are always relative to the engine executable.

## Setting up the client

Now we come to the first of the small libraries we will load to take care of some of the boilerplate work of getting a game running.
Torque has an inherently client/server structure - even when you're running a game just on your own PC, it will emulate a networked game, though obviously it doesn't actually do any networking.
For that reason, the first module we'll load is the _client_, i.e. the local end of the game.
Clients take care of rendering, sound, user input, and the UI.

    exec("lib/simpleNet/client.cs");
    SimpleNetClient.init();

The first line loads the library script which creates the `SimpleNetClient` object, and the second line initialises the client.
This call creates a canvas (the game window), initialises graphics subsystems, and performs some other tasks.

Note that `init` uses some members of `SimpleNetClient` that you can tweak before calling `init`.
For example, to change the renderer from its default `Advanced Lighting` to the less-fancy `Basic Lighting`, you could instead write this:

    exec("lib/simpleNet/client.cs");
    SimpleNetClient.lightingMode = "Basic Lighting";
    SimpleNetClient.init();

THe game we will create today is designed to only run locally, without communicating with other games over the network.
However, because of this inherent client/server structure the engine always emulates, we need to make some server-side scripts available as well, because our local game will _act_ as both client and server.

    exec("lib/simpleNet/server.cs");

This library defines an object called `SimpleNetServer`, which works similarly to `SimpleNetClient`.
However, as we don't be doing any actual networking, we don't need to call any of its methods.

## Setting up the level

At this point, we're ready to create some objects that we'll see when we join the game.
These objects all get created in what's logically the _server_ half of the game.
Without further ado, here's a really basic setup for you to start working with:

    singleton Material(BlankWhite) {
       diffuseColor[0] = "White";
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

In TorqueScript, an object definition looks like this:

    new ObjectClass(ObjectName) {
        member = value;
    };

This creates an instance of the `ObjectClass` class named `ObjectName`, with a single member (value).
The script above has four examples: creating the `LevelInfo`, the `Sun`, and the `GroundPlane` - but you might not have notived the sneaky fourth one, the `SimGroup` called `GameGroup`.
Object declarations in TorqueScript can be nested.
If you nest objects inside a `SimGroup` or `SimSet`, they get added to that group/set when they're created.
We could have some this ourselves:

    new SimGroup(GameGroup);
    GameGroup.add(new LevelInfo(TheLevelInfo) {...});
    // and so on

But the nested syntax is more convenient.

There's similar syntax for defining the material you saw just before the object declarations, except instead of `new` we use the `singleton` keyword.
We use thid method to define the `Material`s that give the game objects colour and texture.
We'll see more of those in later tutorials, but for now, just know that this `Material` is what makes the ground white.

## Joining the game

Now, we've created all the objects that exist in the game world, but how do we view them?
If you start the engine with just this in your game script file, you won't see anything happen.
This is because of that client/server structure I keep mentioning.
To see our level, we have to connect to it as a _client_, and the _server_ has to assign us a `Camera` object to view the level through.

If you're familiar with MVC, you can think of the client as the View and Controller, and the server as the Model.
Without the View and Controller, there's no way of seeing or interacting with the Model.

Joining the game requires three new things to happen.
First, the server needs to know what to do when a client wants to join the game.
Second, the client needs to know what to do when the server accepts its connection.
And last but not at all least, the client needs to ask for a connection!

Let's dive in to the server first.

### The onEnterGame function

In the `simpleNet` library, a client's connection request is always accepted, so we don't need to worry about the sequence of events that happens before the client actually gets into the game.
(We'll cover that, too, in later tutorials!)
We can fast-forward straight to the good bit: the client has asked to join, been accepted, received some data from the server about the current state of the game, and is now ready to enter the level.
Each client connected to a server (only one in this case) is represented by a `GameConnection` object.
The `onEnterGame` function is called on that object when everything's set up and ready to go, so let's define what happens when it's called:

    datablock CameraData(Observer) {};

    function GameConnection::onEnterGame(%this) {
       new Camera(TheCamera) {
          datablock = Observer;
       };
       TheCamera.setTransform("0 0 2 1 0 0 0");
       TheCamera.scopeToClient(%this);
       %this.setControlObject(TheCamera);
       GameGroup.add(TheCamera);
    }

This creates another object, `TheCamera`, with a member we haven't seen yet: `datablock`.
Most objects you'll encounter in Torque have datablocks.
These special objects hold static information that doesn't change: for example, the shape file associated with a specific character class, or the power of a type of vehicle engine.
The datablock called `Observer` was created just above the function.
Its definition might look a little perplexing - it's actually empty!
Well, it's not totally empty - it has some members, and we prefer to leave them set to their default values.

With the two function calls after that, we allow the client who just connected to the game to know about the camera (cameras, by default, are hidden from clients - only visible to the server), and allow the client to control the camera.
Now, our client has eyes through which to see the game world... but they're not connected to anything!

Note that this code is pretty much guaranteed only to work in single-player.
Can you tell why?
It's because we gave the camera object a unique name, `TheCamera`.
This makes it easier to test and do stuff with the camera, because we always know how to refer to it.
But if we tried to do this when another client joined the game, we'd end up with a name collision, and that'd just be awkward (also it would break the game).
In later tutorials we'll rewrite this function to be a little more multiplayer-friendly.

### The initialControlSet function

Now for the second stage: responding on the client.
Just like the server has a `GameConnection` for every client connected to the game, the client has a single `GameConnection` representing its connection to the server.
When the server accepts the client's request to join, and gives it something to control (the line in `onEnterGame` where we called `%this.setControlObject(TheCamera)`), the client will get a callback.
Let's go ahead and define what happens!

    exec("tutorials/client_server/playGui.gui");

    function GameConnection::initialControlSet(%this) {
       Canvas.setContent(PlayGui);
       activateDirectInput();
    
       closeSplashWindow();
       Canvas.showWindow();
    }

First, we put the `PlayGui` into the `Canvas`.
Remember that the canvas is our game window.
`PlayGui` is a HUD defined in `playGui.gui` which we loaded just before defining the function.
After that we turn on input (not sure what this actually does), and then close that splash window that we opened way back at the top of `main.cs`.
Well, now that we have a control object, we're going to be seeing some stuff, so we can get rid of the splash window and make the canvas window visible!

### Making the connection

And now, finally, we're ready to start the local game connection that will make all of these things happen!
Ready?

    SimpleNetClient.connectTo(self);

Well, that was anticlimactic.
The `simpleNet` library takes care of most of the detail for us.
Now we can sit back and watch the game run!

## The onExit function

Unfortunately, you may have noticed that the game is a bit obnoxious at this stage: if you manage to shut it down by closing the window, it will crash!
This is because when the engine exits, all those objects we created are still hanging around, and funny stuff happens when you try to close the engine with objects remaining.
What we need to do is define another function that cleans up all those objects.

`onExit` to the rescue!
This function is called when the engine shuts down in response to the window being closed, or a scirpt calling `quit`.
Here we must delete all the objects we've created!

    function onExit() {
       GameGroup.delete();
       SimpleNetServer.destroy();
       SimpleNetClient.destroy();
    }

`SimGroup` is a special container class, sort of like a folder in a computer.
When you delete it, all the objects inside it are deleted, too - so we don't have to delete them all one by one!
We also take this moment to shut down the server and client.
This step is important!
They need to run some stuff when the game ends, too.

Now you should be able to run the game and close it without any crashing!

## Keyboard control

But let's go ahead and do one more thing: add some keyboard commands!
This will let us close the game easily, rather than having to alt-tab out and close it manually.
Add this to the end of the script:

    GlobalActionMap.bind("keyboard", "escape", "quit");

This function calls the `bind` _method_ of the `GlobalActionMap` object.
This is a special object of the `ActionMap` class that responds to keyboard and mouse (and any other device) inputs.
This function call _binds_ the `quit` function to be called when we press `escape` on the `keyboard`.
That causes `onExit` to be called, which is defined in `main.cs`, which then calls our `onEnd` function we defined above.

Now try running the engine, and hit escape - goodbye, Torque!

Congratulations - that's your first game level locked and loaded!
Now go and make something more interesting!
