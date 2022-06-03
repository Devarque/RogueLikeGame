using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

//Oyundaki tüm koridorlar için görsel tilemap deðerlerini ve oyun içerisindeki koridorlarý ve bu koridorlarýn pozisyonlarýný 
//projenin diðer kýsýmlarýndan eriþebilmek için oluþturulan Script.
public class Corridor : MonoBehaviour
{
    public Tilemap corridorFloorTilemap;        //Koridor zeminleri görselliði için tilemap
    public Tilemap corridorWallTilemap;         //Koridor duvarlarý görselliði için tilemap
    public Tilemap corridorMask;                //Ziyaret edilmemiþ koridorlarýn haritada gözükmemesi için, kapatýcý tilemap
    public List<Vector2Int> corridorPositions;  //Koridor pozisyonlarý
    public HashSet<Vector2Int> corridorWallPositions = new HashSet<Vector2Int>();   //Koridor duvar pozisyonlarý
    public List<GameObject> corridorRooms;      //Koridorlarýn baðlý olduðu odalarý tutan liste.

    //Oyuncu koridora girdiðinde minimapte ve ýþýnlanma haritasýnda (büyük harita)
    //koridorlarý görüntülemek için kontrol deðiþkeni
    public bool isPlayerVisited = false;


}
