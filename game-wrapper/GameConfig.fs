namespace GameWrapper

type GameConfig<'GameState> = {
    loadAssets: AssetInfo list
    initialState: 'GameState
    updateState: RunState -> 'GameState -> 'GameState
    getView: 'GameState -> TextInfo list * ImageInfo list
}