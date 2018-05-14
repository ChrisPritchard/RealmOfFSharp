namespace GameCore

type DrawImageInfo = {
    assetKey: string
    destRect: int * int * int * int
    sourceRect: (int * int * int * int) option
}

type DrawTextInfo = {
    assetKey: string
    text: string
    position: int * int
    scale: float
}

type Drawable = 
| Image of DrawImageInfo
| Text of DrawTextInfo