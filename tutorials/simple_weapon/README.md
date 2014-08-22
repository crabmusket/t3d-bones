# A simple FPS weapon

In this instalment, we'll learn how to make a simple mounted weapon for our FPS player.
This tutorial builds directly on the work done in [fps_player](../fps_player).
As always, you can load this script up in `t3d-bones` by changing `exec("game/main.cs");` to `exec("tutorials/simple_weapon/main.cs");` in [`main.cs`](../../main.cs).
Let's get started.

## The ShapeBaseImageData block

In Torque, weapons and other items held by characters are often represented by the `ShapeBaseImage` class.
The key thing to remember is that a `ShapeBaseImage` is not an actual object: it doesn't have an ID, or its own properties.
It's simply an image that's attached to another object.
What we'll do now is write a `ShapeBaseImageData` datablock which determines how the image looks and behaves _when it's mounted on another object_.
Here we go:

    datablock ShapeBaseImageData(Gun) {
       shapeFile = "./gun.dae";
       eyeOffset = "0.5 0.5 -0.5";
    };

Pretty easy so far!
What these lines do is declare a new datablock of class `ShapeBaseImageData`, like we've done many times before, and name it `Gun`.
We declare that the gun uses the shape file `gun.dae`, and is offset slightly from the player's eye.

Now, let's make use of this new datablock!
In `GameConnection::onEnterGame` (which, remember, is called when we connect to the local game instance), after we create a `Player`, let's mount the gun to them:

    ThePlayer.mountImage(Gun, 0);

Easy.
This tells the engine to display the image defined by the `Gun` datablock in slot `0`.
Most objects have 4 slots for images to be mounted in, `0` to `3`, so watch out!
Before we run the game, let's not forget to declare a material for our gun.
The material name I created `gun.dae` with is `GunMetal`, so:

    singleton Material(GunMetal) {
       mapTo = "GunMetal";
       diffuseColor[0] = "Gray";
    };

Now if you run the game, you should see a really terrible-looking gun to the lower-right of the screen!
Now let's make it interesting.

## Shooting stuff

Now we get to the good stuff.
We're going to make our gun spit out some projectiles when we click.
There are a couple of things we have to do to make that happen:

 1. Define a `ProjectileData` datablock to tell the engine how we want our projectiles to act
 2. Add a state machine to the `Gun` image datablock, to tell it when to fire
 3. Link the mouse click input to the `Gun` state machine.

Without further ado, in order:

### ProjectileData

Time to define another datablock in our game script:

    datablock ProjectileData(GunProjectile) {
       projectileShapeName = "./projectile.dae";
    };

Well, that was easy.
Note that, annoyingly, this datablock uses a different name to refer to its shape file.
We just have to remember that, I'm afraid.
At the moment, we don't want our projectile to do anything fancy, but do take a look at the [ProjectileData][] reference to get some ideas for other types of projectile behavior (like gravity, particles, and light).

And, as always, let's not forget to implement the material for the new shape file we just used:

    singleton Material(ProjectileMat) {
       mapTo = "ProjectileMat";
       diffuseColor[0] = "Green";
    };

Okay!
Now it's time do implement the weapon state machine.

### State machine

Torque 3D's `ShapeBaseImage` class includes a simple [state machine][] that handles different states the weapon can be in, like firing, reloading, or ready.
Let's add two states to our weapon: `ready` and `fire`, which have obvious purposes.
We'll transition to `fire` from `ready` when you click the mouse, and back again after 0.25 seconds.
Add this to the declaration of `Gun`, after the `eyeOffset` line:

       stateName[0] = "ready";
       stateTransitionOnTriggerDown[0] = "fire";

       stateName[1] = "fire";
       stateFire[1] = true;
       stateScript[1] = "onFire";
       stateTimeoutValue[1] = 0.5;
       stateTransitionOnTimeout[1] = "ready";

That all looks fairly simple, right?
Important thing to mention: `stateScript`.
This state property gives the name of a function to call when the weapon enters this state.
In our case, we want to do some script stuff when the weapon fires, so we'll ask it to call the `onFire` callback.
Let's go ahead and write that callback now!

    function Gun::onFire(%this, %obj) {
       GameGroup.add(new Projectile() {
          datablock = GunProjectile;
          initialPosition = %obj.getMuzzlePoint(0);
          initialVelocity = VectorScale(%obj.getMuzzleVector(0), 5);
          sourceObject = %obj;
       });
    }

