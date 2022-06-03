using UnityEngine;
using UnityEngine.Tilemaps;

//Koridor olu�turucu algoritma i�in de�erleri tutan Scriptable Object nesnesi.
//Oyun ba�lamadan da oyuncu taraf�ndan de�i�ikliklerin yap�lmas�n� sa�lamak amac�yla olu�turuldu.

[CreateAssetMenu(fileName = "RandomWalkerParameters_", menuName = "Corridor Walker Data")]
public class ScriptableObjectForCorridorWalker : ScriptableObject {
    public int corridorWalkLength = 14; //Koridor geni�li�i, birim ba��na kare
    public int corridorIterations = 5;  //Koridor walker algoritmas� i�in iterasyon say�s�
    public int minRoomCount = 4;        //Minimum oda say�s�, bu say�n�n alt�na oyunda oda olu�maz
    [SerializeField] public TileBase floorTile; //Koridorlar�n zemininde g�z�kecek tile (zemin g�r�nt�s�)
    [SerializeField] public TileBase wallTile;  //Koridorlar�n duvar�nda g�z�kecek tile (duvar g�r�nt�s�)

}
