using UnityEngine;

//Ekran üzerinde gözüken crosshair iþaretini mouse imlecinin ekranda bulunduðu pozisyonda gösteren Script

public class WeaponAim : MonoBehaviour {
    [SerializeField] private GameObject reticlePrefab;

    private Camera mainCamera;
    private GameObject reticle;

    private Vector3 direction;
    private Vector3 mousePosition;
    private Vector3 reticlePosition;

    private void Awake() {
        Cursor.visible = true;
    }
    private void Start() {
        Cursor.visible = false;

        mainCamera = Camera.main;
        reticle = Instantiate(reticlePrefab);
    }

    private void Update() {
        GetMousePosition();
        MoveReticle();
    }

    private void MoveReticle() {
        reticle.transform.rotation = Quaternion.identity;
        reticle.transform.position = reticlePosition;
    }

    private void GetMousePosition() {
        mousePosition = Input.mousePosition;
        mousePosition.z = 5f;

        direction = mainCamera.ScreenToWorldPoint(mousePosition);
        direction.z = transform.position.z;
        reticlePosition = direction;
    }
}
