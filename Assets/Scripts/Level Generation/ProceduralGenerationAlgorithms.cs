using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    //Oyunun yapý taþý olan RandomWalker algoritmasý:
    //Detaylar oyun raporuna bulunmaktadýr.
    //Ek bilgi için: https://en.wikipedia.org/wiki/Random_walk
    //RandomWalker odalar için çalýþtýrýlýr.

    public static HashSet<Vector2Int> RandomWalkerFloor(Vector2Int startPosition, ScriptableObjectForRandomWalker parameters) // koþu sýrasýnda atýlacak olan adýmlar için rastgelelik
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;
        int walkLength = Random.Range(parameters.minWalkLength, parameters.maxWalkLength);

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();  // yeni bir konum seç
            path.Add(newPosition);                                                          // yeni konumu path listesine ekle
            previousPosition = newPosition;                                                 // güncel konumu, yani eski konumu yeni konuma eþitle
        }
        return path;
    }

    //RandomWalker koridorlar için çalýþtýrýlýr.
    //Koridorlar daðýnýk yapýda olmayacaðýndan dolayý ek bir fonksiyon ile oluþturulur.
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

    //Tekrara giren koridorlarýn listeden silinmesi için kullanýlan statik method. DungeonGenerator
    //scripti içerisinde kullanýmý açýklanmýþtýr.
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

    //Odalarýn ve koridorlarýn kesiþiminde bulunan tüm noktalarýn silinmesini saðlayan fonksiyon
    //Her bir koridor ve koridorun baðlý olduðu oda için çalýþýr. Bu sayede her bir koridor ile
    //Odanýn varsa kesiþtikleri noktalar silinir. Bu sayede duvar oluþumu sýrasýnda
    //Odalardan çýkýþýn engellenmemesi saðlanýr.
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

