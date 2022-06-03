using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//Oyun içi yönetimi saðlayan Scriptlerden biridir.


public class LevelManager : MonoBehaviour {
    public static LevelManager instance;

    [HideInInspector] public bool isPaused;

    public int currentCoins;        //oyuncunun anlýk altýn sayýsý
    public int currentKeys;         //oyuncunun boss odasýna ýþýnlanmada ihtiyacý olan anahtar sayýsý
    public GameObject marketDoor;   //market alanýna geçiþ kapýsý
    public GameObject bossTeleporter;   //boss odasýna geçiþi saðlayan ýþýnlayýcý

    private List<Room> rooms;       //oyun içerisindeki odalarýn listesi


    private void Awake() {
        instance = this;
    }
    void Start() {
        //Eðer aktif olan Level oyunun oynandýðý level ise odalar içerisinde
        //Düþman oluþumlarý, kutu oluþumlarý, hasar verici dikenlerin oluþumlarý
        //Silah kutularýnýn oluþumlarý burada yapýlýr.
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene")) {
            rooms = DungeonGenerator.instance.roomsInGameList;

            Time.timeScale = 1f;
            UIController.instance.coinText.text = currentCoins.ToString();
            UIController.instance.keyText.text = currentKeys.ToString();

            FillRooms();
        }
    }

    void Update() {
        //ESC tuþuna basýldýðýnda oyunu durdurup devam ettirir
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseUnpause();
        }
    }


    public void PauseUnpause() {
        if (!isPaused) {
            //Oyun durdurulduðunda oynanma süresini tutan sayaç durdurulur.
            //Durdurma ekraný yüklenilir.
            InGameCounter.instance.StopTimer();
            UIController.instance.pauseMenu.SetActive(true);
            isPaused = true;
            Time.timeScale = 0f;
        } else {
            //Sayaç devam ettirilir.
            //Durdurma ekraný kaybolur.
            InGameCounter.instance.ContinueTimer();
            UIController.instance.pauseMenu.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f;
        }
    }

    //Altýn toplandýðýnda anlýk altýn sayýsýný girilen miktar kadar arttýrýr.
    public void GetCoins(int amount) {
        currentCoins += amount;
        UIController.instance.coinText.text = currentCoins.ToString();
    }

    //Markette alýþveriþ sonrasýnda altýn sayýsý fiyat kadar azaltýlýr.
    public void SpendCoins(int amount) {
        currentCoins -= amount;

        if (currentCoins < 0) {
            currentCoins = 0;
        }
        UIController.instance.coinText.text = currentCoins.ToString();
    }

    //Odalarý doldurur.
    public void FillRooms() {
        //Odalar içerisinde oluþturulacak her bir düþman, kutu ve engeller odalara has
        //olduklarý için oyunda bulunan her bir oda için bu iþlemler gerçekleþtirilir.
        foreach (var room in rooms) {
            room.SpawnEnemies();
            room.SpawnBoxes();
            room.GenerateSpikeObstacle();

        }

        //Oyunda bulunan silah kutularý ve market giriþi her bir oda için deðil,
        //tüm level baz alýnarak, tek bir sefer çalýþtýrýlacak.
        SpawnWeaponBoxes();
        SpawnMarketDoor();
        SpawnBossDoor();
        SpawnKeys();
    }

    //Rastgele seçilen 2 adet odada silah kutularý oluþturulur.
    //Bu kutulardan birisinde Tüfek diðerinde ise Pompalý vardýr.
    //Her yeni oyunda silah kutularý farklý odalarda oluþacaktýr.
    private void SpawnWeaponBoxes() {
        var roomCount = rooms.Count;
        var room1Index = Random.Range(0, roomCount);
        var room2Index = Random.Range(0, roomCount);

        while (room2Index == room1Index) room2Index = Random.Range(0, roomCount);

        var room = rooms.ElementAt(room1Index);
        var weaponPosIndex = Random.Range(0, room.potentialBoxPositions.Count - 1);
        Vector2Int weaponPos = room.potentialBoxPositions.ElementAt(weaponPosIndex);
        Instantiate(RoomManager.instance.weaponBox[0], (Vector3Int)weaponPos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        room.potentialBoxPositions.RemoveAt(weaponPosIndex);
        room.emptyRoomPositions.Remove(weaponPos);

        room = rooms.ElementAt(room2Index);
        weaponPosIndex = Random.Range(0, room.potentialBoxPositions.Count - 1);
        weaponPos = room.potentialBoxPositions.ElementAt(weaponPosIndex);
        Instantiate(RoomManager.instance.weaponBox[1], (Vector3Int)weaponPos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        room.potentialBoxPositions.RemoveAt(weaponPosIndex);
        room.emptyRoomPositions.Remove(weaponPos);

    }

    //Harita içerisinden rastgele seçilen bir odada rastgele seçilen bir konumda market
    //kapýsý oluþturulur.
    private void SpawnMarketDoor() {
        Room room = rooms.ElementAt(Random.Range(1, rooms.Count));
        var marketDoorPos = room.emptyRoomPositions.ElementAt(Random.Range(0, room.emptyRoomPositions.Count));
        Instantiate(marketDoor, new Vector3(marketDoorPos.x + 0.5f, marketDoorPos.y + 0.5f, 0), Quaternion.identity,
            room.gameObject.transform);
        room.emptyRoomPositions.Remove(marketDoorPos);
    }

    //Harita içerisinden rastgele seçilen bir odada rastgele seçilen bir konumda 
    //boss ýþýnlayýcýsý oluþturulur
    private void SpawnBossDoor() {
        Room room = rooms.ElementAt(Random.Range(1, rooms.Count));
        var bossPortalPos = room.emptyRoomPositions.ElementAt(Random.Range(0, room.emptyRoomPositions.Count));
        Instantiate(bossTeleporter, new Vector3(bossPortalPos.x + 0.5f, bossPortalPos.y + 0.5f, 0), Quaternion.identity, room.gameObject.transform);
        room.emptyRoomPositions.Remove(bossPortalPos);
    }

    //Harita içeriisnden rastgele seçilen birbirinden farklý üç odada 
    //boss ýþýnlayýcýsýný aktif etmek için kullanýlan anahtarlar oluþturulur.
    private void SpawnKeys() {
        List<Room> keyRooms = new List<Room>();

        while (!(keyRooms.Count == 3)) {
            var room = rooms.ElementAt(Random.Range(0, rooms.Count));
            if (!keyRooms.Contains(room)) {
                keyRooms.Add(room);
                var keyPos = room.emptyRoomPositions.ElementAt(Random.Range(0, room.emptyRoomPositions.Count));
                Instantiate(RoomManager.instance.key, new Vector3(keyPos.x + 0.5f, keyPos.y + 0.5f, 0), Quaternion.identity, room.gameObject.transform);
                room.emptyRoomPositions.Remove(keyPos);
            }
        }
    }

}
