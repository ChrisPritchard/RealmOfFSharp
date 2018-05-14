namespace GameCore

type KeyPath = {
    key: string
    path: string
}

type Loadable =
| Texture of KeyPath
| Font of KeyPath