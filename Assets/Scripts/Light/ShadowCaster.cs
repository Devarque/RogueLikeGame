using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//I��kland�rmalar sonras�nda g�lgelerin olu�turulmas� i�in kullan�lan ShadowCaster scripti.
//Normal ko�ullar alt�nda run time s�ras�nda olu�an nesneler i�in g�lgelendirici kullan�lamazken
//Universal rendering k�t�phanesinde de�i�iklikler yap�larak bu durum m�mk�n k�l�nm��t�r.
//Yap�lan ara�t�rmalar sonras�nda script haz�r olarak kullan�lm��t�r.

//De�i�iklik yap�lan universal rendering k�t�phanesi github �zerinden .git paketi olarak
//Unity paketlerine dahil edilmi�tir.
//K�t�phaneye eri�mek i�in: https://github.com/Devarque/com.unity.render-pipelines.universal
//De�i�iklik yap�lan k�t�phane dosyas�: Runtime/2D/Shadows/ShadowCaster2D.cs dosyas�d�r.

//Bu s�n�f kullan�lan yap�n�n anla��lmas� ad�na, kaynak al�nan site: https://forum.unity.com/threads/shadow-caster-2d-not-working-on-tilemap.793803/
//thomasedw kullan�c�n�s�n payla��m�n�ndan al�nm��t�r. https://forum.unity.com/members/thomasedw.4150512/


//��leme kat�lmayacak Sorting Layerlar�n isimlerinin edit�rde girilmesinden sonra, bu layerlar hari�
//geri kalan t�m layerlar i�in g�lgelendirici i�lemleri �al��t�r�l�r. Sahne i�erisinde bulunan "Lights" objesi i�erisindeki
//"Shadow Caster Objects" objesi i�erisinde odalar ve koridorlar i�in ayr� objelerde t�m g�lgelendiriciler olu�turulur.
//Edit�rden gizmo aktif edildi�inde bu nesneler se�ildii�i taktirde shadowcaster nesneleri beyaz bir �ekilde g�r�lebilir.


//T�m g�lgelendirici i�lemleri oyunun update fonksiyonunun �al��mas�ndan sonra bir kez �al��t�r�l�r. 
//Yani t�m i�lemler runtime s�ras�nda ilk framde ger�ekle�tirilir.
//Oyun haritas� runtime esnas�nda olu�tu�u i�in b�yle bir ��z�me ihtiya� duyulmu�tur ve kullan�lm��t�r!!!


public class ShadowCaster : MonoBehaviour {
    public static ShadowCaster instance;
    private bool control = false;
    public string[] avoidedSortingLayers;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        if (!control) {
            CompositeCollider2D tilemapCollider = GetComponent<CompositeCollider2D>();

            if (tilemapCollider.pathCount > 0) {
                GameObject shadowCasterContainer = GameObject.FindGameObjectWithTag("shadow_caster_room");

                for (int i = 0; i < tilemapCollider.pathCount; i++) {
                    Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
                    tilemapCollider.GetPath(i, pathVertices);
                    GameObject shadowCaster = new GameObject(transform.parent.name + "_wall_polygon_" + i);

                    shadowCaster.transform.position += (Vector3Int)transform.parent.GetComponent<Room>().roomGeneratePoint;

                    PolygonCollider2D shadowPolygon = (PolygonCollider2D)shadowCaster.AddComponent(typeof(PolygonCollider2D));
                    shadowCaster.transform.parent = shadowCasterContainer.transform;
                    shadowPolygon.points = pathVertices;
                    shadowPolygon.enabled = false;
                    ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
                    int layerCount = SortingLayer.layers.Length;
                    int[] allLayers = new int[layerCount - avoidedSortingLayers.Length];

                    int x = 0;

                    for (int layerIndex = 0; layerIndex < layerCount; layerIndex++) {
                        if (!avoidedSortingLayers.Contains(SortingLayer.layers[layerIndex].name)) {
                            allLayers[x++] = SortingLayer.layers[layerIndex].id;
                        }
                    }
                    shadowCaster.GetComponent<ShadowCaster2D>().m_ApplyToSortingLayers = allLayers;
                    shadowCasterComponent.selfShadows = true;
                }
                control = true;
            }
        }
    }
}
