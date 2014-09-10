# Client and server scripting

Hello, and welcome to an exciting milestone for `t3d-bones`: it's got networking!
After this instalment, you'll be familiar with the basics of Torque's networking, and have learned how to make a simple multiplayer environment.
Until now, all `t3d-bones` tutorials have ignored the networking that is built into the engine in favour of simple single-player games.

This tutorial will build on work done in [fly_camera](../fly_camera), so go read that first.
It might also be helpful to be familiar with `t3d-bones`'s [engine startup sequence](../the_main_file), but everything relating specifically to networking will be covered here.

## The client/server model

Networking in Torque 3D is based on the client/server model, where the server is authoritative.
This means that one computer acts as a central 'source of truth' for game data, such as who is where and what state they're in.
This might be one of the players (if that player is the game's host), or it might be a dedicated server in the cloud.
When implemented properly, an authoritative server can prevent some kinds of cheating, since it can decide just how far to trust each client.

Torque applications can run in three ways:

 * As a dedicated server, where no graphics are displayed.
   Dedicated servers are often run on cloud hosting services, and have the advantage that they can dedicate all their processing resources to communicating with clients and updating the game state, without also having to update rendering, sound, etc.
 * As a dedicated client, which connects to a dedicated server.
   A dedicated client is responsible for all those parts of the game that directly touch the user - gathering input, rendering the game and UI, etc.
 * As a mixed client/server, where both parts of the game run on the same computer.
   This mode allows users to host their own game servers without needing to run a dedicated command-line server, but obviously requires more resources on the computer since it has to do server and client calculations itself.
   Note that this is how Torque handles single-player games: it creates a local game server that does not allow connections from outside.

These three configurations are well-illustrated in this diagram from the engine's [networking documentation](http://docs.garagegames.com/torque-3d/reference/group__Networking.html#_details).

 ![Diagram explaining the three network configurations](http://docs.garagegames.com/torque-3d/reference/images/networkingServerTypes.png)

On the left is a dedicated server with several dedicated clients.
In the middle is a mixed client/server with several dedicated clients connected to it.
And on the very right is another client/server with nobody else connected.

`t3d-bones` uses a mixed local client/server scaffolding to enable you to quickly write single-player games with little boilerplate.
However, if you actually want to create multiplayer games, you'll need to learn how the client and server work separately.
This tutorial will implement two separate applications, a dedicated server and a dedicated client, which can connect to each other to render a basic multiplayer scene.

## Structure

`t3d-bones` tutorials have generally all followed the same structure - there is a `main.cs` file next to the engine executable, which you edit to execute one of the `tutorials/*/main.cs` files as you wish.
However, that won't cut it this time.
This tutorial requires two custom root `main.cs` files to replace the one sitting beside the executable.
You'll find them in `tutorials/client_server/main.client.cs` and `tutorials/client_server/main.server.cs`.
For convenience, you should copy/paste them so they're right next to the engine executable (i.e. in the same folder as the `tutorials` folder and the usual `main.cs`).

Now that we have files not called `main.cs`, we have to tell the engine how to find them when we run it.
This often means running the engine from the command-line.
For example,

    "Torque 3D.exe" main.server.cs

will run the server, and using `main.client.cs` instead will run the client.

Left in the `tutorials/client_server/` directory are three script files: `config.cs`, `client.cs`, and `server.cs`.
We'll see exactly how these are used later, but for now, just know that they're just like our usual `tutorials/*/main.cs` files - they provide code specific to this tutorial, that you wouldn't always want to reuse in other applications.

## The server

Let's dive on into the server code, starting with the mainfile