This little chunk is pretty dense.
The most important thing to note is the arguments to the function: `%this` and `%obj`.
Remember how I said that `ShapeBaseImages` aren't real objects?
So what do these parameters refer to?
`%this` refers to the datablock, `Gun`.
`%obj` refers to the object that the image is currently mounted to - in this case, our player!

There are two important methods to note here: `getMuzzlePoint` and `getMuzzleVector`.
They're both called on `%obj` - the player whom the weapon image is mounted to.
`ShapeBase` provides these two methods which allow you to get the position and direction of an image's muzzle point.
The parameter (`0` in both cases) is the image slot to query - we use `0` since we mounted `Gun` to slot `0` earlier using `mountImage(Gun, 0)`.
These methods allow you to specify the muzzle point within an image's shape file by adding a node named `muzzlePoint`.

Setting a `Projectile`'s `sourceObject` tells it to ignore collision with a certain object, which in this case is the player.
In case the bullet is created inside the player's bounding box - we don't want them shooting themselves!

So, this little script creates a new `Projectile`, adds it to `GameGroup`, and defines some properties.
This is a bare-minimum projectile definition.
The datablock lets the `Projectile` access properties like which shape file to use, and the initial position and velocity tell it where to start, and where to go.
We're nearly there!
Now your gun will almost be functional - but at the moment, Torque isn't listening for mouse clicks.

### Registering clicks

Remember that in [fly_camera](../fly_camera) we used an `ActionMap` to capture keyboard actions, which the `MoveManager` then sent to the server to be enacted on our control object?
For movement we used `$mvForwardAction` and friends.
Torque uses 'triggers' for actions like mouse clicks (and, like in [fps_player](../fps_player), jumping):

    MoveMap.bindCmd("mouse", "button0", "$mvTriggerCount0++;", "$mvTriggerCount0++;");

That's asking Torque to increment `$mvTriggerCount0` every time we press or release the mouse.
It just so happens that trigger `0` is wired into mounted image triggers (this happens inside the engine source code), so when we press the mouse, our image's state machine will receive a 'trigger' input.
That causes transitions to happen in the state machine we defined above.

So now you can run your level, and you should be able to shoot some little green cubes!

## Oh, you want collisions?

There's one thing I've left out of the discussion so far.
What happens when the `Projectile` collides with something?
At the moment, nothing - it just disappears anticlimactically.
In a later tutorial we'll look at adding explosions and all sorts of fun stuff, but for now, you might like to know that you can make a script callback happen when the projectile collides:

    function GunProjectile::onCollision(%this, %obj, %col) {
       error(%col.getClassName());
    }

This will print the class name of the object the `Projectile` collides with in the console in red.
Not very exciting, I know, but handling damage or anything more exciting is a bit beyond the scope of this tutorial!
Fun fact: `onCollision` may be called multiple times if your projectile can bounce or ricochet.

## Taking it further

To find out more cool things to do with the classes we've covered today, check out the great documentation for the [ShapeBaseImageData][] and [ProjectileData][] classes.
Try making ballistic projectiles, or bouncy ones.
Add some destructible environment objects using Mike Hall's excellent [damage and destruction][] tutorial series.
Or could you even implement dual-wielding?
What would need to change?
Could you use the same datablock with the same `eyeOffset`, or would you need another datablock?
How could you [avoid][classes] writing `Gun::onFire` again for your new datablock?
(Hint: callbacks are also passed the slot of the image that triggers them. So we could have written `function Gun::onFire(%this, %obj, %slot) {...}`).

Till next time!

 [ShapeBaseImageData]: http://docs.garagegames.com/torque-3d/reference/classShapeBaseImageData.html#_details
 [ProjectileData]: http://docs.garagegames.com/torque-3d/reference/classProjectileData.html
 [state machine]: http://gameprogrammingpatterns.com/state.html
 [damage and destruction]: http://www.garagegames.com/community/blogs/view/21016
 [classes]: http://www.garagegames.com/community/forums/viewthread/62868/1#comment-461493
