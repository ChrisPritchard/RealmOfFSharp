namespace GameWrapper

type GameConfig<'TState> = {
        loadAssets: AssetInfo list
        initialState: 'TState
        updateState: KeyboardInfo -> 'TState -> 'TState
        getView: 'TState -> TextInfo list
    }