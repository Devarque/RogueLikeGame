using UnityEngine;

//Kameran�n atanan transform objesini takip etmesini sa�layan Script
public class CameraFollow : MonoBehaviour {
    public Transform player;
    public Vector3 offset;

    public static CameraFollow instance;

    private void Awake() {
        instance = this;
        //Levellar aras� ge�i�te kameran�n silinmesini engeller.
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update() {
        transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z);
    }

}
