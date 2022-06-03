using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//Oyun i�i y�netimi sa�layan Scriptlerden biridir.


public class LevelManager : MonoBehaviour {
    public static LevelManager instance;

    [HideInInspector] public bool isPaused;

    public int currentCoins;        //oyuncunun anl�k alt�n say�s�
    public int currentKeys;         //oyuncunun boss odas�na ���nlanmada ihtiyac� olan anahtar say�s�
    public GameObject marketDoor;   //market alan�na ge�i� kap�s�
    public GameObject bossTeleporter;   //boss odas�na ge�i�i sa�layan ���nlay�c�

    private List<Room> rooms;       //oyun i�erisindeki odalar�n listesi


    private void Awake() {
        instance = this;
    }
    void Start() {
        //E�er aktif olan Level oyunun oynand��� level ise odalar i�erisinde
        //D��man olu�umlar�, kutu olu�umlar�, hasar verici dikenlerin olu�umlar�
        //Silah kutular�n�n olu�umlar� burada yap�l�r.
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene")) {
            rooms = DungeonGenerator.instance.roomsInGameList;

            Time.timeScale = 1f;
            UIController.instance.coinText.text = currentCoins.ToString();
            UIController.instance.keyText.text = currentKeys.ToString();

            FillRooms();
        }
    }

    void Update() {
        //ESC tu�una bas�ld���nda oyunu durdurup devam ettirir
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseUnpause();
        }
    }


    public void PauseUnpause() {
        if (!isPaused) {
            //Oyun durduruldu�unda oynanma s�resini tutan saya� durdurulur.
            //Durdurma ekran� y�klenilir.
            InGameCounter.instance.StopTimer();
            UIController.instance.pauseMenu.SetActive(true);
            isPaused = true;
            Time.timeScale = 0f;
        } else {
            //Saya� devam ettirilir.
            //Durdurma ekran� kaybolur.
            InGameCounter.instance.ContinueTimer();
            UIController.instance.pauseMenu.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f;
        }
    }

    //Alt�n topland���nda anl�k alt�n say�s�n� girilen miktar kadar artt�r�r.
    public void GetCoins(int amount) {
        currentCoins += amount;
        UIController.instance.coinText.text = currentCoins.ToString();
    }

    //Markette al��veri� sonras�nda alt�n say�s� fiyat kadar azalt�l�r.
    public void SpendCoins(int amount) {
        currentCoins -= amount;

        if (currentCoins < 0) {
            currentCoins = 0;
        }
        UIController.instance.coinText.text = currentCoins.ToString();
    }

    //Odalar� doldurur.
    public void FillRooms() {
        //Odalar i�erisinde olu�turulacak her bir d��man, kutu ve engeller odalara has
        //olduklar� i�in oyunda bulunan her bir oda i�in bu i�lemler ger�ekle�tirilir.
        foreach (var room in rooms) {
            room.SpawnEnemies();
            room.SpawnBoxes();
            room.GenerateSpikeObstacle();

        }

        //Oyunda bulunan silah kutular� ve market giri�i her bir oda i�in de�il,
        //t�m level baz al�narak, tek bir sefer �al��t�r�lacak.
        SpawnWeaponBoxes();
        SpawnMarketDoor();
        SpawnBossDoor();
        SpawnKeys();
    }

    //Rastgele se�ilen 2 adet odada silah kutular� olu�turulur.
    //Bu kutulardan birisinde T�fek di�erinde ise Pompal� vard�r.
    //Her yeni oyunda silah kutular� farkl� odalarda olu�acakt�r.
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

    //Harita i�erisinden rastgele se�ilen bir odada rastgele se�ilen bir konumda market
    //kap�s� olu�turulur.
    private void SpawnMarketDoor() {
        Room room = rooms.ElementAt(Random.Range(1, rooms.Count));
        var marketDoorPos = room.emptyRoomPositions.ElementAt(Random.Range(0, room.emptyRoomPositions.Count));
        Instantiate(marketDoor, new Vector3(marketDoorPos.x + 0.5f, marketDoorPos.y + 0.5f, 0), Quaternion.identity,
            room.gameObject.transform);
        room.emptyRoomPositions.Remove(marketDoorPos);
    }

    //Harita i�erisinden rastgele se�ilen bir odada rastgele se�ilen bir konumda 
    //boss ���nlay�c�s� olu�turulur
    private void SpawnBossDoor() {
        Room room = rooms.ElementAt(Random.Range(1, rooms.Count));
        var bossPortalPos = room.emptyRoomPositions.ElementAt(Random.Range(0, room.emptyRoomPositions.Count));
        Instantiate(bossTeleporter, new Vector3(bossPortalPos.x + 0.5f, bossPortalPos.y + 0.5f, 0), Quaternion.identity, room.gameObject.transform);
        room.emptyRoomPositions.Remove(bossPortalPos);
    }

    //Harita i�eriisnden rastgele se�ilen birbirinden farkl� �� odada 
    //boss ���nlay�c�s�n� aktif etmek i�in kullan�lan anahtarlar olu�turulur.
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
