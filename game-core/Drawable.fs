namespace GameCore

type DrawableImage = {
    textureKey: string
    destRect: int * int * int * int
    sourceRect: (int * int * int * int) option
}

type DrawableText = {
    fontKey: string
    text: string
    position: int * int
    scale: float
}