using UnityEngine;
using UnityEngine.SceneManagement;

//Oyuncunun kullanaca�� silahlar i�in olu�turulan Script.
public class Weapon : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float timeBetweenShots = 0.3f; //at��lar aras� bekleme s�resi

    [Header("Weapon")]
    [SerializeField] private bool useMagazine = true;       //cephaneli silah m�
    [SerializeField] private int magazineSize = 30;         //cephane mermi say�s�
    public string weaponName;                               //silah�n ad�
    public Sprite weaponUI;                                 //UI da g�sterilecek olan silah g�rseli

    private float nextShotTime;                             //at��lar aras� bekleme s�resi

    public int itemCost;                                    //silah sat�n alma bedeli
    public Sprite weaponShopSprite;                         //market odas�nda g�sterilecek silah g�rseli


    public int CurrentAmmo { get; set; }                    //anl�k mermi say�s�
    public WeaponAmmo WeaponAmmo { get; set; }              //silah�n mermisi
    public bool UseMagazine => useMagazine;                 //silah cephaneli mi
    public int MagazineSize => magazineSize;                //silah mermi say�s�
    public bool CanShoot { get; set; }                      //silah ete� edebilir mi. Arka arkaya at��� engellemek i�in.


    private void Awake() {
        WeaponAmmo = GetComponent<WeaponAmmo>();
        CurrentAmmo = magazineSize;
    }

    //E�er oyun Ana Men� veya Kazanma ekran�nda ise karakter ve kamera yok edilir. 
    //Yeni bir oyun ba�lat�lmas� halinde sahnede iki adet kamera ve iki adet oyuncu olmamas� i�in.
    private void Update() {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Title Menu")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Victory")) {
            if (PlayerController.instance.gameObject.activeInHierarchy) {
                Destroy(CameraFollow.instance.gameObject);
                Destroy(PlayerController.instance.gameObject);
            }
        } else {
            //E�er aktif olan level ana men� veya kazanma ekran� de�ilse silahtan mermi ate� edilebilir.
            //Mouse sol t�k ile at�� i�lemi ger�ekle�tirilir. UI �zerinde silah�n mermi say�s� g�ncellenir.
            WeaponCanShoot();
            if (PlayerController.instance.canMove && !LevelManager.instance.isPaused) {
                if (Input.GetMouseButton(0)) {
                    TriggerShot();
                }
            }
            UIController.instance.ammo.text = CurrentAmmo.ToString() + "/" + MagazineSize.ToString();
            //Se�ili silah pistol ise mermi say�s� s�n�rs�z olarakg�sterilir.
            if (gameObject.name == "Pistol") UIController.instance.ammo.text = "oo";
        }
    }
    //At�� i�lemi ba�lar
    public void TriggerShot() {
        StartShooting();
    }

    //E�er cephaneli silahsa mermisini azaltarak at�� yapar.
    //De�ilse (pistol ise) normal at�� yapar. mermi azalmas� olmaz
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
        //Bekleme s�resi ge�tiyse bir sonraki at�� i�lemi i�in onay verir.
        //Kullan�c� bu s�re ge�tikten sonra ate� edebilir.
        if (Time.time > nextShotTime) {
            CanShoot = true;
            nextShotTime = Time.time + timeBetweenShots;
        }
    }

    //E�er at�� etme kontrol� true ise silahtan mermi ate�lenir.
    //Arka arkaya at�� olmamas� i�in at�� kontrol de�i�keni false yap�l�r
    //Bekleme s�resi sayac� tekrar aktif olur
    private void RequestShot() {
        if (!CanShoot) {
            return;
        }
        WeaponAmmo.ConsumeAmmo();
        CanShoot = false;
    }


}
