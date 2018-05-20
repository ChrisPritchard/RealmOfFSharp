# Realm of Racket but with the exercises in F# rather than Racket

Built as a learning exercise, both for F# and for the used rendering tech of MonoGame + F# + CoreRT

Details on Realm of Racket can be found here: http://www.realmofracket.com/

The games were implemented in the following order:
- guessing-game-text
- guessing-game-ui
- snake
- orc-battle
- dice-of-doom

Each has increasing complexity, and from -ui onwards, involved a reference to the game-core project, which is an F# wrapper of MonoGame. As the games improved in depth, game-core was further refined to support more functionality in a consistent style.