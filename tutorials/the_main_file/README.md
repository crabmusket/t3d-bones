# The main file

This tutorial explains the contents of [`main.cs`][main.cs] - the flow of engine initialisation and so on.
If you're not interested in the engine startup routine, but just want to start making some cool stuff, skip right on over this tutorial!
Otherwise, read on...

This tutorial will take the form of an annotated walk through `main.cs`.
If you're experienced with Torque you probably won't find much here you couldn't get out of the comments.
But hey, you might like to hear it again, or without all the template scripts getting in the way!

 [main.cs]: ../../main.cs

## First things first

The very first thing we do in the mainfile is open a splash window.
This really isn't necessary, and it's barely visible when your app is as small as this one.
But hey, why not?

    displaySplashWindow("splash.bmp");

The next section of the file is dedicated to Torque's subsystems - rendering, lighting, sound, etc.
There's a _lot_ of script that goes into this process, and most of it is hidden in the `sys/` directory.
Fortunately, we only need to call a few of those functions to get a window up and running.

The first thing we do is execute the mainfile for the `sys/` directory, which then executes all the other relevant files.

    exec("sys/main.cs");

Now, those scripts have defined a function called `createCanvas`.
It's really just a big wrapper around a call to `new GuiCanvas(Canvas)`, but it handles setting the right video mode and so on.
We pass it just one argument - the name to put at the top of the window.

    createCanvas("T3Dbones");

Now that the canvas exists, we can initialise the renderer and lighting.
These are separate calls because the light manager has an option - whether to use basic or advanced lighting.
I usually choose to use basic lighting, because it loads and runs quicker - I'm not doing anything fancy!

    initRenderManager();
    initLightingSystems("Basic Lighting"); 

If you use advanced lighting, though, you'll have to enable PostFX as well.
Without it, advanced lighting can't really do anything.

    initPostEffects();

And finally, we can start up the audio subsystem.
This is a single function call:

    sfxStartup();

## Stub methods

Some functions are called by the engine's C++ code during engine loading, and expect script-side callbacks to be defined.
I provide a couple of function stubs (i.e. empty functions) because if we don't define them at all, they print warnings to the console.
But we don't want to actually use them, so we'll just define them as empty to stop the warnings.

    function onDatablockObjectReceived() {}
    function onGhostAlwaysObjectReceived() {}
    function onGhostAlwaysStarted() {}
    function updateTSShapeLoadProgress() {}

## The console

At this point, I load up the console scripts and GUIs.
These are all in their own folder, `lib/console/`, so you can get rid of them completely if you want, by omitting the following line:

    exec("lib/console/main.cs");

Note that currently, the console library adds a keybind to the `~` key.
If you want to modify that, you'll have to look at `lib/console/main.cs`.

The `lib/` folder is intended to contain 'libraries' us useful features - such as this console window, or several post-effect shaders that are distributed with the engine's, but which might not be used by everyone who wants to make a game with the engine.
Later tutorials will introduce the features of the libraries that come with `t3d-bones`.

## Game scripts

And, finally, it's time to load up our actual game scripts which create cameras and objects and other favourite things of mine.

    exec("game/main.cs");

If you follow this tutorial series, you'll no doubt see me mention you can change this line - point it to, for example, `"tutorials/02_camera_movement/main.cs"`.
This will make the engine load that 'game' instead.

## The client/server connection

Torque, funnily enough, is _always_ in the mood for multiplayer.
Even if you're creating a single-player app, the engine will internally be working with a client/server structure.
In `t3d-bones`, this isn't too apparent, except here.
The next few lines deal explicitly with the client/server interactions that Torque needs to load up a level successfully.
The first of them is the `onConnect` callback of the `GameConnection` class.

    function GameConnection::onConnect(%client) {
        %client.transmitDataBlocks(0);
    }

This function is called when a client connects to the game.
In a networked game, this may represent an actual client connection from the internet, but in our case, it's always a local connection from inside the engine.
Don't worry - no data is actually being transferred over the internet.
Torque just likes to pretend there is.

The function starts the datablock transmission to the client.
This basically copies all the server's datablocks (pieces of static information that specify object properties) to the client.
Again, over the internet this would actually do something useful - here, since the client and server share the same memory space, it's just a hassle.

_Note that some people have figured out a way to bypass this step. I haven't yet._

When all the datablocks have been transferred, the 'client' gets the `onDatablocksDone` callback:

    function GameConnection::onDataBlocksDone(%client) {
        %client.activateGhosting();
        %client.onEnterGame();
    }

This simply enables ghosting (synchronising objects across the network) and calls the `onEnterGame` function, which should be defined in game-specific scripts.
Now we can actually create the server and connect to it:

    new SimGroup(ServerGroup);
    new GameConnection(ServerConnection);
    ServerConnection.connectLocal();

This `ServerConnection` object is the one receiving all these `GameConnection::` callbacks.
Calling `connectLocal` is the critical piece that creates a short-circuit local server connection, and starts off the whole local networking shebang with `onConnect` which we defined above.
Have a look at the [client/server tutorial](../client_server) for a more in-depth look at networking, and how to actually set up clients and servers on separate machines.

Finally, all we need to do is call the startup routine defined by the game scripts:

    onStart();

And we're off!

## Ending the game

When someone calls the `quit()` function, the engine shuts down.
It calls the `onExit` function first, though, to give scripts a chance to clean themselves up.
To reduce the amount of boilerplate code modules need to write, I define the `onExit` function,
which cleans up the objects created in this file, as well as providing an `onEnd` hook, symmetrical with the call to `onStart` when the game first starts.

The first thing we ned to do `onExit` is delete the `ServerConnection` we created above.
This object holds all the client-side objects, 'ghosts', which mirror the server-side objects.
If we don't delete this, these ghosts become orphaned and cause memory leaks.

Then, after the game-specific cleanup defined in`onEnd`, we can clean up all the server-side data structures, which we do by deleting the `ServerGroup` and calling `deleteDataBlocks`.

    function onExit() {
       ServerConnection.delete();

       // Clean up game objects and so on.
       onEnd();

       // Delete server-side objects and datablocks.
       ServerGroup.delete();
       deleteDataBlocks();
    }

And that's it for your basic `main.cs`!
Now have a look at some other tutorials to see how to use the structure I've set up.
