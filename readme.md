# Targets

The goal of this project is to determine the minimum acceptable target size for
objects selected by looking at them, in a headset.

This is exposed with a gameplay mechanic. A target is presented in a random
location in the user's field of view. The user has to look at it and press a
button to "shoot it". We vary:

- the size of the target
- the visual feedback to the user (a targeting reticle, lighting up the target, or no feedback)

Users play the game, shooting at 5 points at a time, then resting their heads.
We measure:

- The time to acquire the target
- The accuracy rate.

We will use this data to determine the minimum appropriate target size.

### Reading

["Target Size Study for One-Handed Thumb Use on Small Touchscreen
Devices"][paper] is a good resource.

[paper]: http://www.twentymilliseconds.com/pdf/minimum-button-size.pdf

### To Do

This file will be used in lieu of an issue tracker, and like most issue
trackers will probably go out of date soon.

- Initial "Press to Recenter Display screen", and graphics which compute based on this setting
- Effect to blow up the object when you hit spacebar and look at it.
- Final Score/Leaderboard/Play Again screen
- Show the score on each quad (a number that decreases as time increases)
- Create a target-looking object in Photoshop
- Anonymous data publisher
- Server side code to manage the data (maybe just dumping to text files at first)

### Installing/Building

I haven't actually tested this, but you should be able to load this folder as a
project in Unity and run it. You may also need the Oculus plugin. The important
code is in TargetScript.cs.
