using UnityEngine;
using UnityEngine.SceneManagement;

//Oyuncunun kullanacaðý silahlar için oluþturulan Script.
public class Weapon : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float timeBetweenShots = 0.3f; //atýþlar arasý bekleme süresi

    [Header("Weapon")]
    [SerializeField] private bool useMagazine = true;       //cephaneli silah mý
    [SerializeField] private int magazineSize = 30;         //cephane mermi sayýsý
    public string weaponName;                               //silahýn adý
    public Sprite weaponUI;                                 //UI da gösterilecek olan silah görseli

    private float nextShotTime;                             //atýþlar arasý bekleme süresi

    public int itemCost;                                    //silah satýn alma bedeli
    public Sprite weaponShopSprite;                         //market odasýnda gösterilecek silah görseli


    public int CurrentAmmo { get; set; }                    //anlýk mermi sayýsý
    public WeaponAmmo WeaponAmmo { get; set; }              //silahýn mermisi
    public bool UseMagazine => useMagazine;                 //silah cephaneli mi
    public int MagazineSize => magazineSize;                //silah mermi sayýsý
    public bool CanShoot { get; set; }                      //silah eteþ edebilir mi. Arka arkaya atýþý engellemek için.


    private void Awake() {
        WeaponAmmo = GetComponent<WeaponAmmo>();
        CurrentAmmo = magazineSize;
    }

    //Eðer oyun Ana Menü veya Kazanma ekranýnda ise karakter ve kamera yok edilir. 
    //Yeni bir oyun baþlatýlmasý halinde sahnede iki adet kamera ve iki adet oyuncu olmamasý için.
    private void Update() {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Title Menu")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Victory")) {
            if (PlayerController.instance.gameObject.activeInHierarchy) {
                Destroy(CameraFollow.instance.gameObject);
                Destroy(PlayerController.instance.gameObject);
            }
        } else {
            //Eðer aktif olan level ana menü veya kazanma ekraný deðilse silahtan mermi ateþ edilebilir.
            //Mouse sol týk ile atýþ iþlemi gerçekleþtirilir. UI üzerinde silahýn mermi sayýsý güncellenir.
            WeaponCanShoot();
            if (PlayerController.instance.canMove && !LevelManager.instance.isPaused) {
                if (Input.GetMouseButton(0)) {
                    TriggerShot();
                }
            }
            UIController.instance.ammo.text = CurrentAmmo.ToString() + "/" + MagazineSize.ToString();
            //Seçili silah pistol ise mermi sayýsý sýnýrsýz olarakgösterilir.
            if (gameObject.name == "Pistol") UIController.instance.ammo.text = "oo";
        }
    }
    //Atýþ iþlemi baþlar
    public void TriggerShot() {
        StartShooting();
    }

    //Eðer cephaneli silahsa mermisini azaltarak atýþ yapar.
    //Deðilse (pistol ise) normal atýþ yapar. mermi azalmasý olmaz
    public void StartShooting() {
        if (useMagazine) {
            if (WeaponAmmo != null) {
                if (WeaponAmmo.CanUseWeapon()) {
                    RequestShot();
                }
            }
        } else {
            RequestShot();
        }
    }

    private void WeaponCanShoot() {
        //Bekleme süresi geçtiyse bir sonraki atýþ iþlemi için onay verir.
        //Kullanýcý bu süre geçtikten sonra ateþ edebilir.
        if (Time.time > nextShotTime) {
            CanShoot = true;
            nextShotTime = Time.time + timeBetweenShots;
        }
    }

    //Eðer atýþ etme kontrolü true ise silahtan mermi ateþlenir.
    //Arka arkaya atýþ olmamasý için atýþ kontrol deðiþkeni false yapýlýr
    //Bekleme süresi sayacý tekrar aktif olur
    private void RequestShot() {
        if (!CanShoot) {
            return;
        }
        WeaponAmmo.ConsumeAmmo();
        CanShoot = false;
    }


}
