# t3d-bones

A truly skeletal starter/learning project for [Torque 3D][]. Check out the [releases][] page for demos.

 [Torque 3D]: https://github.com/GarageGames/Torque3D

## About

`t3d-bones` provides an almost completely minimal example of how to start the T3D engine, load a level and create a camera to view it through.
Oh, and the console is still there, but removing it is as simple as removing one line of code.
The template has been structured as little as possible, but I found that moving it towards a modular format helped make the code understandable.
Therefore:

 * `main.cs` handles engine initialisation, loads up the console, and finally the base game.
 * All the console scripts and GUIs are defined in `console/`.
 * All game-related info (i.e. object creation) is defined in `game/`, which is also where the art lives.
 * System initialisation (i.e. graphics and sound) lives in `sys/`. Don't go in there. It's terrifying.

## Projects using t3d-bones

 * [vlrtt](https://github.com/eightyeight/vlrtt): a real-time tactics game with vim-like controls
 * [sspt](https://github.com/lukaspj/sspt): a side-scroller prototype

[Let me know](http://www.garagegames.com/account/profile/79478) if you use t3d-bones!

## How to get started

Want to start your own project based off `t3d-bones`?
Awesome!
To get started, you can download a zip of this repository with binaries from [the releases page][releases] (7MB).
If you're a `git` user, you can also just clone this repository somewhere:

    git clone git@github.com:eightyeight/t3d-bones.git myAwesomeGame

And if you don't want the tutorials, you can just delete the folder:

    rm -rf myAwesomeGame/tutorials

In the interests of keeping the repository small and focused on the scripts, there are no binary files included.
You can either copy them from your main T3D installation, or download one of the [releases][].
And you're ready to go!

 [releases]: https://github.com/eightyeight/t3d-bones/releases
