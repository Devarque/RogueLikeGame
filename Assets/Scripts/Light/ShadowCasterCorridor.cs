using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


//DETAYLARI SHADOWCASTER.CS DOSYASI ÝÇERÝSÝNDE AÇIKLANMIÞTIR. TEK FARKI BU SCRÝPTÝN KORÝDORLAR 
//VE ONLARIN GÖLGE OLUÞTURUCULARI ÝÇÝN ÇALIÞMASIDIR.

public class ShadowCasterCorridor : MonoBehaviour {
    public static ShadowCasterCorridor instance;

    private bool control = false;
    public string[] avoidedSortingLayers;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        if (!control) {
            CompositeCollider2D tilemapCollider = GetComponent<CompositeCollider2D>();

            if (tilemapCollider.pathCount > 0) {
                GameObject shadowCasterContainer = GameObject.FindGameObjectWithTag("shadow_caster_corridor");

                for (int i = 0; i < tilemapCollider.pathCount; i++) {
                    Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
                    tilemapCollider.GetPath(i, pathVertices);
                    GameObject shadowCaster = new GameObject(transform.parent.name + "_wall_polygon_" + i);

                    shadowCaster.transform.position += transform.parent.GetComponent<Corridor>().transform.position;

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
