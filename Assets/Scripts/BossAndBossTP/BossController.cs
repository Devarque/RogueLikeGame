using System;
using UnityEngine;

public class BossController : MonoBehaviour {
    //T�m projede nesne olup�turmadan bu s�n�ftan olui�turulan statik nesne ile t�m public de�erlere eri�im sa�lamak i�in
    public static BossController instance;  
    //Oyuncu verilerine eri�im i�in
    public SOPlayerData data;


    public BossAction[] actions;    //Bossun statelerini i�eren dizi, BossAction s�n�f� altta tan�mlanm��t�r
    private int currentAction;      //Aktif olan state index numaras�
    private float actionCounter;    //Aktif olan statein biti� s�resine kadar kontrolleri sa�layan saya�

    private float shotCounter;      //Bossun at��lar� aras�ndaki s�renin sayac�
    public Rigidbody2D theRB;       
    private Vector2 moveDirection;  //Bossun hareket y�n�

    public int currentHealth;       //Bossun sa�l���

    [HideInInspector] public bool bossIsDead = false;   //Bossun ya�ama durumunu tutar
    private bool move = true;                           //Boss hareket halinde ise true 
    private Transform movePoint;                        //Bossun �nceden tan�ml� hareket noktalar�n� tutan dizi
    public float moveToNextMovePointCounter = 0.7f;     //Bir hareket noktas�ndan di�erine ge�mek i�in beklenilen s�re

    private bool playMusic = true;                      //M�zik kontrol�

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
        //Bossun stateleri aras�ndaki ge�i�leri kontrol eden fonksiyon.
        if (PlayerEntry.instance.playerIsPresent) {
            if (actionCounter > 0) {
                actionCounter -= Time.deltaTime;

                //Boss hareket i�lemleri.
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

                //Boss sald�r� i�lemleri
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
                //her bir state sonunda bir sonraki state e ge�ebilmek i�in state index numaras� 1 artt�r�l�r
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

    //Boss hasar alma durumlar�n� y�neten fonksiyon. Hasar ald���nda can� 1 azal�r. 0 olursa �l�r.
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

//Boss state s�n�f�. State de�i�kenlerini i�erir.
[Serializable]
public class BossAction {
    [Header("Action")]
    public float actionLength;          //State aktif olma s�resi
        
    public bool shouldMove;             //Hareket kontrol�
    public bool shouldChasePlayer;      //Oyuncuyu kovalamal� m� kontrol�
    public float moveSpeed;             //Hareket h�z�
    public bool moveToPoints;           //�nceden tan�ml� noktalara hareket etmeli mi

    public bool shouldShoot;            //Ate� etmeli mi
    public GameObject itemToShoot;      //Ate� edece�i nesne
    public float timeBetweenShots;      //Ate�ler aras� bekleme s�resi
    public Transform[] firePoints;      //Ate� etme noktalar�
    public Transform[] pointsToMoveTo;  //Hareket etme noktalar�



}
