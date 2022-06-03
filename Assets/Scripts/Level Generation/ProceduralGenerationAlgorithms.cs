using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    //Oyunun yap� ta�� olan RandomWalker algoritmas�:
    //Detaylar oyun raporuna bulunmaktad�r.
    //Ek bilgi i�in: https://en.wikipedia.org/wiki/Random_walk
    //RandomWalker odalar i�in �al��t�r�l�r.

    public static HashSet<Vector2Int> RandomWalkerFloor(Vector2Int startPosition, ScriptableObjectForRandomWalker parameters) // ko�u s�ras�nda at�lacak olan ad�mlar i�in rastgelelik
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;
        int walkLength = Random.Range(parameters.minWalkLength, parameters.maxWalkLength);

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();  // yeni bir konum se�
            path.Add(newPosition);                                                          // yeni konumu path listesine ekle
            previousPosition = newPosition;                                                 // g�ncel konumu, yani eski konumu yeni konuma e�itle
        }
        return path;
    }

    //RandomWalker koridorlar i�in �al��t�r�l�r.
    //Koridorlar da��n�k yap�da olmayaca��ndan dolay� ek bir fonksiyon ile olu�turulur.
    public static List<Vector2Int> RandomWalkerCorridor(Vector2Int startPosition, int corridorWalkLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection(); 
        var currentPosition = startPosition;
        corridor.Add(currentPosition);
        for (int i = 0; i < corridorWalkLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }

    //Tekrara giren koridorlar�n listeden silinmesi i�in kullan�lan statik method. DungeonGenerator
    //scripti i�erisinde kullan�m� a��klanm��t�r.
    public static List<List<Vector2Int>> RemoveDuplicates(List<List<Vector2Int>> listOfLists) {
        List<List<Vector2Int>> uniqueList = new List<List<Vector2Int>>();
        for (int i = 0; i < listOfLists.Count; i++) {
            var count = 0;
            var corridor = listOfLists[i];
            var corridorMiddle = corridor.ElementAt(corridor.Count / 2);
            for (int y = 0; y < listOfLists.Count; y++) {
                var check = listOfLists[y];
                var checkMiddle = check.ElementAt(check.Count / 2);
                if (corridorMiddle == checkMiddle) {
                    count++;
                }
            }
            if (count > 1) {
                if (uniqueList.Count == 0) {
                    uniqueList.Add(corridor);
                } else {
                    bool present = false;
                    for (int z = 0; z < uniqueList.Count; z++) {
                        var temp = uniqueList[z];
                        var tempMiddle = temp.ElementAt(temp.Count / 2);
                        if (corridorMiddle == tempMiddle) {
                            present = true;
                        }
                    }
                    if (!present) {
                        uniqueList.Add(corridor);
                    }
                }
            }
            if (count == 1) {
                uniqueList.Add(corridor);
            }
        }
        return uniqueList;
    }

    //Odalar�n ve koridorlar�n kesi�iminde bulunan t�m noktalar�n silinmesini sa�layan fonksiyon
    //Her bir koridor ve koridorun ba�l� oldu�u oda i�in �al���r. Bu sayede her bir koridor ile
    //Odan�n varsa kesi�tikleri noktalar silinir. Bu sayede duvar olu�umu s�ras�nda
    //Odalardan ��k���n engellenmemesi sa�lan�r.
    public static List<List<Vector2Int>> RemoveCorridorPositionsInRooms(List<List<Vector2Int>> corridorRoomCollisionsList, List<Corridor> corridorsInGameList) {
        for (int k = 0; k < corridorsInGameList.Count; k++) {
            var corridor = corridorsInGameList[k];
            corridorRoomCollisionsList.Add(new List<Vector2Int>());
            var corridorPositions = corridor.corridorPositions;
            int middle = corridorPositions.Count / 2;
            foreach (var room in corridor.corridorRooms) {
                bool f1 = false, f2 = false;
                var roomPositions = (room.transform.GetComponent(typeof(Room)) as Room).roomPositions;
                for (int i = middle - 1; i >= 0; i--) {
                    var current = corridorPositions[i];
                    for (int y = 0; y < roomPositions.Count; y++) {
                        if (!f1 && roomPositions.Contains(current)) {
                            for (int l = i; l >= 0; l--) {
                                corridorRoomCollisionsList[k].Add(corridorPositions[l]);
                            }
                            f1 = true;
                        }
                    }
                }
                for (int y = middle + 1; y < corridorPositions.Count; y++) {
                    var current = corridorPositions[y];
                    for (int z = 0; z < roomPositions.Count; z++) {
                        if (!f2 && roomPositions.Contains(current)) {
                            for (int l = y; l < corridorPositions.Count; l++) {
                                corridorRoomCollisionsList[k].Add(corridorPositions[l]);
                            }
                            f2 = true;
                        }
                    }
                }
            }
        }
        return corridorRoomCollisionsList;
    }
}

