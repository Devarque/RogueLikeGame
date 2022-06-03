using System.Collections.Generic;
using System.Linq;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Oyunun en �nemli k�sm�n� olu�turan Script.
//Oyuncunun oyunu ba�latmas�ndan sonra ilk olarak ger�ekle�tirlen i�lemlerdir.
//T�m harita bu script i�erisindeki fonksiyonlar ile belirlenen de�erlere g�re olu�turulur
//Koridorlar�n ve odalar�n olu�turulmas�nda RandomWalker algoritmas� kullan�lm��t�r.
//RandomWalker algoritmas� ile se�ilen rastgele konumlarda odalar olu�turulur.
//Listelere aktar�lan bu konumlarda TileMap ile boyamalar yap�l�r
//Oyunun her yeniden ba�lat�l���nda t�m harita rastgele bir �ekilde belirlenen aral�klarda
//yeniden e�siz olarak olu�turulur
//  �nce oyunun koridor rotas� olu�turulur
//  Sonras�nda koridorlar bu rota �zerinde olu�tulur
//  Olu�an koridorlar�n kesi�im noktalar� ve ve ucu a��k noktalarda odalar olu�turulur.
//  Sonras�nda harita duvarlar� olu�turulur
//  En sonunda t�m olu�an yap�n�n tile boyamalar� yap�l�r
//Harita ve oyun tilemap ve bunlar�n colliderlar� kullan�larak olu�turulur



public class DungeonGenerator : RandomWalker {
    public static DungeonGenerator instance;
    [SerializeField] public ScriptableObjectForCorridorWalker corridorParameters;   //koridor olu�turma de�erleri

    [SerializeField] public CorridorManager corridorManager;                        //olu�an koridorlar� y�neten nesne
    [SerializeField] public RoomManager roomManager;                                //olu�an odalar� y�neten nesne

    HashSet<Vector2Int> roomGeneratePoints = new HashSet<Vector2Int>();             //oda olu�turma noktalar�n� tutan liste
    List<List<Vector2Int>> allCorridors = new List<List<Vector2Int>>();             //oyun i�erisindeki t�m koridorlar�n t�m pozisyonlar�n� tutan liste

    List<Corridor> corridorsInGameList = new List<Corridor>();                      //oyun i�erisindeki t�m koridorlar
    [HideInInspector] public List<Room> roomsInGameList = new List<Room>();         //oyun i�erisindeki t�m odalar
    List<GameObject> doorsInGameList = new List<GameObject>();                      //oyun i�erisindeki t�m kap�lar
            
    public GameObject teleporterPrefab;                                             //���nlay�c� oyun objesi



    private void Awake() {
        instance = this;
        RunProceduralGeneration();  //Procedural generation, random walker metodlar� �al��t�r�l�r
    }
    protected void RunProceduralGeneration() {
        GenerateDungeon();          
    }

    private void GenerateDungeon() {  //S�ras�yla �stte belirtilen yap�lar olu�turulur.
        GeneratePath();
        CreateCorridors();
        CreateRooms();
        CreateWalls();
        PaintMap();
    }

    private void CreateWalls() {
        //Statik wallgenerator s�n�f�ndaki methodlar kullan�larak oyun i�erisindeki
        //odalar ve koridorlar i�in duvarlar olu�turulur. Duvarlar�n konumlar�
        //bu methodlar i�erisinde belirlenir.
        WallGenerator.FindRoomWalls(roomsInGameList);
        WallGenerator.FindCorridorWalls(corridorsInGameList);
    }

