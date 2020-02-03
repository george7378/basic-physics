# basic-physics
> Take a look at [this Android app](https://play.google.com/store/apps/details?id=kristianseng.swingball) for another fun demo of this project, and my [lunar landing simulator](https://play.google.com/store/apps/details?id=kristianseng.perilune) which shows it being applied to an arbitrary 3D terrain mesh.

Basic Physics is an introductory rigid body physics simulation, designed to show a clean example of the minimal code for 3D dynamics. It uses MonoGame for graphical rendering.

There are two slightly different rigid body models included. The first is the most flexible, as it allows you to define a mass and inertia tensor for each object. When supplied with the cumulative force and torque, it can realistically integrate the motion from frame to frame. The second model is stripped back to simulate symmetrical objects only, such as cubes and spheres. Mass is assumed to be 1 and the inertia tensor is assumed to be an identity matrix. This makes the maths simpler, and reduces the amount of information required to simulate a new object.

The program shows two example objects falling under gravity and interacting with a flat plane. The thin plank is modelled with an inertia tensor using the first method, and the cube uses the stripped-back method for symmetrical objects. The collisions are demonstrated using a 'penalty method', which allows for easy modelling of impacts and friction without discontinuities.

![Objects resting](https://github.com/george7378/basic-physics/blob/master/_img/1.png)
