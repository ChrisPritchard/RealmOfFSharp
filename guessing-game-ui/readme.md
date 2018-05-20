## Guess my Number - UI Version

Implementation of 5.4 Guessing Game with a GUI. 

Uses a monogame library I built that allows the game loop to be orchestrated.
By so doing, the game class (Program.fs) is completely immutable and almost entirely
(except for the Keys enum and a reference to the font asset) MonoGame agnostic