using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public static EnemyController instance;
    public SOPlayerData ingamedata;

    [Header("Attributes")] //d��man �zellikleri
    public float moveSpeed; //hareket h�z�
    public int health = 15; //sa�l�k
    public bool shooting; //ate� etmeli mi
    public float fireRate; //at��lar aras� s�re
    private float fireCounter; //at��lar aras� s�reyi kontrol etmek i�in 
    public float shootRange; //ate� menzili

    public Room room; //bulundu�u oda
    private Vector3 moveDirection; //hareket y�n�

    [Header("Chase")] //karakteri kovalama
    public bool shouldChase;
    public float rangeToChasePlayer;

    [Header("Run Away")] //karakterden ka�ma
    public bool shouldRunAway;
    public float runAwayRange;

    [Header("Wander")] //yak�n�nda karakter yoksa kendi ba��na dola�ma
    public bool shouldWander;
    public float wanderLength, pauseLength;
    [HideInInspector] public float wanderCounter, pauseCounter;
    private Vector3 wanderDirection;

    [Header("Divide")] //�ld���nde b�l�nerek yeni d��manlar olu�turma
    public bool shouldDivide;
    public int divideCount;
    private bool dividedChild = false;

    [Header("Components")] //scriptte kullan�lmak �zere edit�rden komponent alma
    public SpriteRenderer theBody;
    public Transform firePoint;
    public GameObject bullet;
    public Animator anim;
    public Rigidbody2D theRB;

    [Header("Potion Drop")] //iksir d���rme
    public bool canPotionDrop;
    public float potionDropChance;
    public GameObject[] potionsToDrop;

    [Header("Coin Drop")] //alt�n d���rme
    public bool canCoinDrop;
    public float coinDropChance;
    public GameObject[] coinsToDrop;

    [Header("Ammo Drop")]
    public bool canAmmoDrop;
    public float ammoDropChance;
    public GameObject[] ammoesToDrop;

    //Singleton i�lemi ger�ekle�tirilir. T�m scriptlerden static de�ere eri�ilerek instance olu�turmadan veya event i�lemlerine 
    //gerek duymadan i�lemler yap�labilir.
    private void Awake() {
        instance = this;
    }

    private void Start() {
        if (shouldWander) {
            pauseCounter = Random.Range(pauseLength * .75f, pauseLength * 1.25f);
        }
    }

    void Update() {
        //D��man�n oyuncunun oldu�u tarafa y�z�n� �evirmesini sa�lar. D��man slime ise �l�mden sonra olu�an
        //yavrular�n boyut �l�e�i de istenilen oranda k���lt�l�r.
        if (transform.position.x - PlayerController.instance.transform.position.x < 0) {
            if (dividedChild) transform.localScale = new Vector3(-0.712f, 0.712f, 1f);
            else transform.localScale = new Vector3(-1f, 1f, 1f);
        } else {
            if (dividedChild) transform.localScale = new Vector3(0.712f, 0.712f, 1f);
            else transform.localScale = transform.localScale = Vector3.one;
        }

        //E�er oyuncu i�e d��man ekranda g�z�k�rse ve oyuncu �lmemi�se i�lemler ger�ekle�ir.
        //SpriteRenderer componentinin visible property si ile bu durum kontrol edilir.
        if (theBody.isVisible && PlayerController.instance.gameObject.activeInHierarchy) {
            moveDirection = Vector3.zero;

            //oyuncu kovalama mesafesine girdi�i anda d��man�n hareket y�n� oyuncunun kendisi olur
            if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToChasePlayer && shouldChase) {
                moveDirection = PlayerController.instance.transform.position - transform.position;
            } else {
                //E�er oyuncu d��man kovalama mesafesinin d���ndayse ba��bo� bir �ekilde rastgele y�nlerde
                //rastgele s�relerde hareket eder.
                if (shouldWander) {
                    if (wanderCounter > 0) {
                        wanderCounter -= Time.deltaTime;

                        //Ba��bo� dola�ma s�ras�nda d��man�n hareket edece�i y�n belirlenir.
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
            //E�er d��man oyuncudan ka�an d��mansa ve aralar�ndaki mesafe ka�ma mesafesinden k���kse d��man oyuncunun
            //hareket y�n�n�n tam tersi istikamette geri geri ka�ar.
            if (shouldRunAway && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < runAwayRange) {
                moveDirection = transform.position - PlayerController.instance.transform.position;
            }

            moveDirection.Normalize();

            theRB.velocity = moveDirection * moveSpeed;

            //D��man ate� edebilen d��mansa belirlenen at�� aral�klar�nda ate� etmeye ba�lar.
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

        //D��man�n y�r�me animasyonu e�er hareket halinde ise aktif olur, de�ilse durur.
        if (moveDirection != Vector3.zero) {
            anim.SetBool("isMoving", true);
        } else {
            anim.SetBool("isMoving", false);
        }
    }

    //Karakterden hasar al�nd���nda sa�l���n azalt�lmas�.
    public void DamageEnemy(int damage) {
        health -= damage;
        SoundManager.instance.enemyDamage.Play();
        ingamedata.totalDamageDealt += damage;
        if (health <= 0) {
            ingamedata.totalEnemyKilled++;
            //sa�l�k s�f�r olursa b�l�nme aktifse b�ler ve odadaki d��man say�s�n� b�l�nme say�s� kadar artt�r�r.
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
            //sa�l�k s�f�rlan�nca d��man yok edilir.
            Destroy(gameObject);
            room.enemyCount -= 1;

            //�l�nce iksir d���rme i�lemleri.
            if (canPotionDrop) {
                float dropChance = Random.Range(0f, 100f);

                if (dropChance < potionDropChance) {
                    int randomItem = Random.Range(0, potionsToDrop.Length);
                    Instantiate(potionsToDrop[randomItem], transform.position, transform.rotation);
                }
            }

            //�l�nce alt�n d���rme i�lemleri.
            if (canCoinDrop) {
                float coinChance = Random.Range(0f, 100f);
                if (coinChance < coinDropChance) {
                    int randomItem = Random.Range(0, coinsToDrop.Length);
                    Instantiate(coinsToDrop[randomItem], transform.position, transform.rotation);
                }
            }

            //�l�nce cephane d���rme �i�lemleri
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
