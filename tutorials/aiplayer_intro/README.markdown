# AIPlayer: an introduction

This tutorial will teach you the basics of the `AIPlayer` class, the base for non-player characters in Torque.
If you want to make side-scrolling or top-down games, `AIPlayer` is even a good choice for your player character.
It's slightly misnamed - it doesn't really do any AI - but it's very useful all the same.
We'll make a simple level where you can click the ground to make an `AIPlayer` move around.

This tutorial assumes you understand the basics of `Player` datablocks and the `Camera` object,
so go ahead and look at the [camera movement][02] and [player][03] tutorials if you're unfamiliar with either of those.
The code, as usual, is in the [`main.cs`](./main.cs) file.
Check it out!

 [02]: ../camera_movement
 [03]: ../fps_player

## Disclaimer: it's not really AI!

When you think of AI, you probably think of pathfinding, state machines, and so on.
Unfortunately, `AIPlayer` includes none of these.
What makes an `AIPlayer` distinct from a `Player` is that it cannot accept movement inputs directly from a client.
Instead, we use a couple of methods to direct it where to move and look, and we control its weapon firing and so on directly from scripts.
This means we can _control_ an `AIPlayer` with AI, even if there isn't any built-in.

## Adding an AIPlayer

Constructing an `AIPlayer` object is pretty simple.
We'll do it in our `onEnterGame` callback.
The code looks very similar to any other object spawn:

    new AIPlayer(ThePlayer) {
       datablock = BoxPlayer;
       position = "0 0 0";
    };
    GameGroup.add(ThePlayer);

We give the `AIPlayer` a datablock and a starting position, then add it to the `GameGroup` so we don't get a crash on exit (that's bad).

In this tutorial, I've decided to go with a static camera, similar to the setup in the base game code,
except rotated to face directly downwards, where the AIPlayer spawns.
Now we'll make this level a bit interactive.

## Making this level a bit interactive

The two most basic functions to use with an `AIPlayer` are `setMoveDestination` and `setAimLocation`.
They're pretty self-explanatory.
Given a world position, `setMoveDestination` will make the `AIPlayer` turn and move forwards towards that point.
`setAimLocation` will make the `AIPlayer` aim at a certain location in the world.
Excitingly, as you'll see in a minute, you can use both functions to have the `AIPlayer` aim at one location while moving somewhere else.

For now, we'll just get our character running to the spot we click.
To do this, we'll add an `onMouseDown` function to the `PlayGui`, the GUI control that renders the game world.
The function looks like this:

    function PlayGui::onMouseDown(%this, %screenPos, %worldPos, %vector) {
        ...
    }

The arguments to this function include:

 * `%this` is the GUI control itself
 * `%screenPos` is the x.y screen coordinates that were clicked
 * `%worldPos` is the location of the camera in the world
 * `%vector` tells us the world-space vector the click represents

The `%vector` parameter will be incredibly useful for us,
since it means we don't need to do lots of maths with the screen position, camera FOV,
etc to figure out where we are clicking.
We just need to extend the vector's length, then raycast along it from the camera's position.

## Raycasting

You can imagine a raycast as drawing a 3D line between two points, and stopping at the first thing you hit.
It's a very common operation in games, used for projectiles, measuring distances, testing line of sight, selecting things, and many more things.

So. Raycasts. First we construct the location we want to cast _to_.
A bit of vector maths later, and:

    %targetPos = VectorAdd(%worldPos, VectorScale(%vector, 1000));

And now we can actually perform the raycast:

    %hit = ContainerRayCast(%worldPos, %targetPos, $TypeMasks::StaticObjectType);

The function takes three arguments - start and end points, and a _typemask_.
This is just an integer value that specifies what sorts of objects we'd like to look for collisions with.
In this case, we only use the `StaticObjectType` mask, which means our ray will ignore _dynamic_ (non-static) objects like `Players`.

Now `%hit` will contain either the empty string, or a bunch of words separated by spaces.
From the [documentation][containerraycast], this string will look something like:

    "objectID hitX hitY hitZ normalX normalY normalZ"

So we can extract, for example, the position that our raycast collided with the world by doing:

    %hitPos = getWords(%hit, 1, 3);

## Finally, some action!

Now that we have the click location in the world, we can actually send our `AIPlayer` there!

    ThePlayer.setMoveDestination(%hitPos);

Well that was a bit anticlimactic.

## Adding finesse

Browsing the code, you'll notice I've added a second handler to the game - if you right-click,
the `AIPlayer` will aim at the location you clicked, even if you then left-click to move it somewhere.
You can make the `AIPlayer` stop looking at that point by right-clicking the `AIPlayer` itself.

The right-click code is very similar to the left-click code.
Obviously it goes in a new function called `onRightMouseDown`, and it works a little differently
to handle the case where we click on the `AIPlayer`.
First thing, we need to include the `ShapeBaseObjectType` mask in our query, so that our ray can actually hit `Player`s (which are a subclass of `ShapeBase`).
Then, when we get a hit, we need to check whether the hit object is our player.
It looks a little like this:

    if(%hit) {
        %hitObj = getWord(%hit, 0);
        %hitPos = getWords(%hit, 1, 3);
        if(%hitObj.name $= ThePlayer) {
            ThePlayer.clearAim();
        } else {
            ThePlayer.setAimLocation(%hitPos);
        }
    }

That snippet introduces the `clearAim` method, which tells the `AIPlayer` to stop aiming at a particular spot -
instead, it will just look in the direction it's moving.

## In summary

That's our siple demo application complete!
Now go make your `AIPlayer` minions do something more entertaining.

 [containerraycast]: http://docs.garagegames.com/torque-3d/reference/group__Game.html#ga398b62ea736cbae539a20bcd0bcdd2ac
