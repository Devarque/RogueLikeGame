using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Büyük haritanýn açýlmasý ile birlikte daha öncesinde ziyaret edilmiþ odalara harita üzerinden mouse ile týklama yapýlarak
//ýþýnlanma iþlemlerini gerçekleþtiren Script.

public class TeleporterCamera : MonoBehaviour
{
    public Camera mainCamera, bigMapCamera; //Ana kamera ve büyük harita (ýþýnlanma) kamerasý
    private bool bigMapActive = false;      //M tuþuna basýldýðýnda ýþýnlanma ekranýnýn ekranda gözükmesini kontrol eden deðiþken.
    public Transform playerTransform;       //Haritada oyuncuyu kýrmýzý bir nokta olarak göstermek için oyuncunun pozisyonunu tutan transform.
    public GameObject uielements;           //UI güncellemeleri için, UI nesnesi
    public GameObject returngame;           //M tuþuna basýldýktan sonra oyuna tekrar dönebilmek için 
    public GameObject pauseMenu;            //ESC ile oyun durdurulmuþsa haritanýn M ile açýlmamasý için, durdurma ekraný nesnesi.


    void Update()
    {
        //Eðer oyun durdurulmamýþsa ve büyük harita aktifse, harita üzerinden týklanýlan odaya ýþýnlanmayý gerçekleþtirir.
        if (!pauseMenu.activeInHierarchy && bigMapActive) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3 mousePos = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null) {
                    if (hit.collider.gameObject.transform.parent != null) {
                        var room = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Room>();
                        if(room != null && PlayerController.instance.room.enemyCount == 0) {
                            var teleporter = room.teleporter;
                            playerTransform.position = teleporter.transform.position;
                            SoundManager.instance.bossTp.Play();
                        }
                    }
                }
            }
        }

        //M tuþu ile birlikte büyük haritayý (ýþýnlanma ekraný) açar veya kapatýr.
        if (Input.GetKeyDown(KeyCode.M)) {
            if (!bigMapActive) {
                ActivateBigMap();
            } else {
                DeactiveBigMap();
            }
        }
    }

    //Iþýnlanma ekranýný açar. Oyuncu ýþýnlanma ekranýnda hareket edemez, oyun durdurulur.
    public void ActivateBigMap() {
        if (!LevelManager.instance.isPaused) {
            bigMapActive = true;
            uielements.SetActive(false);
            returngame.SetActive(true);

            bigMapCamera.enabled = true;
            mainCamera.enabled = false;


            PlayerController.instance.canMove = false;
            Time.timeScale = 0f;
            UIController.instance.mapDisplay.SetActive(false);
        }
    }

    //Iþýnlanma ekranýnýn kapatýr. Oyuncu ýþýnlanma ekranýnýn kapatýlmasýyla birlikte tekrar hareket edebilir
    //oyun devam ettirilir.
    public void DeactiveBigMap() {
        if (!LevelManager.instance.isPaused) {
            bigMapActive = false;
            uielements.SetActive(true);
            returngame.SetActive(false);


            bigMapCamera.enabled = false;
            mainCamera.enabled = true;

            PlayerController.instance.canMove = true;
            Time.timeScale = 1f;
            UIController.instance.mapDisplay.SetActive(true);
        }
    }
}
