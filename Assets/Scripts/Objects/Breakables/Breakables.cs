using UnityEngine;

//K�r�labilen kutular i�in ve bunlardan e�ya d��mesini sa�layan Script


public class Breakables : MonoBehaviour {
    public static Breakables instance;
    public new Collider2D collider;     //etkile�im girilece collider. (player)
    public GameObject[] brokenPieces;   //kutu k�r�ld�ktan sonra olu�acak k�r�lan par�alar
    public int maxPieces = 5;           //ka� adet k�r�k par�a olu�acak

    [Header("Potion Drop")]             //iksir d���rme
    public bool canPotionDrop = false; 
    public float potionDropChance;      //iksir d���rme �ans�
    public GameObject[] potionsToDrop;  //d��ebilecek iksirler

    [Header("Coin Drop")]               //alt�n d���rme
    public bool canCoinDrop = false;    
    public float coinDropChance;        //alt�n d���rme �ans�
    public GameObject[] coinsToDrop;    //d��ebilecek altnlar

    [Header("Gun Drop")]                //silah d���rme
    public bool canGunDrop = false;     
    public float weaponDropChance;      //silah d���rme �ans�
    public GameObject[] weaponsToDrop;  //d��ebilecek silahlar

    [Header("Ammo Drop")]               //cephane d���rme
    public bool canAmmoDrop;
    public float ammoDropChance = 10f;  //cephane d���rme �ans�
    public GameObject[] ammoToDrop;     //d��ebilecek cephaneler

    private bool firstTouch = true;     //dash ile kutular�n k�r�lmas�, normal temas ile kutular�n k�r�lmamas� i�in kontrol de�i�keni

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

    //Oyuncunun ate� etti�i mermiler ile kutunun k�r�lmas�
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player_Bullet") && firstTouch) {
            firstTouch = false;
            Smash();
        }
    }

    //oyuncunun dash ile kutuyla temas�nda trigger�n true yap�larak
    //oyuncunun dash yetene�i durdurulmam�� olur ve kutu collision trigger�
    //kullan�larak k�r�l�r
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            if (PlayerController.instance.dashCounter > 0) {
                collider.isTrigger = true;
                Smash();
            }
        }
    }

    //kutunun k�r�lmas� 
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

    //hangi e�yalar�n d��ebilece�ini belirleyen fonksiyon
    //�stteki bool de�i�kenlerinin durumlar�na g�re belirlenen e�yalar d��er
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
