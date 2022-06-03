using UnityEngine;
using UnityEngine.UI;

//Market alanýný ve marketten eþya alýnmaýsný yöneten Script.

public class ShopItem : MonoBehaviour {
    public SOPlayerData data;

    public GameObject buyMessage;           //satýn alma mesajý
    private bool playerInArea;              //oyuncu satýn alma alanýnda mý
    public bool healthRestore, healthUpgrade, isWeapon;     //satýn alýnacak eþyanýn türü
    public int itemCost;                    //satýn alýnacak eþyanýn fiyatý
    
    [Header("Weapon Buy")]
    public Weapon[] weaponsToBuy;           //satýlabilecek silahlarý tutan liste. Rastgele bir tanesi seçilir
    private Weapon theWeapon;               //seçilen silah
    public SpriteRenderer weaponSprite;     //seçilen silahýn market alanýnda gözükmesini saðlayan SpriteRenderer
    public Text info;                       //silahýn fiyatý


    void Start() {
        //eðer satýlacak þey silah ise rastgele seçilen silaha göre bilgileri günceller
        if (isWeapon) {
            int selectedWeapon = Random.Range(0, weaponsToBuy.Length);
            theWeapon = weaponsToBuy[selectedWeapon];


            weaponSprite.sprite = theWeapon.weaponShopSprite;
            info.text = theWeapon.weaponName + "\n - " + theWeapon.itemCost + " ALTIN -";
            itemCost = theWeapon.itemCost;
        }
    }

    void Update() {
        //oyuncu satýn alma alaný içerisinde ise e tuþuna basarak satýlan ürünün fiyatýndan
        //fazla altýna sahipse satýn alma iþlemi gerçekleþir
        if (playerInArea) {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (LevelManager.instance.currentCoins >= itemCost) {
                    LevelManager.instance.SpendCoins(itemCost);

                    SoundManager.instance.purchase.Play();

                    //satýlan ürün saðlýk yneileme ise oyuncunun saðlýðýný tamamen doldur
                    if (healthRestore) {
                        PlayerAttributeController.instance.HealPlayer(PlayerAttributeController.instance.maxHealth);
                    }

                    //satýlan ürün saðlýk yükseltme ise oyuncunun saðlýðýný kalýcý olarak yükselt
                    if (healthUpgrade) {
                        PlayerAttributeController.instance.IncreseMaxHealth(3);

                    }

                    //satýlacsak ürün silah ise oyuncuya silahý ver
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
