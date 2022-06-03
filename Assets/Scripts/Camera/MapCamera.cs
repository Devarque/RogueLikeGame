using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Minimap haritas�n�n odalar�n a��lmas� ile birlikte a��lan odalar� g�stermesini sa�lamak amac�yla olu�turulan script.
public class MapCamera : MonoBehaviour {
    public static MapCamera instance;
    public float size; 
    [SerializeField] private ScriptableObjectForCorridorWalker parameters;

    private void Awake() {
        instance = this;
    }

    //A��lan her oda ile birlikte haritan�n g�ncellenmesini sa�lar.
    private void Update() {
        transform.position = FindCenterOfTransforms(DungeonGenerator.instance.roomsInGameList);
        GetComponent<Camera>().orthographicSize = ((parameters.corridorWalkLength / 2) * FindMapHeight());
        size = GetComponent<Camera>().orthographicSize;
    }

    //Minimap ve ���nlama i�lemleri i�in kullan�lacak olan kameran�n haritan�n r�lativistik 
    //merkez noktas�nda olmas� i�in haritan�n merkezini bulan fonksiyon.
    private Vector3 FindCenterOfTransforms(List<Room> rooms) {
        var visitedRooms = VisitedRooms();

        if(visitedRooms.Count == 1) {
            return (Vector3Int)rooms[0].roomGeneratePoint;
        }

        var bound = new Bounds(VisitedRooms()[0].transform.position, Vector3.zero);
        for (int i = 1; i < visitedRooms.Count; i++) {
            bound.Encapsulate(visitedRooms[i].transform.position);
        }
        return bound.center;
    }

    //Haritan�n y�ksekli�ini oda say�s� olarak d�nd�ren fonksiyon.
    //Minimap ve ���nlanama ekran�nda kullan�lan kameran�n�n harita s�n�rlar�nda olmas� i�in kullan�l�r.
    private float FindMapHeight() {
        if(VisitedRooms().Count == 1) return 1;
        else if(VisitedRooms().Count == 2) return 2;
        else {
            int minY = int.MaxValue;
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach (var room in VisitedRooms()) {
                var center = room.roomGeneratePoint;
                if (center.x > maxX) maxX = center.x;
                else if (center.x < minX) minX = center.x;
                if (center.y > maxY) maxY = center.y;
                else if (center.y < minY) minY = center.y;
            }

            int heigth = maxY - minY;
            if (heigth < 0) heigth *= -1;
            int width = maxX - minX;
            if (width < 0) width *= -1;

            if(heigth >= width - 2) 
                return (heigth / parameters.corridorWalkLength) + 1;
            else 
                return ((heigth / parameters.corridorWalkLength) + ((width - heigth) / parameters.corridorWalkLength - 1) + 1);
        }
    }

    //Oyuncunun giri� yapt��� her oday� bir listede tutar. Bu listede bulunan odalar haritada g�z�km�� olur.
    public List<Room> VisitedRooms() {
        List<Room> activeRooms = new List<Room>();
        foreach (var room in DungeonGenerator.instance.roomsInGameList) {
            if (room.name == "Room_1") room.isPlayerVisited = true;
            if (room.isPlayerVisited) {
                activeRooms.Add(room);
            }
        }
        return activeRooms;
    }
}
