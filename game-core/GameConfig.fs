namespace GameCore

type GameConfig<'GameState> = {
    loadAssets: LoadableAsset list
    initialState: 'GameState
    updateState: RunState -> 'GameState -> 'GameState
    getView: 'GameState -> DrawableText list * DrawableImage list
}