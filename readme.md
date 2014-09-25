# Targets

The goal of this project is to determine the minimum acceptable target size for objects selected by looking at them, in a headset.

This is exposed with a gameplay mechanic. A target is presented in a random location in the user's field of view. The user has to look at it and press a button to "shoot it". We vary:

- the size of the target
- the visual feedback to the user (a targeting reticle, lighting up the target, or no feedback)

Users play the game, shooting at 5 points at a time, then resting their heads. We measure:

- The time to acquire the target
- The accuracy rate.

We will use this data to determine the minimum appropriate target size.

### Gameplay Elements

You can collect data by forcing people to play the game, or by making it fun to play so people actually try it. We opt for the latter approach

### To Do

This file will be used in lieu of an issue tracker, and like most issue trackers will probably go out of date soon.

- Initial "Press to Recenter Display screen"
- Create a new object when the spacebar is pressed & user is looking at the object
- Effect to blow up the object when you hit spacebar and look at it.
- A way to measure misses with the spacebar
- A way to prevent the user from just holding down spacebar while looking around (accuracy penalty, or sample once every 50ms or something)
- Final Score/Leaderboard/Play Again screen
- Show the score on each quad (a number that decreases as time increases)
- Create a target looking object in Photoshop
- Anonymous data publisher
- Server side code to manage the data (maybe just dumping to text files at first)

### Installing/Building

I haven't actually tested this, but you should be able to load this folder as a project in Unity and run it. You may also need the Oculus plugin. The important code is in TargetScript.cs.
