# Open-PictoPull

PictoPull is a puzzle game released on [Steam](https://store.steampowered.com/app/1820790/PictoPull/) and [Switch](https://www.nintendo.com/us/store/products/pictopull-switch/) in 2023. It was developed in the Unity Engine by Joseph Feldmann under the Screen Smith Studios LLC. The Switch port was handled by [Bearded Ants](https://www.beardedants.com/). The game is heavily inspired by the [Pushmo](https://en.wikipedia.org/wiki/Pushmo) and [Crashmo](https://en.wikipedia.org/wiki/Crashmo) puzzle games by Nintendo. The Steam version of the game has 150 puzzles + a full level editor with Steam workshop support so users can create and share their own levels. Development was started on a sequel to PictoPull, called PictoFall, which would have been a similar style game focused on a new type of level called "Crash Levels". These are based on the puzzles found in Crashmo. The sequel was abandoned as I (Joseph Feldmann) lost interest and wanted to work on other projects instead. Instead of releasing a sequel I am open sourcing the project as much as I can so that others can experiment with it or create a sequel themselves. This open source version of PictoPull will be referred to as Open-PictoPull and has some differences from the full version for legal reasons


# How To Play/Build

1. Open the Project in Unity 2022.2.7f1
2. Open the scene titled "0-Open-PictoPull". This is the master scene which contains all of the game logic and menus. The other scenes in the project are basically just the 3D backgrounds that are loaded in based on the selected level.
3. Hit play button to ensure everything is working
4. The project is set up to use the [Multitarget Build System](https://github.com/voldien/UMultiTargetBuilder). I liked this system as it allowed me to easily manage and build multiple different build configurations at once, something not possible in older Unity versions by default. One side effect of this is that the default Unity build system WILL NOT WORK, you HAVE TO build via the Multitarget build system. This can now probably be replaced with Unity's improved build system if the project is upgraded to Unity 6. You can also remove the multitarget build system and then use the default build system with no issues.
5. To Build:
		- Go to "Edit->Project Settings->Multitarget Build Settings"
		- Choose an output directory
		- Create or select a build target
		- Press the "Build Targets" button
		
# Differences from released version
In order to open source the the project I have had to remove some artwork, code, and levels that I have the legal right to use in a game, but do not have the right to distribute as an open source project. I also removed some assets that could be legally distributed but require attribution, as I want to remove that and make the project require as little attribution as possible Here are some examples:

## Textures/Models for environments

In the original PictoPull the gameplay took place in 10 different 3D environments which mainly served as functionless backgrounds for the gameplay. Some of those backgrounds were built with third party assets, textures, and models that I cannot distribute/open source. I have removed all non distributable third party textures/models from the game. This means many of the environments are currently broken and glitchy looking. I have changed the games data files so that instead of loading these old broken environments a simple empty environment is now loaded.

## Character Model

The character model for the original game was [Walking Robot Guy](https://sketchfab.com/3d-models/walking-robot-guy-blockbench-animation-7190ff66cb3d4e729a2ab95aeb9e797f). I never liked this model so I replaced this model with the base Blockbench humanoid 3D Model.


## Steam Workshop Support

Unfortunately the steam workshop integration for the Steam version of PictoPull was made with a third party asset [Steam Workshop - Easy Steamworks Integration](https://assetstore.unity.com/packages/tools/integration/steam-workshop-easy-steamworks-integration-86189) I have removed this asset from the project. It will have to be re implemented if you want Steam workshop support.

## Certain levels removed

Certain levels from PictoPull were based on pixel art assets packs I had bought. I had the right to use them as levels but I cannot distribute the artwork as an open source project. These levels have been removed. It is only ~10 levels out of 100+.







<!--stackedit_data:
eyJoaXN0b3J5IjpbLTE0Nzc2NTUyOTAsOTYwNDEwNDY1XX0=
-->