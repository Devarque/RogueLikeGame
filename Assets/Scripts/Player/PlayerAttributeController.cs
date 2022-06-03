using UnityEngine;
using UnityEngine.SceneManagement;

//Oyuncunun dash, hasar almama, sa�l�k iyile�me, hasar alma gibi durumlar�n� kontrol eden Script

public class PlayerAttributeController : MonoBehaviour {
    public SOPlayerData data;           //Oyuncu data s�n�f�
    public static PlayerAttributeController instance;  

    public int currentHealth;           //oyuncunun anl�k sa�l���
    public int maxHealth;               //oyuncunun maksimum sa�l���
    public float invincibleAfterDamageTaken = 1f; //oyuncu herhangi bir kaynaktan hasar ald���nda, girilen s�re kadar o s�re s�ras�nda hasar almaz
    public float invincibleCount;       //hasar almama s�re sayac�

    private void Awake() {
        instance = this;
        //E�er oyuncu Oyun oynama sahnesinde ise anl�k sa�l��� ve maksimum sa�l��� girilen de�erlere e�itlenir
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameScene")) {
            data.maxHealth = 6;
            data.currentHealth = data.maxHealth;
        }
    }

    void Start() {
        //E�er oyuncu oyun oynama sahnesinde ise data g�ncellemeleri yap�l�r
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
        //oyuncu yeni bir oyuna ba�lad���nda 2 saniye hasara kar�� dayan�kl� olur.
        MakeInvincible(2f);

    }

    void Update() {
        //Oyuncunun hasar almama durumunu kontrol eder, e�er hasar almama aktifse hem UI slider� g�ncellenir hem de kalkan ����� aktif edlir.
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

    //Oyuncu girilen de�er kadar sa�l���n� kaybeder.
    //Hasar alma sesi oynat�l�r.
    //E�er oyuncunun sa�l��� 0 olursa oyun sonu ekran� y�klenir, oyun oynama s�re sayac� durdurulur.
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

    //Oyuncuyu girilen s�re kadar t�m hasarlara kar�� kalkanla korur
    public void MakeInvincible(float length) {
        UIController.instance.invincSlider.maxValue = length;
        UIController.instance.invincSlider.gameObject.SetActive(true);
        PlayerController.instance.shield.SetActive(true);
        invincibleCount = length;
        PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, .5f);

    }

    //Oyuncuyu girilen de�er kadar iyile�tirir
    public void HealPlayer(int healAmount) {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    //Oyuncunun maksimum sa�l���n� girilen de�er kadar artt�r�r.
    public void IncreseMaxHealth(int amount) {
        maxHealth += amount;

        data.maxHealth = maxHealth;
        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
    }
}
