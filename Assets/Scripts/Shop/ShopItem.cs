using UnityEngine;
using UnityEngine.UI;

//Market alan�n� ve marketten e�ya al�nma�sn� y�neten Script.

public class ShopItem : MonoBehaviour {
    public SOPlayerData data;

    public GameObject buyMessage;           //sat�n alma mesaj�
    private bool playerInArea;              //oyuncu sat�n alma alan�nda m�
    public bool healthRestore, healthUpgrade, isWeapon;     //sat�n al�nacak e�yan�n t�r�
    public int itemCost;                    //sat�n al�nacak e�yan�n fiyat�
    
    [Header("Weapon Buy")]
    public Weapon[] weaponsToBuy;           //sat�labilecek silahlar� tutan liste. Rastgele bir tanesi se�ilir
    private Weapon theWeapon;               //se�ilen silah
    public SpriteRenderer weaponSprite;     //se�ilen silah�n market alan�nda g�z�kmesini sa�layan SpriteRenderer
    public Text info;                       //silah�n fiyat�


    void Start() {
        //e�er sat�lacak �ey silah ise rastgele se�ilen silaha g�re bilgileri g�nceller
        if (isWeapon) {
            int selectedWeapon = Random.Range(0, weaponsToBuy.Length);
            theWeapon = weaponsToBuy[selectedWeapon];


            weaponSprite.sprite = theWeapon.weaponShopSprite;
            info.text = theWeapon.weaponName + "\n - " + theWeapon.itemCost + " ALTIN -";
            itemCost = theWeapon.itemCost;
        }
    }

    void Update() {
        //oyuncu sat�n alma alan� i�erisinde ise e tu�una basarak sat�lan �r�n�n fiyat�ndan
        //fazla alt�na sahipse sat�n alma i�lemi ger�ekle�ir
        if (playerInArea) {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (LevelManager.instance.currentCoins >= itemCost) {
                    LevelManager.instance.SpendCoins(itemCost);

                    SoundManager.instance.purchase.Play();

                    //sat�lan �r�n sa�l�k yneileme ise oyuncunun sa�l���n� tamamen doldur
                    if (healthRestore) {
                        PlayerAttributeController.instance.HealPlayer(PlayerAttributeController.instance.maxHealth);
                    }

                    //sat�lan �r�n sa�l�k y�kseltme ise oyuncunun sa�l���n� kal�c� olarak y�kselt
                    if (healthUpgrade) {
                        PlayerAttributeController.instance.IncreseMaxHealth(3);

                    }

                    //sat�lacsak �r�n silah ise oyuncuya silah� ver
                    if (isWeapon) {
                        Weapon gunClone = Instantiate(theWeapon);
                        gunClone.transform.parent = PlayerController.instance.gunArm;
                        gunClone.transform.position = PlayerController.instance.gunArm.position;
                        gunClone.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        gunClone.transform.localScale = Vector3.one;

                        foreach (var gun in PlayerController.instance.avaliableGuns) {
                            gun.gameObject.SetActive(false);
                        }

                        PlayerController.instance.avaliableGuns.Add(gunClone);
                        PlayerController.instance.currentGunIndex = PlayerController.instance.avaliableGuns.Count - 1;
                        PlayerController.instance.SwitchGun();
                    }


                    gameObject.SetActive(false);
                    playerInArea = false;

                } else {
                    SoundManager.instance.cantBuy.Play();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            buyMessage.SetActive(true);
            playerInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            buyMessage.SetActive(false);
            playerInArea = false;
        }
    }
}
