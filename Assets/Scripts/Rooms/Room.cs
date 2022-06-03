using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

//Olu�an odalar ve bu odalar i�erisinde olu�acak olan d��manlar, kutular ve hasar tuzaklar�n� y�neten Script.

public class Room : MonoBehaviour {
    public static Room instance;

    public ScriptableObjectForRandomWalker parameters;

    [Header("Room Components")]
    [SerializeField]
    public Tilemap roomFloorTilemap;            //Oda zemin tile
    public Tilemap roomWallTilemap;             //Oda duvar tile
    public Tilemap roomMaskTilemap;             //Ziyaret edilmeyen odalar�n haritada g�z�kmesini engelleyen tile
    public TilemapRenderer roomMask;            //Ziyaret edilmeyen odalar�n haritada g�z�kmesini engelleyen tilemaprenderer

    [Header("Room Specs.")]
    public Vector2Int roomGeneratePoint;        //oda olu�turucu randomwalker algoritmas� ba�lang�� noktas�
    public HashSet<Vector2Int> roomPositions;   //randomwalker algoritmas� ile olu�an odalar�n pozisyonlar�n� tutan liste
    public HashSet<Vector2Int> roomWallPositions = new HashSet<Vector2Int>();   //odan�n duvarlar�n�n pozisyonlar�n� tutan liste
    public List<GameObject> roomDoors;          //odan�n duvarlar�n� tutan liste
    public List<GameObject> roomCorridors;      //odan�n ba�l� oldu�u koridorlar� tutan liste
    public GameObject teleporter;               //oda i�eisndeki normal ���nlay�c�

    [Header("Objects In Room")]
    public int enemyCount;                      //oda i�erisindeki d��man say�s� 0 olunca kap�lar a��l�r
    private List<GameObject> enemies = new List<GameObject>();  //oda i�erisindeki d��manlar�n listesi
    public int propCount;                       //oda i�erisindeki kutular�n say�s�
    private List<GameObject> props = new List<GameObject>();    //oda i�erisndeki kutular� tutan liste
    public bool keyPresent;                     //oda i�erisinde anahtar var m� 
    public TileBase floorTile;                  //oda zemin tile
    public bool isPlayerVisited = false;        //oyuncu oday� ziyaret etti�inde true olur bu sayede ���nlan�labilir ve haritada g�z�k�r
    public bool noEnemy = false;                //odada d��man kalmay�nca true our

    private bool doorFlag;

    public List<Vector2Int> potentialBoxPositions = new List<Vector2Int>();     //odada olu�acak olan kutular�n potansiyel olu�ma noktaalar�
    public List<Vector2Int> emptyRoomPositions = new List<Vector2Int>();        //oda i�erisinde bo� kalan alanlar�n pozisyonlar�

    public GameObject doorText;                                                 //kap�lar a��ld���nda ekranda g�sterilecek yaz�
    private float doorTextFadeOutTime = 2f;                                     //kap� a��ld� yas�n�n�n kaybolma s�resi

    private void Awake() {
        instance = this;
    }

    private void Start() {
        doorFlag = false;
        if (name == "Room_1") isPlayerVisited = true;
    }

    private void Update() {
        CheckDoorsToOpen();
        if (doorText.activeInHierarchy) {
            doorTextFadeOutTime -= Time.deltaTime;
            if (doorTextFadeOutTime <= 0) {
                doorText.SetActive(false);
                doorTextFadeOutTime = 2f;
            }
        }
    }


    //Oda i�erisnde belirlenen aral�klarda d��manlar uygun pozisyonlarda olu�turulur ve �stteki de�i�kenlerde
    //g�ncellemeler yap�l�r.
    public void SpawnEnemies() {
        emptyRoomPositions = roomPositions.ToList();
        emptyRoomPositions.Remove(roomGeneratePoint);

        if (name == "Room_1") {
            enemyCount = 2;
        } else {
            enemyCount = Random.Range(RoomManager.instance.minEnemyCount, RoomManager.instance.maxEnemyCount);
        }

        while (enemies.Count < enemyCount) {
            int randomPos = Random.Range(0, roomPositions.Count);
            bool present = true;
            foreach (var direction in Direction2D.cardinalDirectionsList) {
                var neighbourPos = direction + roomPositions.ElementAt(randomPos);
                if (!roomPositions.Contains(neighbourPos)) {
                    present = false;
                }

                if (enemies.Count > 0) {
                    foreach (var enemy in enemies) {
                        if (neighbourPos.x == enemy.transform.position.x - 0.5f &&
                            neighbourPos.y == enemy.transform.position.y - 0.5f) {
                            present = false;
                        }
                    }
                }
            }
            if (present) {
                Vector2Int position = roomPositions.ElementAt(randomPos);
                emptyRoomPositions.Remove(position);


                int enemyIndex = Random.Range(0, RoomManager.instance.enemies.Length);
                var enemy = RoomManager.instance.enemies[enemyIndex];
                enemy.GetComponent<EnemyController>().room = this;

                enemies.Add(Instantiate(enemy, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), enemy.transform.rotation, gameObject.transform));
            }
        }
    }

    //Oda i�erisinde uygun alanlarda belirlenen oranlarda kutular� olu�turan fonksiyon
    public void SpawnBoxes() {
        foreach (var position in roomPositions) {
            bool present = false;
            foreach (var direction in Direction2D.cardinalDirectionsList) {
                var neighbourPos = direction + position;
                if (roomWallPositions.Contains(neighbourPos)) {
                    present = true;
                }
            }

            if (present) {
                potentialBoxPositions.Add(position);
            }
        }

        for (int i = 0; i < potentialBoxPositions.Count; i++) {
            var pos = potentialBoxPositions.ElementAt(i);
            var chance = Random.Range(0, 100);
            if (chance < RoomManager.instance.roomBoxPercentage) {
                props.Add(Instantiate(RoomManager.instance.roomBox, new Vector2(pos.x + 0.5f, pos.y + 0.5f),
                    transform.rotation, gameObject.transform));
                propCount++;
                potentialBoxPositions.Remove(pos);
                emptyRoomPositions.Remove(pos);
            }
        }
    }

    //Oda i�erisnde bulunan tuzaklar� olu�turan fonksiyon. Uygun alanlarda olu�ur.
    public void GenerateSpikeObstacle() {
        for (int i = 0; i < Random.Range(5, RoomManager.instance.maxSpikeForRooms); i++) {
            var spikePos = emptyRoomPositions.ElementAt(Random.Range(0, emptyRoomPositions.Count));
            Instantiate(RoomManager.instance.spikeObstacle, new Vector3(spikePos.x + 0.5f, spikePos.y + 0.5f, 0), transform.rotation, gameObject.transform);
            emptyRoomPositions.Remove(spikePos);
        }
    }

    //Oyuncu oda i�erisndeki t�m d��manlar� �ld�rd���nde kap�lara a�an fonksiyon.
    private void CheckDoorsToOpen() {
        if (!doorFlag) {
            if (enemyCount == 0) {
                noEnemy = true;
            }

            if (noEnemy) {
                foreach (var door in roomDoors) {
                    door.SetActive(false);
                    doorText.SetActive(true);
                    SoundManager.instance.doorOpen.Play();
                    doorFlag = true;
                }
            }
        }
    }

}