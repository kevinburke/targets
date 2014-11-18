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

- Effect to blow up the object when you hit spacebar and look at it.
- Final Score/Leaderboard/Play Again screen
- Create a target-looking object in Photoshop
- Add "distractor" targets around the red target

### Installing/Building

I haven't actually tested this, but you should be able to load this folder as
a project in Unity and run it. You may also need the Oculus plugin (version
0.4.3). The important game logic is in TargetScript.cs.

## API

Here is the API for posting data to the server. Data is posted after five
single-target rounds and one multi-input round.

```json
{
    "single_input_trials": [
        {
            "time": 0.5,
            "hit": false,
            "camera_rotation": [4.5, 4, 89],
            "misses": 3
        },
        ...
    ],
    "multi_input_trial": [
        {
            "target": "4",
            "time": 0.5,
            "hit": false,
            "camera_rotation": [4.5, 4, 89]
        }
        ...
    ]

}
```
