using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

//Oyundaki t�m koridorlar i�in g�rsel tilemap de�erlerini ve oyun i�erisindeki koridorlar� ve bu koridorlar�n pozisyonlar�n� 
//projenin di�er k�s�mlar�ndan eri�ebilmek i�in olu�turulan Script.
public class Corridor : MonoBehaviour
{
    public Tilemap corridorFloorTilemap;        //Koridor zeminleri g�rselli�i i�in tilemap
    public Tilemap corridorWallTilemap;         //Koridor duvarlar� g�rselli�i i�in tilemap
    public Tilemap corridorMask;                //Ziyaret edilmemi� koridorlar�n haritada g�z�kmemesi i�in, kapat�c� tilemap
    public List<Vector2Int> corridorPositions;  //Koridor pozisyonlar�
    public HashSet<Vector2Int> corridorWallPositions = new HashSet<Vector2Int>();   //Koridor duvar pozisyonlar�
    public List<GameObject> corridorRooms;      //Koridorlar�n ba�l� oldu�u odalar� tutan liste.

    //Oyuncu koridora girdi�inde minimapte ve ���nlanma haritas�nda (b�y�k harita)
    //koridorlar� g�r�nt�lemek i�in kontrol de�i�keni
    public bool isPlayerVisited = false;


}
