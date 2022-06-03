using System.Collections.Generic;
using UnityEngine;

//Olu�an her bir oda ve koridor i�in kenarlar ve k��elerde duvarlar� olu�turmak i�in kullan�lan Script.

public static class WallGenerator {
    //Oyun i�erisindeki her bir koridor i�in bu koridorlar�n konumlar� i�erisinde gezinerek
    //o an i�in se�ilen koridor pozisyonunun a�a��, yukar�, sa� ve sol konumlar� herhangi
    //bir koridor veya oda taraf�ndan kullan�lan bir konum de�ilse orada duvar olu�turulur.
    //Olu�an her bir duvar koridorun duvar listesine eklenir. Koridorlar�n oda giri�lerinde
    //duvar olu�turulmaz.
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

    //Koridor duvarlar�n�n olu�mas�nda oldu�u gibi her odada her bir pozisyonda kontrolleri yaparak
    //bo� alanlarda duvarlar� olu�turur. Odalar�n koridor ��k��lar�nda duvarlar olu�maz.
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
