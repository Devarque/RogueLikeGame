using UnityEngine;
using UnityEngine.SceneManagement;

//Oyuncu boss odasýna girdiðinde aktif olan Script.

public class PlayerEntry : MonoBehaviour
{
    public SOPlayerData data;                //Kullanýcý verileri.

    public static PlayerEntry instance; 
    public bool playerIsPresent = false;    //Oyuncunun odaya girme kontrolü
    public GameObject doors;                //Boss odasý kapýlarý. Kapatmak için
    private float endGameWaitTime = 3.5f;   //Oyun bitince bitiþ ekranýna 

    private void Awake() {
        instance = this;                    
    }
    private void Start() {
        //Oyuncu boss Levelýna girdiðinde oyuncunun saðlýk deðerleri, öncesinde oluþturulan data'dan çeker
        PlayerAttributeController.instance.currentHealth = data.currentHealth;
        PlayerAttributeController.instance.maxHealth = data.maxHealth;
    }

    private void Update() {
        //UI üzerinde saðlýk deðerinin doðru gözükmesini saðlar.
        UIController.instance.healthSlider.maxValue = data.maxHealth;
        UIController.instance.healthSlider.minValue = 0;
        UIController.instance.healthSlider.value = data.currentHealth;

        //Bossun yaþama durumunu kontrol eder. Ölürse aktifleþir.
        if (BossController.instance.bossIsDead) {
            endGameWaitTime -= Time.deltaTime;
            

            SoundManager.instance.bossMusic.Stop();
            if (endGameWaitTime <= 0) {
                Destroy(PlayerController.instance.gameObject);
                Destroy(CameraFollow.instance.gameObject);
                SceneManager.LoadScene("Victory");
            }
        }
    }

    //Oyunucunun boss odasýna girmesini kontrol eder.
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            doors.SetActive(true);
            playerIsPresent = true;
            if (BossController.instance.gameObject.activeInHierarchy) {
                SoundManager.instance.BossFight();
                UIController.instance.bossHealthBar.gameObject.SetActive(true);
            }
        }
    }
}
