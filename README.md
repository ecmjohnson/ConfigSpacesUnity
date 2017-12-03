# ConfigSpacesUnity
Generating 3D configuration spaces from 2D robotic motion problems using Unity

## What?

This is a Unity project that generates a 3D configuration space in normalized
coordinates from the (x,y,\theta) coordinates of a 2D robotic motion problem.
Everything is very much brute force, with very little optimization.

## How?

Simply open the repo in Unity and you'll see the demo I've set up. You should be
able to generate a configuration space by playing the demo.

To create your own configuration space based on a new problem, simply attach the
`Generate` script to your 2D robot, configure the public variables and make sure
all your 2D obstacles are tagged as `Obstacles`. You should see the space be
generated rapidly (the `FixedUpdate` timestep is set very low).

## When?

This was made during a hackathon, after I'd gotten everyone setup with wifi. I
am still very much a Unity beginner, so take this with a truck of salt.
