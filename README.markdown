# t3d-bones

A truly skeletal starter/learning project for [Torque 3D][].

 [Torque 3D]: https://github.com/GarageGames/Torque3D

## About

`t3d-bones` provides an almost completely minimal example of how to start the T3D engine, load a level and create a camera to view it through.
Oh, and the console is still there, but removing it is as simple as removing one line of code.
The template has been structured as little as possible, but I found that moving it towards a modular format helped mae the code understandable.
Therefore:

 * `main.cs` handles engine initialisation, loads up the console, and finally the base game.
 * All the console scripts and GUIs are defined in `console/`.
 * All game-related info (i.e. object creation) is defined in `game/`, which is also where the art lives.
 * System initialisation (i.e. graphics and sound) lives in `sys/`. Don't go in there. It's terrifying.

## How to get started

Want to start your own project based off `t3d-bones`?
Awesome!
To get started, you can download a zip of this repository with binaries [from my DropBox][package] (5MB).
If you're a `git` user, you can also just clone this repository somewhere:

    git clone git@github.com:eightyeight/t3d-bones.git myAwesomeGame

And if you don't want the tutorials, you can just delete the folder:

    rm -rf myAwesomeGame/tutorials

In the interests of keeping the repository small and focused on the scripts, there are no binary files included.
You can either copy them from your main T3D installation, or download Empty.dll and Empty.exe [from DropBox][binaries] (4.5MB).
And you're ready to go!

 [package]: https://www.dropbox.com/s/e07civwvkvdjvd7/t3d-bones.zip 
 [binaries]: https://www.dropbox.com/s/6ggmqcps07ky5pi/t3d-bones-binaries.zip
