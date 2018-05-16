module Hex

(* Utilities for hex tile representation and calculations
   Taken from the excellent guide here https://www.redblobgames.com/grids/hexagons/ *)

type Cube = {
    x: float; y: float; z: float
}

type Hex = {
    q: float; r: float
} with static member toCube hex = { x = hex.q; z = hex.r; y = -hex.q-hex.r }

type HexTop = | Pointy | Flat
type PointyTopDir = | East = 0 | NorthEast = 1 | NorthWest = 2 | West = 3 | SouthWest = 4 | SouthEast = 5
type FlatTopDir = | SouthEast = 0 | NorthEast = 1 | North = 2 | NorthWest = 3 | SouthWest = 4 | South = 5
type Dir = | PointyTop of PointyTopDir | FlatTop of FlatTopDir

let private cubeDirections = [
    { x = 1.; y = -1.; z = 0. }; { x = 1.; y = 0.; z = 1. }; { x = 0.; y = 1.; z = 1. }
    { x = -1.; y = 1.; z = 0. }; { x = -1.; y = 0.; z = 1. }; { x = 0.; y = -1.; z = 1. }; 
]

let private roundCube cube = 
    let rx = round cube.x
    let ry = round cube.y
    let rz = round cube.z

    let xdiff = abs (rx - cube.x)
    let ydiff = abs (ry - cube.y)
    let zdiff = abs (rz - cube.z)

    if xdiff > ydiff && xdiff > zdiff then
        { x = -ry-rz; y = ry; z = rz} 
    elif ydiff > zdiff then
        { x = rx; y = -rx-rz; z = rz}
    else 
        { x = rx; y = ry; z = -rx-ry}

let private sqrt3 = sqrt 3.0

type Cube with
    static member toAxial cube = { q = cube.x; r = cube.z }

    static member width cubeTop size = 
        match cubeTop with
        | Flat -> 2. * size
        | Pointy -> sqrt3 * size

    static member height cubeTop size = 
        match cubeTop with
        | Flat -> sqrt3 * size
        | Pointy -> 2. * size

    static member add target cube = { x = cube.x + target.x; y = cube.y + target.y; z = cube.z + target.z }

    static member neighbour dir cube = 
        let adj = 
            match dir with
            | PointyTop enm -> cubeDirections.[int enm]
            | FlatTop enm -> cubeDirections.[int enm]
        Cube.add adj cube
    
    static member neighbours cubeTop cube = 
        let asCubes valueArray =
            valueArray |> Array.map (fun i -> cubeDirections.[i]) 
            |> Array.toList |> List.map (Cube.add cube)
        match cubeTop with
        | Pointy -> System.Enum.GetValues(typeof<PointyTopDir>) :?> PointyTopDir [] |> Array.map int |> asCubes
        | Flat -> System.Enum.GetValues(typeof<FlatTopDir>) :?> FlatTopDir [] |> Array.map int |> asCubes

    static member distance target cube = 
        (abs(target.x - cube.x) + abs(target.y - cube.y) + abs(target.z - cube.z)) / 2.

    static member toPixel cubeTop size cube = 
        let hex = Cube.toAxial cube
        match cubeTop with
        | Flat -> 
            let x = size * (3./2. * hex.q)
            let y = size * (sqrt3/2. * hex.q  + sqrt3 * hex.r)
            x,y
        | Pointy ->
            let x = size * (sqrt3 * hex.q  +  sqrt3/2. * hex.r)
            let y = size * (3./2. * hex.r)
            x,y
       
    static member fromPixel cubeTop size (x,y) =
        match cubeTop with
        | Flat ->
            let q = (2.0/3.0 * x) / size
            let r = (-1.0/3.0 * x + sqrt3/3.0 * y) / size
            Hex.toCube { q = q; r = r } |> roundCube
        | Pointy ->
            let q = (sqrt3/3.0 * x - 1.0/3.0 * y) / size
            let r = (2.0/3.0 * y) / size
            Hex.toCube { q = q; r = r } |> roundCube

type Hex with
    static member width = Cube.width
    static member height = Cube.height
    static member add target hex = Hex.toCube hex |> Cube.add (Hex.toCube target) |> Cube.toAxial
    static member neighbour dir hex = Hex.toCube hex |> Cube.neighbour dir |> Cube.toAxial
    static member neighbours cubeTop hex = Hex.toCube hex |> Cube.neighbours cubeTop |> List.map Cube.toAxial
    static member distance target hex = Hex.toCube hex |> Cube.distance (Hex.toCube target)
    static member toPixel hexTop size hex = Hex.toCube hex |> Cube.toPixel hexTop size
    static member fromPixel hexTop size (x,y) = Cube.fromPixel hexTop size (x,y) |> Cube.toAxial