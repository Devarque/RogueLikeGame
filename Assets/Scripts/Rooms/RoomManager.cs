using System.Collections.Generic;
using UnityEngine;


//Odalar� y�neten Script

public class RoomManager : MonoBehaviour {
    public static RoomManager instance;

    public GameObject roomPrefab;       //Odalar�n �retiminde kullan�lacak prefab
    public int minEnemyCount;           //bir odada bulunabilecek minumum d��man say�s�
    public int maxEnemyCount;           //bir odada bulunabilecek maksinmum d��man say�s�
    public GameObject[] enemies;        //odadaki d��manlar�n listesi
    public GameObject roomBox;          
    [Range(0, 100)] public int roomBoxPercentage = 8;           //odadaki kutular�n potansiyel kutu olu�ma alanlar�nda olu�ma ihtimali
    public List<GameObject> weaponBox = new List<GameObject>(); //oda i�erisinde varsa silah kutular�n� tutan liste
    public GameObject spikeObstacle;                            //oda i�erisindeki kullan�lacak olan tuza��n prefab�
    public int maxSpikeForRooms = 5;                            //bir oda i�erisnde maksiumum ka� tuzak olabilir
    public GameObject key;                                      //oda i�erinde olu�acak olan anahtar�n prefab�
    

    private void Awake() {
        instance = this;
    }

    
}
