using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//OYUN ANA EKRAN PANEL�N� Y�NETEN SCR�PT

public class MainMenu : MonoBehaviour {
    public string loadLevel;                //butona bas�nca y�klenilecek level ad�
    public GameObject settingsCanvas;       //ayarlar butonuna bas�nca a��lan ayarlar alan�
    public GameObject infoCanvas;           //bilgi butonuna bas�nca a��lan bilgi alan�

    public ScriptableObjectForCorridorWalker corridorParameters;//ayarlar k�sm�ndan harita olu�turma ayarlar�n� de�i�tirmek i�in koridor datas�
    public ScriptableObjectForRandomWalker roomsParameters;     //ayarlar k�sm�ndan harita olu�turma ayarlar�n� de�i�tirmek i�in oda datas�

    public Slider minRoomCountSlider;
    public Slider minRoomIterations;
    public Slider maxRoomIterations;
    public Slider minRoomWalkerLength;
    public Slider maxRoomWalkerLength;

    public Text odaSayisi;
    public Text minItr;
    public Text maxItr;
    public Text minWalk;
    public Text maxWalk;

    public Text bildiri;

    private float fadeLength = 2f;

    void Start() {
        maxRoomIterations.minValue = 10;
        maxRoomIterations.maxValue = 30;

        minRoomIterations.minValue = 5;


        maxRoomWalkerLength.minValue = 30;
        maxRoomWalkerLength.maxValue = 90;

        minRoomWalkerLength.minValue = 10;


        minRoomCountSlider.value = corridorParameters.minRoomCount;
        odaSayisi.text = minRoomCountSlider.value.ToString();

        minRoomIterations.value = roomsParameters.minIterations;
        minItr.text = minRoomIterations.value.ToString();

        maxRoomIterations.value = roomsParameters.maxIterations;
        maxItr.text = maxRoomIterations.value.ToString();

        minRoomWalkerLength.value = roomsParameters.minWalkLength;
        minWalk.text = minRoomWalkerLength.value.ToString();

        maxRoomWalkerLength.value = roomsParameters.maxWalkLength;
        maxWalk.text = maxRoomWalkerLength.value.ToString();
    }

    void Update() {
        minRoomIterations.maxValue = maxRoomIterations.value - 1;
        minRoomWalkerLength.maxValue = maxRoomWalkerLength.value - 1;

        odaSayisi.text = minRoomCountSlider.value.ToString();
        minItr.text = minRoomIterations.value.ToString();
        maxItr.text = maxRoomIterations.value.ToString();
        minWalk.text = minRoomWalkerLength.value.ToString();
        maxWalk.text = maxRoomWalkerLength.value.ToString();

        if (bildiri.gameObject.activeInHierarchy) {
            fadeLength -= Time.deltaTime;

            if (fadeLength <= 0) {
                bildiri.gameObject.SetActive(false);
                fadeLength = 2f;
            }
        }
    }

    public void StartGame() {
        SceneManager.LoadScene(loadLevel);
    }

    public void Settings() {
        settingsCanvas.SetActive(true);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void SaveSettings() {
        corridorParameters.minRoomCount = (int)minRoomCountSlider.value;

        roomsParameters.minIterations = (int)minRoomIterations.value;
        roomsParameters.maxIterations = (int)maxRoomIterations.value;

        roomsParameters.minWalkLength = (int)minRoomWalkerLength.value;
        roomsParameters.maxWalkLength = (int)maxRoomWalkerLength.value;

        bildiri.gameObject.SetActive(true);
    }

    public void BackToMenu() {
        settingsCanvas.SetActive(false);
        infoCanvas.SetActive(false);

        if (bildiri.gameObject.activeInHierarchy) {
            bildiri.gameObject.SetActive(false);
        }
    }

    public void InfoScreen() {
        infoCanvas.SetActive(true);
    }

}
