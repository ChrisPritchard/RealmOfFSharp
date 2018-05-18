namespace GameCore
open Microsoft.Xna.Framework

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
| ColouredImage of Color * DrawImageInfo
| Text of DrawTextInfo
| ColouredText of Color * DrawTextInfo