# basic-physics
> Take a look at [this Android app](https://play.google.com/store/apps/details?id=kristianseng.swingball) for another fun demo of this project, and my [lunar landing simulator](https://play.google.com/store/apps/details?id=kristianseng.perilune) which shows it being applied to a 3D terrain mesh. I've also made a [demo game concept](https://github.com/george7378/bazookoids) with driving physics and more complex collisions.

Basic Physics is an introductory rigid body physics simulation, meant to show a clean example of the minimal code for 3D rigid body dynamics. It uses MonoGame for graphical rendering.

It includes two slightly different rigid body models:
* The first is the most flexible and lets you define a mass and inertia tensor for each object.
* The second is stripped back to simulate symmetrical objects only, such as cubes and spheres. Mass is assumed to be 1 and the inertia tensor is assumed to be an identity matrix, which makes defining new objects easier.

The repo is organised with the core platform-independent code in the **PhysicsCore** folder. Everything relevant to the physics simulation is defined here. This is referenced by the demo projects, each in their own platform-specific folder:

## Base demo

This shows two sample objects falling under gravity and interacting with a flat plane. The thin plank is modelled with an inertia tensor, and the cube uses the stripped-back method for symmetrical objects. The collisions are demonstrated using a 'penalty method', which allows for easy modelling of impacts and friction without discontinuities.

![Base demo](https://github.com/george7378/basic-physics/blob/master/misc/readme/1.gif)

## Weightless

This is a more interactive demo showing how cuboid objects behave while floating in zero gravity. The **R** key resets and randomises the simulation, while the **Arrow** and **W**/**S** keys add some pseudo-gravity by accelerating the reference frame.

![Weightless](https://github.com/george7378/basic-physics/blob/master/misc/readme/2.gif)

## Intermediate axis

A demonstration of the intermediate axis theorem, where an object with three distinct moments of inertia will be unstable when rotating about the axis with the middle value. Also called the Dzhanibekov effect or tennis racket theorem, it causes the object to flip alternately between facing forwards and backwards as it spins. After a while, it will settle into a spin around one of its more stable axes.

![Intermediate axis](https://github.com/george7378/basic-physics/blob/master/misc/readme/3.gif)

There are also some scripts included in the miscellaneous folder for use with [Blender](https://www.blender.org/), including one that will export an inertia tensor for a closed 3D mesh.
