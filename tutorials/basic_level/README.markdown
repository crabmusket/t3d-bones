# Load a basic level

This tutorial covers the very basics of creating objects in a level once the engine has started up.
This includes datablocks, materials, and as a bonus, key binds.
It's pretty much based on the contents of the default [`game/main.cs`][game.cs], so if you're comfortable with all that code, you might want to skip ahead.

 [game.cs]: ../../game/main.cs

## The game script

For this tutorial, we'll be working in one script file, called `game/main.cs`.
It is _executed_ during the `t3d-bones` startup procedures in [`main.cs`][main.cs].
This means that the script file is loaded and interpreted.
Objects are created and functions are defined.

This file contains some information needed to start a _local server_ - more on that later - so this file is executed before the server is started.
However, it also contains some instructions that need to be run _after_ the server is created.
This is done by putting them in the `onStart` function.

 [main.cs]: ../../main.cs

## The onStart function

After Torque has finished loading itself up and creating a server for you, it will call the `onStart` function.
This isn't build into the engine itself - it's done at the end of [`main.cs`][main.cs].
You could easily call it something else, or remove it entirely.

## Creating objects

Let's take a look at a simple `onStart` function:

    function onStart() {
        new SimGroup(GameObjects) {
            new LevelInfo(TheLevelInfo) {
                canvasClearColor = "0 0 0";
            };
            new GroundPlane(TheGround) {
                position = "0 0 0";
                material = "BlankWhite";
            };
            new Sun(TheSun) {
                azimuth = 230;
                elevation = 45;
                color = "1 1 1";
                ambient = "0.1 0.1 0.1";
                castShadows = true;
            };
        };
    }

This function creates a bunch of objects.
A basic object definition looks like this:

    new ObjectClass(ObjectName) {
        member = value;
    };

This creates an _instance_ of the `ObjectClass` _class_, named `ObjectName`, with a single _member_.
A member is just a property of an object.
You can see some examples in the script above - for eample, the `TheSun` object (of class `Sun`) has members for its azimuth and elevation, as well as direct colour and ambient colour.

## The server and client

Now, this function will create all the objects that exist in the game world, but how do we view them?
If you start the engine with just this in your game script file, you won't see anything happen.
This is because Torque has an inherent _client/server_ structure.
To see our level, we have to connect to it as a _client_, and create a `Camera` object to view it through.

For more detail about the server creation and connection process, you can look through tutorial [`00_the_main_file`][00].
But for now, all you need to know is that the function `GameConnection::onEnterGame` is called when the client enters the server's game world.
The `::` in the name denotes a _namespace_.
This means the function belongs to objects of the `GameConnection` class, in a way.

 [00]: ../00_the_main_file

## The onEnterGame function

A good `onEnterGame` function might look like this:

    function GameConnection::onEnterGame(%client) {
        new Camera(TheCamera) {
            datablock = Observer;
        };
        TheCamera.scopeToClient(%client);
        %client.setControlObject(TheCamera);
        GameObjects.add(TheCamera);

        Canvas.setContent(HudlessPlayGui);
        activateDirectInput();
    }

This creates another object, `TheCamera`, with a member we haven't seen yet: `datablock`.
Most objects you'll encounter in Torque have datablocks.
These special objects hold static information that doesn't change: for example, the shape file associated with a specific character class, or the power of a type of vehicle engine.
This `Camera`-class object refers to a datablock called `Observer`, which we'll create a bit later.

With the two function calls after that, we allow the client who just connected to the game to know about the camera (cameras, by default, are hidden from clients - only visible to the server), and allow the client to control the camera.
Now, our client has eyes through which to see the game world... but they're not connected to anything!

The final part of the function displays a GUI object on the Canvas.
This GUI element is defined in `hudlessGui.gui` - but we'll take a look at it in a later tutorial.
For now, all we have to do is `exec("./hudlessGui.gui");` somewhere in our script file.
By displaying `HudlessPlayGui` in the window (called `Canvas` inside the engine), we'll now be able to see the game world!

## Datablocks and materials

Well, nearly.
We have made two deliberate omissions so far - one that I mentioned, and one that I didn't.
We did not create a datablock called `Observer` for the camera, and we also didn't define the material `BlankWhite` for the ground plane.
Let's do both of those now!

    datablock CameraData(Observer) {};

Wait, _what?_
This datablock is regrettably empty!
It's a funny quirk of Torque that many objects cannot be created without a datablock... even if that datablock doesn't contain any information!
Well, that's a lie - the datablock _does_ contain information - default values.
We just haven't chosen to change any of the defaults, so in our script file, the datablock looks empty.
And now for the material:

    singleton Material(BlankWhite) {
        diffuseColor[0] = "1 1 1";
    };

This very simple material declaration literally makes the entire object white (decimal RGB colour `(1, 1, 1)`).
Hurrah!
We now have enough information to actually go ahead and run the game!
Start 'er up!

## The onEnd function

Unfortunately, you may have noticed that the game is a bit obnoxious at this stage: if you manage to shut it down by closing the window, it will crash!
This is because when the engine exits, all those objects we created are still hanging around, and funny stuff happens when you try to close the engine with objects remaining.
What we need to do is define another function that cleans up all those objects.

`onEnd` to the rescue!
This function, like `onStart`, is called in `main.cs` when the engine shuts down.
So if you want to call it something else, you know where to look!

Given all the objects we created  in `onStart`, let's have a look at an `onEnd` function that might be suitable:

    function onEnd() {
        ServerConnection.delete();
        GameObjects.delete();
    }

`SimGroup` is a special container class, sort of like a folder in a computer.
When you delete it, all the objects inside itare deleted, too - so we don't have to delete them all one by one!

Right, now you should be able to run the game and close it without any crashing!

## Keyboard control

But let's go ahead and do one more thing: add some keyboard commands!
This will let us close the game easily, rather than having to alt-tab out and close it manually.
Add this to the end of the `onStart` function:

    GlobalActionMap.bind("keyboard", "escape", "quit");

This function calls the `bind` _method_ of the `GlobalActionMap` object.
This is a special object of the `ActionMap` class that responds to keyboard and mouse (and any other device) inputs.
This function call _binds_ the `quit` function to be called when we press `escape` on the `keyboard`.
That causes `onExit` to be called, which is defined in `main.cs`, which then calls our `onEnd` function we defined above.

Now try running the engine, and hit escape - goodbye, Torque!

Congratulations - that's your first game level locked and loaded!
Now go and make something more interesting!
