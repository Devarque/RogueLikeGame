using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public static EnemyController instance;
    public SOPlayerData ingamedata;

    [Header("Attributes")] //düþman özellikleri
    public float moveSpeed; //hareket hýzý
    public int health = 15; //saðlýk
    public bool shooting; //ateþ etmeli mi
    public float fireRate; //atýþlar arasý süre
    private float fireCounter; //atýþlar arasý süreyi kontrol etmek için 
    public float shootRange; //ateþ menzili

    public Room room; //bulunduðu oda
    private Vector3 moveDirection; //hareket yönü

    [Header("Chase")] //karakteri kovalama
    public bool shouldChase;
    public float rangeToChasePlayer;

    [Header("Run Away")] //karakterden kaçma
    public bool shouldRunAway;
    public float runAwayRange;

    [Header("Wander")] //yakýnýnda karakter yoksa kendi baþýna dolaþma
    public bool shouldWander;
    public float wanderLength, pauseLength;
    [HideInInspector] public float wanderCounter, pauseCounter;
    private Vector3 wanderDirection;

    [Header("Divide")] //öldüðünde bölünerek yeni düþmanlar oluþturma
    public bool shouldDivide;
    public int divideCount;
    private bool dividedChild = false;

    [Header("Components")] //scriptte kullanýlmak üzere editörden komponent alma
    public SpriteRenderer theBody;
    public Transform firePoint;
    public GameObject bullet;
    public Animator anim;
    public Rigidbody2D theRB;

    [Header("Potion Drop")] //iksir düþürme
    public bool canPotionDrop;
    public float potionDropChance;
    public GameObject[] potionsToDrop;

    [Header("Coin Drop")] //altýn düþürme
    public bool canCoinDrop;
    public float coinDropChance;
    public GameObject[] coinsToDrop;

    [Header("Ammo Drop")]
    public bool canAmmoDrop;
    public float ammoDropChance;
    public GameObject[] ammoesToDrop;

    //Singleton iþlemi gerçekleþtirilir. Tüm scriptlerden static deðere eriþilerek instance oluþturmadan veya event iþlemlerine 
    //gerek duymadan iþlemler yapýlabilir.
    private void Awake() {
        instance = this;
    }

    private void Start() {
        if (shouldWander) {
            pauseCounter = Random.Range(pauseLength * .75f, pauseLength * 1.25f);
        }
    }

    void Update() {
        //Düþmanýn oyuncunun olduðu tarafa yüzünü çevirmesini saðlar. Düþman slime ise ölümden sonra oluþan
        //yavrularýn boyut ölçeði de istenilen oranda küçültülür.
        if (transform.position.x - PlayerController.instance.transform.position.x < 0) {
            if (dividedChild) transform.localScale = new Vector3(-0.712f, 0.712f, 1f);
            else transform.localScale = new Vector3(-1f, 1f, 1f);
        } else {
            if (dividedChild) transform.localScale = new Vector3(0.712f, 0.712f, 1f);
            else transform.localScale = transform.localScale = Vector3.one;
        }

        //Eðer oyuncu iþe düþman ekranda gözükürse ve oyuncu ölmemiþse iþlemler gerçekleþir.
        //SpriteRenderer componentinin visible property si ile bu durum kontrol edilir.
        if (theBody.isVisible && PlayerController.instance.gameObject.activeInHierarchy) {
            moveDirection = Vector3.zero;

            //oyuncu kovalama mesafesine girdiði anda düþmanýn hareket yönü oyuncunun kendisi olur
            if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToChasePlayer && shouldChase) {
                moveDirection = PlayerController.instance.transform.position - transform.position;
            } else {
                //Eðer oyuncu düþman kovalama mesafesinin dýþýndayse baþýboþ bir þekilde rastgele yönlerde
                //rastgele sürelerde hareket eder.
                if (shouldWander) {
                    if (wanderCounter > 0) {
                        wanderCounter -= Time.deltaTime;

                        //Baþýboþ dolaþma sýrasýnda düþmanýn hareket edeceði yön belirlenir.
                        moveDirection = wanderDirection;

                        if (wanderCounter <= 0) {
                            pauseCounter = Random.Range(pauseLength * .75f, pauseLength * 1.25f);
                        }
                    }

                    if (pauseCounter > 0) {
                        pauseCounter -= Time.deltaTime;

                        if (pauseCounter <= 0) {
                            wanderCounter = Random.Range(wanderLength * .75f, wanderLength * 1.25f);

                            wanderDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                        }
                    }
                }
            }
            //Eðer düþman oyuncudan kaçan düþmansa ve aralarýndaki mesafe kaçma mesafesinden küçükse düþman oyuncunun
            //hareket yönünün tam tersi istikamette geri geri kaçar.
            if (shouldRunAway && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < runAwayRange) {
                moveDirection = transform.position - PlayerController.instance.transform.position;
            }

            moveDirection.Normalize();

            theRB.velocity = moveDirection * moveSpeed;

            //Düþman ateþ edebilen düþmansa belirlenen atýþ aralýklarýnda ateþ etmeye baþlar.
            if (shooting && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < shootRange) {
                fireCounter -= Time.deltaTime;

                if (fireCounter <= 0) {
                    fireCounter = fireRate;
                    Instantiate(bullet, firePoint.position, firePoint.rotation);
                    SoundManager.instance.enemyFire.Play();
                }
            }
        } else {
            theRB.velocity = Vector2.zero;
        }

        //Düþmanýn yürüme animasyonu eðer hareket halinde ise aktif olur, deðilse durur.
        if (moveDirection != Vector3.zero) {
            anim.SetBool("isMoving", true);
        } else {
            anim.SetBool("isMoving", false);
        }
    }

    //Karakterden hasar alýndýðýnda saðlýðýn azaltýlmasý.
    public void DamageEnemy(int damage) {
        health -= damage;
        SoundManager.instance.enemyDamage.Play();
        ingamedata.totalDamageDealt += damage;
        if (health <= 0) {
            ingamedata.totalEnemyKilled++;
            //saðlýk sýfýr olursa bölünme aktifse böler ve odadaki düþman sayýsýný bölünme sayýsý kadar arttýrýr.
            if (shouldDivide) {
                shouldDivide = false;
                for (int i = 0; i < divideCount; i++) {
                    GameObject divided = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation, room.transform);
                    divided.GetComponent<EnemyController>().health = 8;
                    divided.transform.localScale = new Vector3(-0.712f, 0.712f, 0f);
                    divided.gameObject.GetComponent<EnemyController>().dividedChild = true;
                    room.enemyCount += 1;
                }
            }
            //saðlýk sýfýrlanýnca düþman yok edilir.
            Destroy(gameObject);
            room.enemyCount -= 1;

            //Ölünce iksir düþürme iþlemleri.
            if (canPotionDrop) {
                float dropChance = Random.Range(0f, 100f);

                if (dropChance < potionDropChance) {
                    int randomItem = Random.Range(0, potionsToDrop.Length);
                    Instantiate(potionsToDrop[randomItem], transform.position, transform.rotation);
                }
            }

            //Ölünce altýn düþürme iþlemleri.
            if (canCoinDrop) {
                float coinChance = Random.Range(0f, 100f);
                if (coinChance < coinDropChance) {
                    int randomItem = Random.Range(0, coinsToDrop.Length);
                    Instantiate(coinsToDrop[randomItem], transform.position, transform.rotation);
                }
            }

            //Ölünce cephane düþürme Ýiþlemleri
            if (canAmmoDrop) {
                float ammoChance = Random.Range(0f, 100f);
                if (ammoChance < ammoDropChance) {
                    int randomItem = Random.Range(0, ammoesToDrop.Length);
                    Instantiate(ammoesToDrop[randomItem], transform.position, transform.rotation);
                }
            }
        }
    }
}
