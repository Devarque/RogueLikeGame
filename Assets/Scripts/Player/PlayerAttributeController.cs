using UnityEngine;
using UnityEngine.SceneManagement;

//Oyuncunun dash, hasar almama, saðlýk iyileþme, hasar alma gibi durumlarýný kontrol eden Script

public class PlayerAttributeController : MonoBehaviour {
    public SOPlayerData data;           //Oyuncu data sýnýfý
    public static PlayerAttributeController instance;  

    public int currentHealth;           //oyuncunun anlýk saðlýðý
    public int maxHealth;               //oyuncunun maksimum saðlýðý
    public float invincibleAfterDamageTaken = 1f; //oyuncu herhangi bir kaynaktan hasar aldýðýnda, girilen süre kadar o süre sýrasýnda hasar almaz
    public float invincibleCount;       //hasar almama süre sayacý

    private void Awake() {
        instance = this;
        //Eðer oyuncu Oyun oynama sahnesinde ise anlýk saðlýðý ve maksimum saðlýðý girilen deðerlere eþitlenir
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene")) {
            data.maxHealth = 6;
            data.currentHealth = data.maxHealth;
        }
    }

    void Start() {
        //Eðer oyuncu oyun oynama sahnesinde ise data güncellemeleri yapýlýr
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene")) {
            maxHealth = data.maxHealth;
            currentHealth = data.currentHealth;
            data.currentCoins = 0;
            data.totalDamageDealt = 0;
            data.totalDamageTaken = 0;
            data.totalEnemyKilled = 0;

            UIController.instance.healthSlider.maxValue = maxHealth;
            UIController.instance.healthSlider.value = currentHealth;
            UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        }
        //oyuncu yeni bir oyuna baþladýðýnda 2 saniye hasara karþý dayanýklý olur.
        MakeInvincible(2f);

    }

    void Update() {
        //Oyuncunun hasar almama durumunu kontrol eder, eðer hasar almama aktifse hem UI sliderý güncellenir hem de kalkan ýþýðý aktif edlir.
        if (invincibleCount > 0) {
            invincibleCount -= Time.deltaTime;
            UIController.instance.invincSlider.value = invincibleCount;
            if (invincibleCount <= 0) {
                PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, 1f);
                UIController.instance.invincSlider.gameObject.SetActive(false);
                PlayerController.instance.shield.SetActive(false);
            }

        }
        data.currentHealth = currentHealth;
    }

    //Oyuncu girilen deðer kadar saðlýðýný kaybeder.
    //Hasar alma sesi oynatýlýr.
    //Eðer oyuncunun saðlýðý 0 olursa oyun sonu ekraný yüklenir, oyun oynama süre sayacý durdurulur.
    public void DamagePlayer(int damage) {
        if (invincibleCount <= 0) {
            PlayerController.instance.shield.SetActive(true);
            UIController.instance.invincSlider.maxValue = invincibleAfterDamageTaken;
            currentHealth -= damage;
            SoundManager.instance.playerDamage.Play();
            data.currentHealth = currentHealth;
            data.totalDamageTaken += damage;
            UIController.instance.invincSlider.gameObject.SetActive(true);
            invincibleCount = invincibleAfterDamageTaken;

            PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, .5f);

            if (currentHealth <= 0) {
                SoundManager.instance.PlayGameOver();
                PlayerController.instance.gameObject.SetActive(false);
                UIController.instance.deathScreen.SetActive(true);
                InGameCounter.instance.StopTimer();
                BossController.instance.gameObject.SetActive(false);
            }
            UIController.instance.healthSlider.value = currentHealth;
            UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        }
    }

    //Oyuncuyu girilen süre kadar tüm hasarlara karþý kalkanla korur
    public void MakeInvincible(float length) {
        UIController.instance.invincSlider.maxValue = length;
        UIController.instance.invincSlider.gameObject.SetActive(true);
        PlayerController.instance.shield.SetActive(true);
        invincibleCount = length;
        PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, .5f);

    }

    //Oyuncuyu girilen deðer kadar iyileþtirir
    public void HealPlayer(int healAmount) {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    //Oyuncunun maksimum saðlýðýný girilen deðer kadar arttýrýr.
    public void IncreseMaxHealth(int amount) {
        maxHealth += amount;

        data.maxHealth = maxHealth;
        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
    }
}
