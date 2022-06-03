using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

//Oluþan odalar ve bu odalar içerisinde oluþacak olan düþmanlar, kutular ve hasar tuzaklarýný yöneten Script.

public class Room : MonoBehaviour {
    public static Room instance;

    public ScriptableObjectForRandomWalker parameters;

    [Header("Room Components")]
    [SerializeField]
    public Tilemap roomFloorTilemap;            //Oda zemin tile
    public Tilemap roomWallTilemap;             //Oda duvar tile
    public Tilemap roomMaskTilemap;             //Ziyaret edilmeyen odalarýn haritada gözükmesini engelleyen tile
    public TilemapRenderer roomMask;            //Ziyaret edilmeyen odalarýn haritada gözükmesini engelleyen tilemaprenderer

    [Header("Room Specs.")]
    public Vector2Int roomGeneratePoint;        //oda oluþturucu randomwalker algoritmasý baþlangýç noktasý
    public HashSet<Vector2Int> roomPositions;   //randomwalker algoritmasý ile oluþan odalarýn pozisyonlarýný tutan liste
    public HashSet<Vector2Int> roomWallPositions = new HashSet<Vector2Int>();   //odanýn duvarlarýnýn pozisyonlarýný tutan liste
    public List<GameObject> roomDoors;          //odanýn duvarlarýný tutan liste
    public List<GameObject> roomCorridors;      //odanýn baðlý olduðu koridorlarý tutan liste
    public GameObject teleporter;               //oda içeisndeki normal ýþýnlayýcý

    [Header("Objects In Room")]
    public int enemyCount;                      //oda içerisindeki düþman sayýsý 0 olunca kapýlar açýlýr
    private List<GameObject> enemies = new List<GameObject>();  //oda içerisindeki düþmanlarýn listesi
    public int propCount;                       //oda içerisindeki kutularýn sayýsý
    private List<GameObject> props = new List<GameObject>();    //oda içerisndeki kutularý tutan liste
    public bool keyPresent;                     //oda içerisinde anahtar var mý 
    public TileBase floorTile;                  //oda zemin tile
    public bool isPlayerVisited = false;        //oyuncu odayý ziyaret ettiðinde true olur bu sayede ýþýnlanýlabilir ve haritada gözükür
    public bool noEnemy = false;                //odada düþman kalmayýnca true our

    private bool doorFlag;

    public List<Vector2Int> potentialBoxPositions = new List<Vector2Int>();     //odada oluþacak olan kutularýn potansiyel oluþma noktaalarý
    public List<Vector2Int> emptyRoomPositions = new List<Vector2Int>();        //oda içerisinde boþ kalan alanlarýn pozisyonlarý

    public GameObject doorText;                                                 //kapýlar açýldýðýnda ekranda gösterilecek yazý
    private float doorTextFadeOutTime = 2f;                                     //kapý açýldý yasýnýnýn kaybolma süresi

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


    //Oda içerisnde belirlenen aralýklarda düþmanlar uygun pozisyonlarda oluþturulur ve üstteki deðiþkenlerde
    //güncellemeler yapýlýr.
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

    //Oda içerisinde uygun alanlarda belirlenen oranlarda kutularý oluþturan fonksiyon
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

    //Oda içerisnde bulunan tuzaklarý oluþturan fonksiyon. Uygun alanlarda oluþur.
    public void GenerateSpikeObstacle() {
        for (int i = 0; i < Random.Range(5, RoomManager.instance.maxSpikeForRooms); i++) {
            var spikePos = emptyRoomPositions.ElementAt(Random.Range(0, emptyRoomPositions.Count));
            Instantiate(RoomManager.instance.spikeObstacle, new Vector3(spikePos.x + 0.5f, spikePos.y + 0.5f, 0), transform.rotation, gameObject.transform);
            emptyRoomPositions.Remove(spikePos);
        }
    }

    //Oyuncu oda içerisndeki tüm düþmanlarý öldürdüðünde kapýlara açan fonksiyon.
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