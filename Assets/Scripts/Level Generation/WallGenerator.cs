using System.Collections.Generic;
using UnityEngine;

//Oluþan her bir oda ve koridor için kenarlar ve köþelerde duvarlarý oluþturmak için kullanýlan Script.

public static class WallGenerator {
    //Oyun içerisindeki her bir koridor için bu koridorlarýn konumlarý içerisinde gezinerek
    //o an için seçilen koridor pozisyonunun aþaðý, yukarý, sað ve sol konumlarý herhangi
    //bir koridor veya oda tarafýndan kullanýlan bir konum deðilse orada duvar oluþturulur.
    //Oluþan her bir duvar koridorun duvar listesine eklenir. Koridorlarýn oda giriþlerinde
    //duvar oluþturulmaz.
    public static void FindCorridorWalls(List<Corridor> corridorsInGameList) {
        HashSet<Vector2Int> removeWalls = new HashSet<Vector2Int>();
        foreach (var corridor in corridorsInGameList) {
            foreach (var corridorPosition in corridor.corridorPositions) {
                foreach (var direction in Direction2D.allDirectionsList) {
                    var neighbourPosition = corridorPosition + direction;
                    foreach (var corridorRoom_ in corridor.corridorRooms) {
                        var corridorRoom = corridorRoom_.transform.GetComponent(typeof(Room)) as Room;
                        if (!corridor.corridorPositions.Contains(neighbourPosition)) {
                            corridor.corridorWallPositions.Add(neighbourPosition);
                        }
                        if (!corridor.corridorPositions.Contains(neighbourPosition) && corridorRoom.roomPositions.Contains(neighbourPosition)) {
                            removeWalls.Add(neighbourPosition);
                        }
                    }
                }
            }
        }
        foreach (var corridor in corridorsInGameList) {
            corridor.corridorWallPositions.ExceptWith(removeWalls);
        }
    }

    //Koridor duvarlarýnýn oluþmasýnda olduðu gibi her odada her bir pozisyonda kontrolleri yaparak
    //boþ alanlarda duvarlarý oluþturur. Odalarýn koridor çýkýþlarýnda duvarlar oluþmaz.
    public static void FindRoomWalls(List<Room> roomsInGameList) {
        HashSet<Vector2Int> removeWalls = new HashSet<Vector2Int>();
        foreach (var room in roomsInGameList) {
            foreach (var roomPosition in room.roomPositions) {
                foreach (var direction in Direction2D.allDirectionsList) {
                    var neighbourPosition = roomPosition + direction;
                    foreach (var roomCorridor_ in room.roomCorridors) {
                        var roomCorridor = roomCorridor_.transform.GetComponent(typeof(Corridor)) as Corridor;
                        if (!room.roomPositions.Contains(neighbourPosition)) {
                            room.roomWallPositions.Add(neighbourPosition);
                        }
                        if (!room.roomPositions.Contains(neighbourPosition) && roomCorridor.corridorPositions.Contains(neighbourPosition)) {
                            removeWalls.Add(neighbourPosition);
                        }
                    }
                }
            }
        }
        foreach (var room in roomsInGameList) {
            room.roomWallPositions.ExceptWith(removeWalls);
        }
    }
}
