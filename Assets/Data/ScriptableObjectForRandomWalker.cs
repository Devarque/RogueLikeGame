using UnityEngine;
using UnityEngine.Tilemaps;

//Odalarý oluþturucu algoritma için deðerleri tutan Scriptable Object nesnesi.
//Oyun baþlamadan da oyuncu tarafýndan deðiþikliklerin yapýlmasýný saðlamak amacýyla oluþturuldu.

[CreateAssetMenu(fileName = "RandomWalkerParameters_", menuName = "Random Walker Data")]
public class ScriptableObjectForRandomWalker : ScriptableObject {
    public int maxIterations;   //Random walker algoritmasý için maksimum iterasyon sayýsý
    public int minIterations;   //Random walker algoritmasý için minimum iterasyon sayýsý
    public int maxWalkLength;   //Random walker algoritmasý için maksimum adým sayýsý
    public int minWalkLength;   //Random walker algoritmasý için minimum adým sayýsý

    //Oyun içerisinde bulunan odalarýn zemini için içerisinden rastgele seçilecek Tile görsellerini tutan dizi
    [SerializeField] public TileBase[] floorTiles = new TileBase[4];
    //Oyun içerisinde bulunan odalarýn duvarlarý için içerisinden rastgele seçilecek Tile görsellerini tutan dizi
    [SerializeField] public TileBase[] wallTiles = new TileBase[4];     



}
