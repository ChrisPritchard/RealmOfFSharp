namespace GameCore

type GameConfig<'GameState> = {
    loadAssets: LoadableAsset list
    initialState: 'GameState
    updateState: RunState -> 'GameState -> 'GameState
    getView: RunState -> 'GameState -> DrawableImage list * DrawableText list
}