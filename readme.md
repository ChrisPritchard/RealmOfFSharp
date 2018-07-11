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

## Hungry Henry / Distributed Guess my Number etc

The final games in the Realm of Racket book are about distributed programming. This is not an area that interests me for game development, at least at the moment, and unlike the other games the code required in F#/MonoGame is completely different than that used by Racket, so I haven't bothered to build implementations at the time of writing. I might come back and do them later, maybe.

## Note on development sequence

This project was the first developed after my original __Monogame F#__ template [here](https://github.com/ChrisPritchard/FSharpMonogameTemplate).

The next project developed after this, and using the lessons learned, was __Battleship__ [here](https://github.com/ChrisPritchard/Battleship).