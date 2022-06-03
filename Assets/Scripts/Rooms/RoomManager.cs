using System.Collections.Generic;
using UnityEngine;


//Odalarý yöneten Script

public class RoomManager : MonoBehaviour {
    public static RoomManager instance;

    public GameObject roomPrefab;       //Odalarýn üretiminde kullanýlacak prefab
    public int minEnemyCount;           //bir odada bulunabilecek minumum düþman sayýsý
    public int maxEnemyCount;           //bir odada bulunabilecek maksinmum düþman sayýsý
    public GameObject[] enemies;        //odadaki düþmanlarýn listesi
    public GameObject roomBox;          
    [Range(0, 100)] public int roomBoxPercentage = 8;           //odadaki kutularýn potansiyel kutu oluþma alanlarýnda oluþma ihtimali
    public List<GameObject> weaponBox = new List<GameObject>(); //oda içerisinde varsa silah kutularýný tutan liste
    public GameObject spikeObstacle;                            //oda içerisindeki kullanýlacak olan tuzaðýn prefabý
    public int maxSpikeForRooms = 5;                            //bir oda içerisnde maksiumum kaç tuzak olabilir
    public GameObject key;                                      //oda içerinde oluþacak olan anahtarýn prefabý
    

    private void Awake() {
        instance = this;
    }

    
}