    private void GeneratePath() {
        //Oyunun koridorlar konumlar�n� pseudo olarak se�er.
        //Olu�an koridor listesinde algoritman�n �al��ma �eklinden dolay�
        //ayn� konumlarda birden fazla koridor olabilir.
        //Ancak daha sonras�nda �al��t�r�lan bir methodla bu tekrar eden koridorlar
        //listeden silinir. B�ylece tekrardan ka��n�lm�� olur.
        //Ba�lang��ta ayn� konumlara sahip koridorlar�n olu�mas�n�n sebebi
        //haritan�n da��n�k bir yap�da olmas�n� sa�lamak i�indir.
        //Olu�an bir koridor konumlar� ters y�nde se�ildi�inde konum se�me i�lemi devam 
        //etti�inden dolay� bir sonraki a�amada daha �ncesinde se�ilmemi� koridor konumlar� se�ilebilir
        //Oyun rapor dosyas�nda g�rseller ile bu i�lem daha detayl� bir �ekilde a��klanacakt�r.
        var currentPosition = startPosition;
        roomGeneratePoints.Add(currentPosition);

        for (int i = 0; i < corridorParameters.corridorIterations; i++) {
            List<Vector2Int> currentCorridorPositions = ProceduralGenerationAlgorithms.RandomWalkerCorridor(currentPosition, corridorParameters.corridorWalkLength);
            if (i == corridorParameters.corridorIterations - 1 && roomGeneratePoints.Count < corridorParameters.minRoomCount) {
                i -= 1;
            }
            allCorridors.Add(currentCorridorPositions);
            currentPosition = currentCorridorPositions[currentCorridorPositions.Count - 1];
            roomGeneratePoints.Add(currentPosition);
        }
    }

    private void CreateCorridors() {
        //Olu�an pseudo koridorlarda �nce tekrar eden koridorlar silinir. uniqueCorridors i�erisinde bu
        //silinme i�leminden sonra kalan koridorlar tutulur.
        //Koridorlar fiziksel olarak oyun i�erisinde ilk olarak burada olu�turulur.
        List<List<Vector2Int>> uniqueCorridors = ProceduralGenerationAlgorithms.RemoveDuplicates(allCorridors);
        Corridor corridor;
        for (int i = 0; i < uniqueCorridors.Count; i++) {
            var currentCorridorPositions = uniqueCorridors[i];
            GameObject corridorPrefab = Instantiate(corridorManager.corridorPrefab, corridorManager.transform);
            corridor = corridorPrefab.GetComponent(typeof(Corridor)) as Corridor;
            corridor.corridorPositions = currentCorridorPositions;
            corridorPrefab.name = "Corridor_" + corridorManager.transform.childCount;
            corridorPrefab.transform.position = new Vector3(corridor.corridorPositions.ElementAt(corridor.corridorPositions.Count / 2).x, corridor.corridorPositions.ElementAt(corridor.corridorPositions.Count / 2).y, 0);
            corridorsInGameList.Add(corridor);
        }
    }

    private void CreateRooms() {
        //E�siz her bir koridor i�in bu koridorlar�n kesi�imi ve koridor u�lar�nda
        //Odalar se�ilen say�da olu�turulur.
        //Odalar RunRandomWalk methodu ile her seferinde birbirlerinden tamamen
        //farkl� olacak �ekilde olu�turulur.
        //Bu odalar�n konumlar�, �ekilleri, zemin g�rselleri birbirlerinden farkl�d�rlar.
        //Bu �ekilde oyuna tekrar oynanabilirlik kat�l�r ve oyunumuz RogueLike t�r�nde oldu�u
        //i�in rastgelelik bizim i�in en �ncelikli s�rada de�erlendirilir.
        Room room;
        for (int i = 0; i < roomGeneratePoints.Count; i++) {
            var roomPosition = roomGeneratePoints.ElementAt(i);
            GameObject roomPrefab;
            roomPrefab = Instantiate(roomManager.roomPrefab, roomManager.transform);
            room = (roomPrefab.GetComponent(typeof(Room)) as Room);
            var roomPositions = RunRandomWalk(randomWalkParemeters, roomPosition);
            room.roomPositions = roomPositions;
            room.roomGeneratePoint = roomGeneratePoints.ElementAt(i);
            roomPrefab.transform.position = new Vector3(room.roomGeneratePoint.x, room.roomGeneratePoint.y, 0);
            roomPrefab.name = "Room_" + roomManager.transform.childCount;
            room.teleporter = Instantiate(teleporterPrefab, new Vector2(roomPosition.x + 0.5f, roomPosition.y + 0.5f), Quaternion.identity, roomManager.transform.GetChild(i));
            roomsInGameList.Add(room);
        }
        CreateDoors();              //Odalar olu�tuktanb sonra her iki oda aras�nda kap�lar olu�turulur
        AddRoomsToCorridors();      //Olu�an odalar koridorlara ba�lan�r.
        AddCorridorsToRooms();      //Olu�an koridorlar odalara ba�lan�l�r.
        //Odalar ve koridorlar�n kesi�imlerinde bulunan alanlar silinir. 
        //Bu sayede olu�acak olan duvarlar�n odalardan ve koridorlardan ��k��� engellememesi sa�lan�r.
        RemoveRoomCorridorCollisions(); 

    }

