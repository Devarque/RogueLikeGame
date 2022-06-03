using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//OYUN ÝÇÝ UI PANELÝNÝ KONTROL EDEN SCRIPT  


public class UIController : MonoBehaviour {
    public static UIController instance;
    public SOPlayerData data;

    [Header("Health")]
    public Slider healthSlider;
    public Text healthText;

    [Header("Dash")]
    public Slider dashSlider;
    public Slider unlimitedDashSlider;

    [Header("Invincibility")]
    public Slider invincSlider;

    [Header("Key")]
    public Text keyText;

    [Header("Event Screens/Scenes")]
    public GameObject deathScreen;
    public string newGameScene, mainMenuScene;
    public GameObject pauseMenu;
    public GameObject mapDisplay;
    public Text bossTpWarning;

    [Header("Coin")]
    public Text coinText;

    [Header("Weapon")]
    public Text ammo;
    public Image currentGun;
    public Text gunText;

    [Header("After Game Data")]
    public Text totalDamageTakenText;
    public Text totalDamageDealtText;
    public Text totalEnemyKilledText;

    [Header("Boss")]
    public Slider bossHealthBar;


    private void Awake() {
        instance = this;
    }

    private void Update() {
        totalDamageDealtText.text = data.totalDamageDealt.ToString();
        totalDamageTakenText.text = data.totalDamageTaken.ToString();
        totalEnemyKilledText.text = data.totalEnemyKilled.ToString();
        healthSlider.value = data.currentHealth;
        healthText.text = data.currentHealth.ToString() + " / " + data.maxHealth.ToString();

    }


    void Start() {
        invincSlider.gameObject.SetActive(true);
        unlimitedDashSlider.gameObject.SetActive(false);
    }

    //oyundan çýk tuþuna basýlýrsa oyun kapatýlýr
    public void ExitGame() {
        Application.Quit();
    }

    //yeni oyun butonu
    public void NewGame() {
        Time.timeScale = 1f;

        Destroy(PlayerController.instance.gameObject);
        Destroy(CameraFollow.instance.gameObject);
        SceneManager.LoadScene(newGameScene);
    }

    //ana menüye dönüþ butonu
    public void ReturnMainMenu() {
        Time.timeScale = 1f;

        Destroy(PlayerController.instance.gameObject);
        Destroy(CameraFollow.instance.gameObject);
        SceneManager.LoadScene(mainMenuScene);

    }


    //oyunu devam ettirme butonu
    public void Resume() {
        InGameCounter.instance.ContinueTimer();
        LevelManager.instance.PauseUnpause();
    }
}
