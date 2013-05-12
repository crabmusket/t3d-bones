# Camera movement

In this tutorial, we'll extend our knowledge of Torque's input system, using the mouse and keyboard to fly the camera around.
This tutorial introduces an important idea: Torque's move manager.

To load the completed tutorial in `ted-bones`, change `exec("game/main.cs");` in [`main.cs`][main.cs] to `exec("tutorials/02_camera_movement/main.cs");`.

 [main.cs]: ../../main.cs

## Advanced keybinds

In tutorial `01_load_a_basic_level`, you used a keybind to allow you to close the game with the escape key.
Now, we'll dig into some more uses of the keybind system.
Like these, which you can add to the bottom of `onStart`:

    GlobalActionMap.bindCmd("keyboard", "w", "$mvForwardAction = 1;",  "$mvForwardAction = 0;");
    GlobalActionMap.bindCmd("keyboard", "s", "$mvBackwardAction = 1;", "$mvBackwardAction = 0;");
    GlobalActionMap.bindCmd("keyboard", "a", "$mvLeftAction = 1;",     "$mvLeftAction = 0;");
    GlobalActionMap.bindCmd("keyboard", "d", "$mvRightAction = 1;",    "$mvRightAction = 0;");

Theese binds, unlike the last one we wrote, use a function called `bindCmd`, which, instead of taking a single function name, takes two entire statements.
You can see the difference - they end in semicolons (as statements must in TorqueScript), and do things.
In this case, they modify some mysteriously-named global variables.

Let's run the game now, and see what happens...

The camera moves now!
Is this some form of black magic?
You might say so...

## The move manager (`$mv*`)

These global variables named `$mvSomething` are all to do with a built-in Torque mechanism, the move manager.
Many timer per second, Torque automatically gathers the values of these global variables from the client, and sends them to the server.
The server recieves them as a `Move`, and uses them to control objects.
These `Move` objects get sent to a client's _control object_, which you might remember we set in the `onEnterGame` method:

    %client.setControlObject(TheCamera);

That means when you set `$mvForwardAction` to some value, Torque automatically sends to the `Camera` you're in control of, which decides what to di with it - in this case, it moves forward!

## Mouse control

We've nearly got a working fly-through camera - just one more detail: the mouse!
We can handle the mouse like any other input binding, though in this case we can't use `bindCmd` - you'll see why:

    GlobalActionMap.bind("mouse", "xaxis", "yaw");
    GlobalActionMap.bind("mouse", "yaxis", "pitch");

These binds call the `picth` and `yaw` functions when the mouse moves.
Let's see those functions, then:

    function yaw(%amount) {
        $mvYaw += %amount * 0.01;
    }

    function pitch(%amount) {
        $mvPitch += %amount * 0.01;
    }

This reveals why we couldn't use `bindCmd` - we need the engine to give us an amount (the `%amount` parameter of each function) that the mouse just moved by.
There's no way to access this if we use `bindCmd` with a code fragment to execute.

These functions introduce two new `$mv` variables: pitch and yaw.
They do pretty much what they sound like - rotate the camera around the right (_x_) and up (_z_) axes respectively.

Nice work!

## And another thing

Just in case you're wondering - we could have used the regular `bind` for those keyboard controls.
Here's how:

    GlobalActionMap.bind("keyboard", "w", "forwards");

    function forwards(%down) {
        if(%down) $mvForwardAction = 1;
        else      $mvForwardAction = 0;
    }

What do you think `%down` - the argument passed to this function does?
Yeah - it's a boolean that's true when they key is pressed _down_, and false when it's released.
The `forwards` function is called twice for every tap of the key - once when you press it, and once when you release.

I chose to use `bindCmd` because it's more succinct in this case, and because we hadn't used it before!
