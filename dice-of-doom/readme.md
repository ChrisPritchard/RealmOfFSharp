## Dice of Doom

Implementation of 10.1 Dice of Doom

Differs in a few ways from the racket version:
- Instead of keyboard commands for input, I use the mouse
- The renderer allows me to 'shade' hexes, which I use to show source and targets for attacks
- I am not using memoization, mainly because it doesnt really add much speed wise and is a little tricky to do in an immutable fashion

Not sure if I got the rules exactly right, but it seems to work reasonably well. Of note is the Hex.fs module, which contains a set of types and methods that are translated from the truly excellent tutorial here https://www.redblobgames.com/grids/hexagons/