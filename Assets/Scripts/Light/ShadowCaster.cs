using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//Iþýklandýrmalar sonrasýnda gölgelerin oluþturulmasý için kullanýlan ShadowCaster scripti.
//Normal koþullar altýnda run time sýrasýnda oluþan nesneler için gölgelendirici kullanýlamazken
//Universal rendering kütüphanesinde deðiþiklikler yapýlarak bu durum mümkün kýlýnmýþtýr.
//Yapýlan araþtýrmalar sonrasýnda script hazýr olarak kullanýlmýþtýr.

//Deðiþiklik yapýlan universal rendering kütüphanesi github üzerinden .git paketi olarak
//Unity paketlerine dahil edilmiþtir.
//Kütüphaneye eriþmek için: https://github.com/Devarque/com.unity.render-pipelines.universal
//Deðiþiklik yapýlan kütüphane dosyasý: Runtime/2D/Shadows/ShadowCaster2D.cs dosyasýdýr.

//Bu sýnýf kullanýlan yapýnýn anlaþýlmasý adýna, kaynak alýnan site: https://forum.unity.com/threads/shadow-caster-2d-not-working-on-tilemap.793803/
//thomasedw kullanýcýnýsýn paylaþýmýnýndan alýnmýþtýr. https://forum.unity.com/members/thomasedw.4150512/


//Ýþleme katýlmayacak Sorting Layerlarýn isimlerinin editörde girilmesinden sonra, bu layerlar hariç
//geri kalan tüm layerlar için gölgelendirici iþlemleri çalýþtýrýlýr. Sahne içerisinde bulunan "Lights" objesi içerisindeki
//"Shadow Caster Objects" objesi içerisinde odalar ve koridorlar için ayrý objelerde tüm gölgelendiriciler oluþturulur.
//Editörden gizmo aktif edildiðinde bu nesneler seçildiiði taktirde shadowcaster nesneleri beyaz bir þekilde görülebilir.


//Tüm gölgelendirici iþlemleri oyunun update fonksiyonunun çalýþmasýndan sonra bir kez çalýþtýrýlýr. 
//Yani tüm iþlemler runtime sýrasýnda ilk framde gerçekleþtirilir.
//Oyun haritasý runtime esnasýnda oluþtuðu için böyle bir çözüme ihtiyaç duyulmuþtur ve kullanýlmýþtýr!!!


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
