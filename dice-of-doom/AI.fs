module AI
open Model
open GameCore

let timeBetweenActions = 1000.

let aiMove (runState:RunState) gameModel moves =
    if runState.elapsed-gameModel.lastAIAction < timeBetweenActions then
        gameModel
    else gameModel