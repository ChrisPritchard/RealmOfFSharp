namespace GameWrapper

type AssetInfo = {
    key: string;
    assetType: AssetType;
    path: string
} and AssetType = | Font | Texture