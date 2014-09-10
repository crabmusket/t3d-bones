# Client and server scripting

Hello, and welcome to an exciting milestone for `t3d-bones`: it's got networking!
After this instalment, you'll be familiar with the basics of Torque's networking, and have learned how to make a simple multiplayer environment.
Until now, all `t3d-bones` tutorials have ignored the networking that is built into the engine in favour of simple single-player games.

This tutorial will build on work done in [fly_camera](../fly_camera), so go read that first.
It might also be helpful to be familiar with `t3d-bones`'s [engine startup sequence](../the_main_file), but everything relating specifically to networking will be covered here.

## The client/server model

Networking in Torque 3D is based on the client/server model, where the server is authoritative.
This means that some computer acts as a central 'source of truth' for game data, such as who is where and what state they're in.
This might be one of the players (if that player is the game's host), or it might be a dedicated server in the cloud.


