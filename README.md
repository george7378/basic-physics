# basic-physics
> Take a look at [this Android app](https://play.google.com/store/apps/details?id=kristianseng.swingball) for another fun demo of this project, and my [lunar landing simulator](https://play.google.com/store/apps/details?id=kristianseng.perilune) which shows it being applied to an arbitrary 3D terrain mesh. I've also made a [demo game concept](https://github.com/george7378/bazookoids) with driving physics and complex collisions, and a more interesting [zero gravity demo](https://github.com/george7378/weightless).

Basic Physics is an introductory rigid body physics simulation, designed to show a clean example of the minimal code for 3D rigid body dynamics. It uses MonoGame for graphical rendering.

The project is organised with the core platform-independent code placed in the **PhysicsCore** folder. Everything here can be re-used and ported to different operating systems by adding a relevant MonoGame platform project to the solution. Right now, only a Windows desktop implementation is provided, located in the **PhysicsWindows** folder. Nothing here is relevant to the physics concepts; it's just a place where a Windows executable is created when the project is built.

The most important core component is the *PhysicsGame.cs* file. This is the central entry point where the MonoGame simulation and rendering loop happens. Even if you're new to 3D graphics or not familiar with MonoGame, the code here is separated into different regions with appropriate variable/function names and hints/comments.

There are two slightly different rigid body models included, both in the *Simulation* folder of the core code. The first is the most flexible, as it allows you to define a mass and inertia tensor for each object. The second model is stripped back to simulate symmetrical objects only, such as cubes and spheres. Mass is assumed to be 1 and the inertia tensor is assumed to be an identity matrix. This makes the maths simpler, and reduces the amount of information required to define a new object.

Note that the core project's *Content* folder contains assets such as models and shaders which are only relevant to the 3D graphics, and can be ignored if you're not interested in this aspect. This folder and its contents don't appear in the solution directly; rather they are referenced by each platform-specific project to allow the assets to be rebuilt for that specific OS.

The program shows two example objects falling under gravity and interacting with a flat plane. The thin plank is modelled with an inertia tensor using the first method, and the cube uses the stripped-back method for symmetrical objects. The collisions are demonstrated using a 'penalty method', which allows for easy modelling of impacts and friction without discontinuities.

![Objects resting](https://github.com/george7378/basic-physics/blob/master/_img/1.png)
