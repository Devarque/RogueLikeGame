using System.Collections.Generic;
using System.Linq;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Oyunun en önemli kýsmýný oluþturan Script.
//Oyuncunun oyunu baþlatmasýndan sonra ilk olarak gerçekleþtirlen iþlemlerdir.
//Tüm harita bu script içerisindeki fonksiyonlar ile belirlenen deðerlere göre oluþturulur
//Koridorlarýn ve odalarýn oluþturulmasýnda RandomWalker algoritmasý kullanýlmýþtýr.
//RandomWalker algoritmasý ile seçilen rastgele konumlarda odalar oluþturulur.
//Listelere aktarýlan bu konumlarda TileMap ile boyamalar yapýlýr
//Oyunun her yeniden baþlatýlýþýnda tüm harita rastgele bir þekilde belirlenen aralýklarda
//yeniden eþsiz olarak oluþturulur
//  Önce oyunun koridor rotasý oluþturulur
//  Sonrasýnda koridorlar bu rota üzerinde oluþtulur
//  Oluþan koridorlarýn kesiþim noktalarý ve ve ucu açýk noktalarda odalar oluþturulur.
//  Sonrasýnda harita duvarlarý oluþturulur
//  En sonunda tüm oluþan yapýnýn tile boyamalarý yapýlýr
//Harita ve oyun tilemap ve bunlarýn colliderlarý kullanýlarak oluþturulur



public class DungeonGenerator : RandomWalker {
    public static DungeonGenerator instance;
    [SerializeField] public ScriptableObjectForCorridorWalker corridorParameters;   //koridor oluþturma deðerleri

    [SerializeField] public CorridorManager corridorManager;                        //oluþan koridorlarý yöneten nesne
    [SerializeField] public RoomManager roomManager;                                //oluþan odalarý yöneten nesne

    HashSet<Vector2Int> roomGeneratePoints = new HashSet<Vector2Int>();             //oda oluþturma noktalarýný tutan liste
    List<List<Vector2Int>> allCorridors = new List<List<Vector2Int>>();             //oyun içerisindeki tüm koridorlarýn tüm pozisyonlarýný tutan liste

    List<Corridor> corridorsInGameList = new List<Corridor>();                      //oyun içerisindeki tüm koridorlar
    [HideInInspector] public List<Room> roomsInGameList = new List<Room>();         //oyun içerisindeki tüm odalar
    List<GameObject> doorsInGameList = new List<GameObject>();                      //oyun içerisindeki tüm kapýlar
            
    public GameObject teleporterPrefab;                                             //ýþýnlayýcý oyun objesi



    private void Awake() {
        instance = this;
        RunProceduralGeneration();  //Procedural generation, random walker metodlarý çalýþtýrýlýr
    }
    protected void RunProceduralGeneration() {
        GenerateDungeon();          
    }

    private void GenerateDungeon() {  //Sýrasýyla üstte belirtilen yapýlar oluþturulur.
        GeneratePath();
        CreateCorridors();
        CreateRooms();
        CreateWalls();
        PaintMap();
    }

    private void CreateWalls() {
        //Statik wallgenerator sýnýfýndaki methodlar kullanýlarak oyun içerisindeki
        //odalar ve koridorlar için duvarlar oluþturulur. Duvarlarýn konumlarý
        //bu methodlar içerisinde belirlenir.
        WallGenerator.FindRoomWalls(roomsInGameList);
        WallGenerator.FindCorridorWalls(corridorsInGameList);
    }

    private void GeneratePath() {
        //Oyunun koridorlar konumlarýný pseudo olarak seçer.
        //Oluþan koridor listesinde algoritmanýn çalýþma þeklinden dolayý
        //ayný konumlarda birden fazla koridor olabilir.
        //Ancak daha sonrasýnda çalýþtýrýlan bir methodla bu tekrar eden koridorlar
        //listeden silinir. Böylece tekrardan kaçýnýlmýþ olur.
        //Baþlangýçta ayný konumlara sahip koridorlarýn oluþmasýnýn sebebi
        //haritanýn daðýnýk bir yapýda olmasýný saðlamak içindir.
        //Oluþan bir koridor konumlarý ters yönde seçildiðinde konum seçme iþlemi devam 
        //ettiðinden dolayý bir sonraki aþamada daha öncesinde seçilmemiþ koridor konumlarý seçilebilir
        //Oyun rapor dosyasýnda görseller ile bu iþlem daha detaylý bir þekilde açýklanacaktýr.
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
        //Oluþan pseudo koridorlarda önce tekrar eden koridorlar silinir. uniqueCorridors içerisinde bu
        //silinme iþleminden sonra kalan koridorlar tutulur.
        //Koridorlar fiziksel olarak oyun içerisinde ilk olarak burada oluþturulur.
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
        //Eþsiz her bir koridor için bu koridorlarýn kesiþimi ve koridor uçlarýnda
        //Odalar seçilen sayýda oluþturulur.
        //Odalar RunRandomWalk methodu ile her seferinde birbirlerinden tamamen
        //farklý olacak þekilde oluþturulur.
        //Bu odalarýn konumlarý, þekilleri, zemin görselleri birbirlerinden farklýdýrlar.
        //Bu þekilde oyuna tekrar oynanabilirlik katýlýr ve oyunumuz RogueLike türünde olduðu
        //için rastgelelik bizim için en öncelikli sýrada deðerlendirilir.
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
        CreateDoors();              //Odalar oluþtuktanb sonra her iki oda arasýnda kapýlar oluþturulur
        AddRoomsToCorridors();      //Oluþan odalar koridorlara baðlanýr.
        AddCorridorsToRooms();      //Oluþan koridorlar odalara baðlanýlýr.
        //Odalar ve koridorlarýn kesiþimlerinde bulunan alanlar silinir. 
        //Bu sayede oluþacak olan duvarlarýn odalardan ve koridorlardan çýkýþý engellememesi saðlanýr.
        RemoveRoomCorridorCollisions(); 

    }

    //Odalar ile koridorlarýn kesiþim noktalarý konum listelerinin kýyasý yapýldýktan sonra
    //eðer kesiþim varsa koridor pozisyonlarý silinir. Odalar ayný þekilde kalýr.
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

    //Her bir koridorun orta noktasýnda kapýlar oluþturulur.
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
        //Oluþan kapýlar odalara baðlanýr. Bu sayede odadaki düþmanlar temizlenince
        //açýlacak olan kapýlar belirlenmiþ olur.
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

    //Tüm oluþturma iþlemlerinden sonra haritanýn görsel boyamasý tilemap kullanýlarak tile 'lar ile boyanýr.
    private void PaintMap() {
        tileMapVisualizer.PaintRooms(roomsInGameList);
        tileMapVisualizer.PaintCorridors(corridorsInGameList);
    }
}