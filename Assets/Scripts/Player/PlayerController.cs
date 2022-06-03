using System.Collections.Generic;
using UnityEngine;

//Oyuncunun oyun i�in y�netiminden sorumlu olan Script


public class PlayerController : MonoBehaviour {
    public static PlayerController instance;
    public Vector3 marketHoldPosition;
    public Room room;

    [Header("Shield")]
    public GameObject shield;

    [Header("Movement")]
    [HideInInspector] public bool canMove = true;       //oyuncu hareket edebilir mi
    public float moveSpeed = 3;                         //oyuncu hareket h�z�
    private float activeMoveSpeed;                      //oyuncunun aktif hareket h�z�
    private Vector2 moveInput;                          //oyuncu hareket y�n�n� tutan vekt�r

    [Header("Components")]
    public Rigidbody2D theRB;                           
    public Transform gunArm;
    private Camera theCam;
    public Animator anim;
    public SpriteRenderer bodySR;
    public CircleCollider2D colliderCL;

    [Header("Dash")]
    public float dashSpeed = 8f;                        //dash s�ras�nda oyuncu h�z�
    public float dashLength = 0.5f;                     //dash s�resi
    public float dashCoolDown = 1f;                     //dash bekleme s�resi
    public float dashInvincibilityLength = 0.5f;        //dash hasar almama s�resi
    [HideInInspector] public float dashCounter;         //dash i�lemleri sayac�
    [HideInInspector] public float dashCoolCounter;     //dash bekleme s�resi sayac�
    [HideInInspector] public bool boostedDash = false;  //iksirle aktif edilen dash
    [HideInInspector] public float boostedDashTime = 3f;//iksirle aktif edilen dash s�resi
    [HideInInspector] public float boostedDashTimeCool; //iksirle aktif edilen dash bekleme s�resi sayac�

    [Header("Weapons")]
    public List<Weapon> avaliableGuns = new List<Weapon>();     //oyuncunun �zerinde silahlar�n listesi
    [HideInInspector] public int currentGunIndex;               //oyuncunun bu listede elinde olan silah�n index numaras�


    private void Awake() {
        instance = this;
        boostedDashTimeCool = boostedDashTime;
        //oyuncu sahneler aras� ge�i�te yok edilmez.
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        theCam = Camera.main;
        activeMoveSpeed = moveSpeed;
        UIController.instance.dashSlider.value = 0f;
        UIController.instance.currentGun.sprite = avaliableGuns[currentGunIndex].weaponUI;
        UIController.instance.gunText.text = avaliableGuns[currentGunIndex].weaponName;
    }

    void Update() {
        UIController.instance.currentGun.sprite = avaliableGuns[currentGunIndex].weaponUI;
        UIController.instance.gunText.text = avaliableGuns[currentGunIndex].weaponName;
        if (canMove && !LevelManager.instance.isPaused) {
            MoveAndRotate();
            SwitchGun();
            Dash();
            Animate();
        } else {
            theRB.velocity = Vector2.zero;
            anim.SetBool("isMoving", false);
        }
    }

    //Oyuncuyu hareket ettirir ve y�n�n� mouse y�n�ne �evirir
    private void MoveAndRotate() {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
        theRB.velocity = moveInput * activeMoveSpeed;

        Vector3 mousePos = Input.mousePosition;
        Vector3 screenPoint = theCam.WorldToScreenPoint(transform.localPosition);
        Vector2 offset = new Vector2(mousePos.x - screenPoint.x, mousePos.y - screenPoint.y);

        if (mousePos.x < screenPoint.x) {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            gunArm.localScale = new Vector3(-1f, -1f, 1f);
        } else {
            transform.localScale = Vector3.one;
            gunArm.localScale = Vector3.one;
        }

        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        gunArm.rotation = Quaternion.Euler(0, 0, angle);
    }

    //Oyuncu space tu�u ile dash ytene�ini aktif eder. Bu aktifle�me s�resi boyunca 
    //h�z� belirli miktarda artt�r�l�r ve hasarlara kar�� dayan�kl� olur.
    private void Dash() {
        if (boostedDash) {
            if (boostedDashTime >= 0) {
                boostedDashTime -= Time.deltaTime;
                UIController.instance.unlimitedDashSlider.value = boostedDashTime;
            }
            if (boostedDashTime <= 0) {
                UIController.instance.unlimitedDashSlider.gameObject.SetActive(false);
                boostedDash = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (dashCoolCounter <= 0 && dashCounter <= 0) {
                SoundManager.instance.dash.Play();
                activeMoveSpeed = dashSpeed;
                dashCounter = dashLength;

                //oyuncunun dahs animasyonu
                anim.SetTrigger("dash");
                if (PlayerAttributeController.instance.invincibleCount <= 0) {
                    UIController.instance.invincSlider.maxValue = dashCoolDown;
                    PlayerAttributeController.instance.MakeInvincible(dashInvincibilityLength);
                }
            }
        }
        if (!boostedDash) {
            if (dashCounter > 0) {
                dashCounter -= Time.deltaTime;
                UIController.instance.dashSlider.value = dashCounter + dashCoolDown;
                if (dashCounter <= 0) {
                    activeMoveSpeed = moveSpeed;
                    dashCoolCounter = dashCoolDown;
                }
            }
        } else {
            if (dashCounter > 0) {
                dashCounter -= Time.deltaTime;
                UIController.instance.dashSlider.value = dashCounter + dashCoolDown;
                if (dashCounter <= 0) {
                    activeMoveSpeed = moveSpeed;
                    dashCoolCounter = 0.2f;
                }
            }
        }
        if (dashCoolCounter > 0) {
            dashCoolCounter -= Time.deltaTime;
            UIController.instance.dashSlider.value = dashCoolCounter;
        }
    }

    //Oyuncunun hareket animasyonlar�.
    private void Animate() {
        if (moveInput != Vector2.zero) {
            anim.SetBool("isMoving", true);
        } else {
            anim.SetBool("isMoving", false);
        }
    }

    //TAB tu�una bas�l�rsa o an oyuncunun �zerinde olan silahar aras�nda ge�i� yap�labilir.
    public void SwitchGun() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (avaliableGuns.Count > 0) {
                currentGunIndex++;
                if (currentGunIndex >= avaliableGuns.Count) {
                    currentGunIndex = 0;
                }
                foreach (var gun in avaliableGuns) {
                    gun.gameObject.SetActive(false);
                }
                avaliableGuns[currentGunIndex].gameObject.SetActive(true);
                UIController.instance.currentGun.sprite = avaliableGuns[currentGunIndex].weaponUI;
                UIController.instance.gunText.text = avaliableGuns[currentGunIndex].weaponName;
            }
        }
    }
}
