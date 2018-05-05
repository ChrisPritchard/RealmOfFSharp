namespace GameCore

type LoadableAsset = {
    key: string;
    assetType: AssetType;
    path: string
} and AssetType = | Font | Texture