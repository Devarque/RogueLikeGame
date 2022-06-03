using UnityEngine;

//Kýrýlabilen kutular için ve bunlardan eþya düþmesini saðlayan Script


public class Breakables : MonoBehaviour {
    public static Breakables instance;
    public new Collider2D collider;     //etkileþim girilece collider. (player)
    public GameObject[] brokenPieces;   //kutu kýrýldýktan sonra oluþacak kýrýlan parçalar
    public int maxPieces = 5;           //kaç adet kýrýk parça oluþacak

    [Header("Potion Drop")]             //iksir düþürme
    public bool canPotionDrop = false; 
    public float potionDropChance;      //iksir düþürme þansý
    public GameObject[] potionsToDrop;  //düþebilecek iksirler

    [Header("Coin Drop")]               //altýn düþürme
    public bool canCoinDrop = false;    
    public float coinDropChance;        //altýn düþürme þansý
    public GameObject[] coinsToDrop;    //düþebilecek altnlar

    [Header("Gun Drop")]                //silah düþürme
    public bool canGunDrop = false;     
    public float weaponDropChance;      //silah düþürme þansý
    public GameObject[] weaponsToDrop;  //düþebilecek silahlar

    [Header("Ammo Drop")]               //cephane düþürme
    public bool canAmmoDrop;
    public float ammoDropChance = 10f;  //cephane düþürme þansý
    public GameObject[] ammoToDrop;     //düþebilecek cephaneler

    private bool firstTouch = true;     //dash ile kutularýn kýrýlmasý, normal temas ile kutularýn kýrýlmamasý için kontrol deðiþkeni

    private void Awake() {
        instance = this;
    }
    void Start() {
        collider.isTrigger = false;
    }

    void Update() {
        if (instance != null) {
            if (collider.isTrigger) {
                if (!collider.IsTouching(PlayerController.instance.colliderCL)) {
                    collider.isTrigger = false;
                }
            }
        }
    }

    //Oyuncunun ateþ ettiði mermiler ile kutunun kýrýlmasý
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player_Bullet") && firstTouch) {
            firstTouch = false;
            Smash();
        }
    }

    //oyuncunun dash ile kutuyla temasýnda triggerýn true yapýlarak
    //oyuncunun dash yeteneði durdurulmamýþ olur ve kutu collision triggerý
    //kullanýlarak kýrýlýr
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            if (PlayerController.instance.dashCounter > 0) {
                collider.isTrigger = true;
                Smash();
            }
        }
    }

    //kutunun kýrýlmasý 
    private void Smash() {
        Destroy(gameObject);
        SoundManager.instance.boxBreak.Play();
        int piecesToSpawn = Random.Range(1, maxPieces);

        for (int i = 0; i < piecesToSpawn; i++) {
            int randomPiece = Random.Range(0, brokenPieces.Length);
            Instantiate(brokenPieces[randomPiece], transform.position, transform.rotation);
        }
        ItemDrop();
    }

    //hangi eþyalarýn düþebileceðini belirleyen fonksiyon
    //üstteki bool deðiþkenlerinin durumlarýna göre belirlenen eþyalar düþer
    private void ItemDrop() {
        if (canPotionDrop) {
            float dropChance = Random.Range(0f, 100f);
            if (dropChance < potionDropChance) {
                int randomItem = Random.Range(0, potionsToDrop.Length);
                Instantiate(potionsToDrop[randomItem], transform.position, transform.rotation);
            }
        }

        if (canCoinDrop) {
            float coinChance = Random.Range(0f, 100f);
            if (coinChance < coinDropChance) {
                int randomItem = Random.Range(0, coinsToDrop.Length);
                Instantiate(coinsToDrop[randomItem], transform.position, transform.rotation);
            }
        }

        if (canGunDrop) {
            float weaponChance = Random.Range(0f, 100f);
            if (weaponChance < weaponDropChance) {
                int randomItem = Random.Range(0, weaponsToDrop.Length);
                Instantiate(weaponsToDrop[randomItem], transform.position, transform.rotation);
            }
        }

        if (canAmmoDrop) {
            float ammoChance = Random.Range(0f, 100f);
            if (ammoChance < ammoDropChance) {
                int randomItem = Random.Range(0, ammoToDrop.Length);
                Instantiate(ammoToDrop[randomItem], transform.position, transform.rotation);
            }
        }

    }

}
