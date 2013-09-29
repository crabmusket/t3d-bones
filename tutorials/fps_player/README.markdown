# Adding a player

In this instalment, we'll add one of the core elements of many games: the player character!
This tutorial builds directly on the work done in [fly_camera](../fly_camera).
As always, you can load this script up in `t3d-bones` by changing `exec("game/main.cs");` to `exec("tutorials/03_add_a_player/main.cs");` in [`main.cs`](../../main.cs).
Let's get started.

## The PlayerData block

The first thing we need to do to create a player is define the datablock the player will use.
We can do this anywhere in the file, really, but I like to put the datablocks at the top.
This one looks like:

    datablock PlayerData(BoxPlayer) {
        shapeFile = "./player.dts";
        cameraMaxDist = 5;
        jumpDelay = 0;
    };

There are a few basic properties in there, ones I think make the object a bit more usable than the defaults.
The most important one, and the only member that is compulsory, is the `shapeFile`.
This gives the Player an actual physical appearance in the game.
Speaking of appearances, let's take this opportunity to define a material for our player as well.

    singleton Material(PlayerMaterial) {
         diffuseColor[0] = "1 0 0";
         mapTo = "PlayerTexture";
    };

We haven't used `mapTo` in Materials before.
It basically means that wherever Torque finds a part of a shape with the `"PlayerTexture"` name mapped to it, it will use this material.
Coincidentally, the `player.dts` file I provided uses this name!

Now, we just need to put this data to use!

## Move over, observer

Let's modify our `onEnterGame` function so the client starts off controlling a Player, not a Camera:

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

It looks pretty much the same, right?
Another cool thing is that our input bindings that use `$mv` can also stay the same!
That's right - the move manager doesn't care about where its data is going, it just wants your key presses.
We will add two extra binds, though:

    MoveMap.bindCmd("keyboard", "space", "$mvTriggerCount2++;", "$mvTriggerCount2++;");
    MoveMap.bindCmd("keyboard", "tab", "ServerConnection.setFirstPerson(!ServerConnection.isFirstPerson());", "");

Now we'll be able to jump (and seriously, why would you play a game if you couldn't jump?) and change to a third-person view.
The `cameraMaxDist` property of the PlayerData block defined above affects this third-person view mode by bringing the floating camera a bit closer to the player.
You might be wondering why the coe relating to `$mvTriggerCount` is repeated - and, in fact, what on earth this variable even is.
The move manager has a bunch of _triggers_ meant for just that - triggering events.
It's typically used for things like firing weapons (which is where the name comes from), but in the case of the Player class, trigger 2 is used for jumping.

Our first instinct when thinking about this code might be to think that the trigger is set to 1 when the button (or key) is held down, and reset to 0 when the button is released.
However, this has the potential to miss very quick events, like the press and release of a key.
Instead, we increment the trigger count variables each time we press _or release_ a button (which is why the same script fragment is bound twice - to both press and release events!).
When the move manager sends its data to the server, it can count the number of times the trigger was pressed since the last update, and it knows whether the trigger is currently depressed - because it has been counting press and release events.

