using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Minimap haritasýnýn odalarýn açýlmasý ile birlikte açýlan odalarý göstermesini saðlamak amacýyla oluþturulan script.
public class MapCamera : MonoBehaviour {
    public static MapCamera instance;
    public float size; 
    [SerializeField] private ScriptableObjectForCorridorWalker parameters;

    private void Awake() {
        instance = this;
    }

    //Açýlan her oda ile birlikte haritanýn güncellenmesini saðlar.
    private void Update() {
        transform.position = FindCenterOfTransforms(DungeonGenerator.instance.roomsInGameList);
        GetComponent<Camera>().orthographicSize = ((parameters.corridorWalkLength / 2) * FindMapHeight());
        size = GetComponent<Camera>().orthographicSize;
    }

    //Minimap ve ýþýnlama iþlemleri için kullanýlacak olan kameranýn haritanýn rölativistik 
    //merkez noktasýnda olmasý için haritanýn merkezini bulan fonksiyon.
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

    //Haritanýn yüksekliðini oda sayýsý olarak döndüren fonksiyon.
    //Minimap ve ýþýnlanama ekranýnda kullanýlan kameranýnýn harita sýnýrlarýnda olmasý için kullanýlýr.
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

    //Oyuncunun giriþ yaptýðý her odayý bir listede tutar. Bu listede bulunan odalar haritada gözükmüþ olur.
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
