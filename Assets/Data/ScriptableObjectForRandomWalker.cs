using UnityEngine;
using UnityEngine.Tilemaps;

//Odalar� olu�turucu algoritma i�in de�erleri tutan Scriptable Object nesnesi.
//Oyun ba�lamadan da oyuncu taraf�ndan de�i�ikliklerin yap�lmas�n� sa�lamak amac�yla olu�turuldu.

[CreateAssetMenu(fileName = "RandomWalkerParameters_", menuName = "Random Walker Data")]
public class ScriptableObjectForRandomWalker : ScriptableObject {
    public int maxIterations;   //Random walker algoritmas� i�in maksimum iterasyon say�s�
    public int minIterations;   //Random walker algoritmas� i�in minimum iterasyon say�s�
    public int maxWalkLength;   //Random walker algoritmas� i�in maksimum ad�m say�s�
    public int minWalkLength;   //Random walker algoritmas� i�in minimum ad�m say�s�

    //Oyun i�erisinde bulunan odalar�n zemini i�in i�erisinden rastgele se�ilecek Tile g�rsellerini tutan dizi
    [SerializeField] public TileBase[] floorTiles = new TileBase[4];
    //Oyun i�erisinde bulunan odalar�n duvarlar� i�in i�erisinden rastgele se�ilecek Tile g�rsellerini tutan dizi
    [SerializeField] public TileBase[] wallTiles = new TileBase[4];     



}