    //Odalar ile koridorlar�n kesi�im noktalar� konum listelerinin k�yas� yap�ld�ktan sonra
    //e�er kesi�im varsa koridor pozisyonlar� silinir. Odalar ayn� �ekilde kal�r.
    private void RemoveRoomCorridorCollisions() {
        List<List<Vector2Int>> corridorRoomCollisionsList = new List<List<Vector2Int>>();
        corridorRoomCollisionsList = ProceduralGenerationAlgorithms.RemoveCorridorPositionsInRooms(corridorRoomCollisionsList, corridorsInGameList);

        for (int i = 0; i < corridorsInGameList.Count; i++) {
            var corridor = corridorsInGameList[i];
            var corridorPositions = corridor.corridorPositions;
            var currentCorridorRoomColisions = corridorRoomCollisionsList[i];
            for (int y = 0; y < currentCorridorRoomColisions.Count; y++) {
                corridorPositions.Remove(currentCorridorRoomColisions[y]);
            }
        }
    }

    //Her bir koridorun orta noktas�nda kap�lar olu�turulur.
    private void CreateDoors() {
        for (int i = 0; i < corridorsInGameList.Count; i++) {
            var corridorPositions = corridorsInGameList[i].corridorPositions;
            var doorPos = corridorPositions[corridorPositions.Count / 2];
            var beforeDoorPos = corridorPositions[corridorPositions.Count / 2 + 1];

            DoorObject doorManager = FindObjectOfType<DoorObject>();
            GameObject door;
            if ((doorPos - beforeDoorPos).x != 0) {
                door = Instantiate(doorManager.doorPrefabLR, new Vector2(doorPos.x + 0.5f, doorPos.y + 0.5f), Quaternion.identity, doorManager.transform);
            } else {
                door = Instantiate(doorManager.doorPrefabUD, new Vector2(doorPos.x + 0.5f, doorPos.y + 0.5f), Quaternion.identity, doorManager.transform);
            }
            door.name = "Door_" + doorManager.transform.childCount;
            door.tag = "Door";

            doorsInGameList.Add(door);
        }
        //Olu�an kap�lar odalara ba�lan�r. Bu sayede odadaki d��manlar temizlenince
        //a��lacak olan kap�lar belirlenmi� olur.
        AddDoorsToRooms();
    }

    private void AddDoorsToRooms() {
        foreach (var room in roomsInGameList) {
            foreach (var direction in Direction2D.cardinalDirectionsList) {
                var controlPos = (room.roomGeneratePoint + ((corridorParameters.corridorWalkLength / 2) * direction)) + new Vector2(0.5f, 0.5f);
                for (int i = 0; i < doorsInGameList.Count; i++) {
                    var door = doorsInGameList[i];
                    if (controlPos == (Vector2)door.transform.position) {
                        room.roomDoors.Add(door);
                    }
                }
            }
        }
    }

    private void AddRoomsToCorridors() {
        foreach (var corridor in corridorsInGameList) {
            foreach (var room in roomsInGameList) {
                if (corridor.corridorPositions.First() == room.roomGeneratePoint || corridor.corridorPositions.Last() == room.roomGeneratePoint) {
                    corridor.corridorRooms.Add(room.transform.gameObject);
                }
            }
        }
    }

    private void AddCorridorsToRooms() {
        foreach (var room in roomsInGameList) {
            foreach (var corridor in corridorsInGameList) {
                if (corridor.corridorPositions.First() == room.roomGeneratePoint || corridor.corridorPositions.Last() == room.roomGeneratePoint) {
                    room.roomCorridors.Add(corridor.transform.gameObject);
                }
            }
        }
    }

    //T�m olu�turma i�lemlerinden sonra haritan�n g�rsel boyamas� tilemap kullan�larak tile 'lar ile boyan�r.
    private void PaintMap() {
        tileMapVisualizer.PaintRooms(roomsInGameList);
        tileMapVisualizer.PaintCorridors(corridorsInGameList);
    }
}