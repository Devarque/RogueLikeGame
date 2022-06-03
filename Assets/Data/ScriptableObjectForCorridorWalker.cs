using UnityEngine;
using UnityEngine.Tilemaps;

//Koridor oluþturucu algoritma için deðerleri tutan Scriptable Object nesnesi.
//Oyun baþlamadan da oyuncu tarafýndan deðiþikliklerin yapýlmasýný saðlamak amacýyla oluþturuldu.

[CreateAssetMenu(fileName = "RandomWalkerParameters_", menuName = "Corridor Walker Data")]
public class ScriptableObjectForCorridorWalker : ScriptableObject {
    public int corridorWalkLength = 14; //Koridor geniþliði, birim baþýna kare
    public int corridorIterations = 5;  //Koridor walker algoritmasý için iterasyon sayýsý
    public int minRoomCount = 4;        //Minimum oda sayýsý, bu sayýnýn altýna oyunda oda oluþmaz
    [SerializeField] public TileBase floorTile; //Koridorlarýn zemininde gözükecek tile (zemin görüntüsü)
    [SerializeField] public TileBase wallTile;  //Koridorlarýn duvarýnda gözükecek tile (duvar görüntüsü)

}
