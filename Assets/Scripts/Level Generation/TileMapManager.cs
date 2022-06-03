using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Harita içerisinde oluþan yapýlarýn görsel olarak ekranda gözükmesi için
//Tilemap renderer ile boyamalar yapýlýr.


public class TileMapManager : MonoBehaviour {
    [SerializeField] private ScriptableObjectForRandomWalker roomParameters;
    [SerializeField] private ScriptableObjectForCorridorWalker corridorParameters;

    //Odalar ve duvarlarý boyanýr.
    public void PaintRooms(List<Room> roomsInGameList) {
        foreach (var room in roomsInGameList) {
            int random_ = Random.Range(0, 4);
            var tile = roomParameters.floorTiles[random_];
            room.floorTile = tile;
            PaintTiles(room.roomPositions, room.roomFloorTilemap, tile);
            PaintWalls(room, random_);
            PaintTiles(room.roomPositions, room.roomMaskTilemap, corridorParameters.floorTile);
        }
    }
    //Koridorlar ve duvarlarý boyanýr.
    public void PaintCorridors(List<Corridor> corridorsInGameList) {
        foreach (var corridor in corridorsInGameList) {
            PaintTiles(corridor.corridorPositions, corridor.corridorFloorTilemap, corridorParameters.floorTile);
            PaintWalls(corridorsInGameList);
            PaintTiles(corridor.corridorPositions, corridor.corridorMask, corridorParameters.floorTile);
        }
    }

    public void PaintWalls(Room room, int random_) {
         PaintTiles(room.roomWallPositions, room.roomWallTilemap, roomParameters.wallTiles[random_]);
        foreach (var pos in room.roomWallPositions) {
            Vector3 casterPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
        }
    }
    public void PaintWalls(List<Corridor> corridorsInGameList) {
        foreach (var corridor in corridorsInGameList) {
            PaintTiles(corridor.corridorWallPositions, corridor.corridorWallTilemap, corridorParameters.wallTile);
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tileMap, TileBase tile) {
        foreach (var position in positions) {
            PaintSingleTile(position, tileMap, tile);
        }
    }
    private void PaintSingleTile(Vector2Int position, Tilemap tileMap, TileBase tile) {
        var tilePosition = tileMap.WorldToCell((Vector3Int)position);
        tileMap.SetTile(tilePosition, tile);
    }
}
