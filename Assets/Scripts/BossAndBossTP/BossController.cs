using System;
using UnityEngine;

public class BossController : MonoBehaviour {
    //Tüm projede nesne olupþturmadan bu sýnýftan oluiþturulan statik nesne ile tüm public deðerlere eriþim saðlamak için
    public static BossController instance;  
    //Oyuncu verilerine eriþim için
    public SOPlayerData data;


    public BossAction[] actions;    //Bossun statelerini içeren dizi, BossAction sýnýfý altta tanýmlanmýþtýr
    private int currentAction;      //Aktif olan state index numarasý
    private float actionCounter;    //Aktif olan statein bitiþ süresine kadar kontrolleri saðlayan sayaç

    private float shotCounter;      //Bossun atýþlarý arasýndaki sürenin sayacý
    public Rigidbody2D theRB;       
    private Vector2 moveDirection;  //Bossun hareket yönü

    public int currentHealth;       //Bossun saðlýðý

    [HideInInspector] public bool bossIsDead = false;   //Bossun yaþama durumunu tutar
    private bool move = true;                           //Boss hareket halinde ise true 
    private Transform movePoint;                        //Bossun önceden tanýmlý hareket noktalarýný tutan dizi
    public float moveToNextMovePointCounter = 0.7f;     //Bir hareket noktasýndan diðerine geçmek için beklenilen süre

    private bool playMusic = true;                      //Müzik kontrolü

    private void Awake() {
        instance = this;
    }

    private void Start() {
        actionCounter = actions[currentAction].actionLength;

        UIController.instance.bossHealthBar.maxValue = currentHealth;
        UIController.instance.bossHealthBar.value = currentHealth;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            PlayerAttributeController.instance.DamagePlayer(1);
            data.totalDamageTaken += 1;
        }
    }

    private void Update() {
        //Bossun stateleri arasýndaki geçiþleri kontrol eden fonksiyon.
        if (PlayerEntry.instance.playerIsPresent) {
            if (actionCounter > 0) {
                actionCounter -= Time.deltaTime;

                //Boss hareket iþlemleri.
                moveDirection = Vector2.zero;

                if (actions[currentAction].shouldMove) {
                    if (actions[currentAction].shouldChasePlayer) {
                        if (playMusic) {
                            SoundManager.instance.chasePlayer.Play();
                            playMusic = false;
                        }
                        moveDirection = PlayerController.instance.transform.position - transform.position;
                        moveDirection.Normalize();
                    }

                    moveToNextMovePointCounter -= Time.deltaTime;

                    if (actions[currentAction].moveToPoints) {
                        if (move || moveToNextMovePointCounter <= 0) {
                            if (moveToNextMovePointCounter <= 0) {
                                moveToNextMovePointCounter = 0.7f;
                            }
                            movePoint = actions[currentAction].pointsToMoveTo[UnityEngine.Random.Range(0, actions[currentAction].pointsToMoveTo.Length)];
                        }

                        moveDirection = movePoint.position - transform.position;
                        moveDirection.Normalize();
                        move = false;
                    }
                }

                theRB.velocity = moveDirection * actions[currentAction].moveSpeed;

                //Boss saldýrý iþlemleri
                if (actions[currentAction].shouldShoot) {
                    shotCounter -= Time.deltaTime;
                    if (shotCounter <= 0) {
                        shotCounter = actions[currentAction].timeBetweenShots;
                        foreach (Transform fp in actions[currentAction].firePoints) {
                            SoundManager.instance.bossFire.Play();
                            Instantiate(actions[currentAction].itemToShoot, fp.position, fp.rotation);
                        }
                    }
                }
            } else {
                //her bir state sonunda bir sonraki state e geçebilmek için state index numarasý 1 arttýrýlýr
                currentAction++;
                move = true;
                playMusic = true;
                if (currentAction >= actions.Length) {
                    currentAction = 0;
                }

                actionCounter = actions[currentAction].actionLength;
            }
        }
    }

    //Boss hasar alma durumlarýný yöneten fonksiyon. Hasar aldýðýnda caný 1 azalýr. 0 olursa ölür.
    public void TakeDamage(int amount) {
        currentHealth -= amount;
        data.totalDamageDealt += amount;
        SoundManager.instance.playerDamage.Play();

        if (currentHealth <= 0) {
            SoundManager.instance.bossDead.Play();
            data.totalEnemyKilled += 1;
            gameObject.SetActive(false);
            bossIsDead = true;
            UIController.instance.bossHealthBar.gameObject.SetActive(false);
        }
        UIController.instance.bossHealthBar.value = currentHealth;
    }
}

//Boss state sýnýfý. State deðiþkenlerini içerir.
[Serializable]
public class BossAction {
    [Header("Action")]
    public float actionLength;          //State aktif olma süresi
        
    public bool shouldMove;             //Hareket kontrolü
    public bool shouldChasePlayer;      //Oyuncuyu kovalamalý mý kontrolü
    public float moveSpeed;             //Hareket hýzý
    public bool moveToPoints;           //Önceden tanýmlý noktalara hareket etmeli mi

    public bool shouldShoot;            //Ateþ etmeli mi
    public GameObject itemToShoot;      //Ateþ edeceði nesne
    public float timeBetweenShots;      //Ateþler arasý bekleme süresi
    public Transform[] firePoints;      //Ateþ etme noktalarý
    public Transform[] pointsToMoveTo;  //Hareket etme noktalarý



}
