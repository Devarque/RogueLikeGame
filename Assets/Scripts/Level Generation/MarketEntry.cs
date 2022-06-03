using UnityEngine;

//Oyuncu market kap�s��n�n trigger�n� aktif etti�inde E tu�una basarak
//Market alan�na ���nlan�r.

public class MarketEntry : MonoBehaviour
{
    public static MarketEntry instance;     //market alan�
    private bool playerPresent = false;     //oyuncu var m� kontrol�
    private bool pressed = false;           //tu�a bas�ld� m� kontrol�

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        //Oyuncu trigger alan� �zerinde ise e tu�una basarak market alan�na ge�i� yapar
        if (playerPresent) {
            if (Input.GetKeyDown(KeyCode.E)) {
                PlayerController.instance.marketHoldPosition = gameObject.transform.position;
                PlayerController.instance.transform.position = MarketExit.instance.transform.position;
                pressed = true;
                SoundManager.instance.bossTp.Play();
            }
        }
        //tu�a bir kez bas�ld�ktan sonra oyuncunun konumu art�k market alan� i�erisinde bir konum
        //olaca��ndan dolay� tekrar e ye bas�p ayn� yere ���nlanmams� i�in market giri� alan�ndaki
        //de�i�kenler false yap�l�r. 
        if (pressed) {
            playerPresent = false;
            pressed = false;
        }
    }

    //trigger i�lemini kontrol eder
    //oyuncu girdi�inde market alan�na ge�i� i�in onay verilir.
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            if (!playerPresent) {
                playerPresent = true;
            }
        }
    }

    //oyuncu alandan ayr�ld���nda market alan�na e tu�una basarak ge�i�
    //yapamamas� i�in onay kald�r�l�r.
    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player") {
            playerPresent = false;
        }
    }
}
