using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//B�y�k haritan�n a��lmas� ile birlikte daha �ncesinde ziyaret edilmi� odalara harita �zerinden mouse ile t�klama yap�larak
//���nlanma i�lemlerini ger�ekle�tiren Script.

public class TeleporterCamera : MonoBehaviour
{
    public Camera mainCamera, bigMapCamera; //Ana kamera ve b�y�k harita (���nlanma) kameras�
    private bool bigMapActive = false;      //M tu�una bas�ld���nda ���nlanma ekran�n�n ekranda g�z�kmesini kontrol eden de�i�ken.
    public Transform playerTransform;       //Haritada oyuncuyu k�rm�z� bir nokta olarak g�stermek i�in oyuncunun pozisyonunu tutan transform.
    public GameObject uielements;           //UI g�ncellemeleri i�in, UI nesnesi
    public GameObject returngame;           //M tu�una bas�ld�ktan sonra oyuna tekrar d�nebilmek i�in 
    public GameObject pauseMenu;            //ESC ile oyun durdurulmu�sa haritan�n M ile a��lmamas� i�in, durdurma ekran� nesnesi.


    void Update()
    {
        //E�er oyun durdurulmam��sa ve b�y�k harita aktifse, harita �zerinden t�klan�lan odaya ���nlanmay� ger�ekle�tirir.
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

        //M tu�u ile birlikte b�y�k haritay� (���nlanma ekran�) a�ar veya kapat�r.
        if (Input.GetKeyDown(KeyCode.M)) {
            if (!bigMapActive) {
                ActivateBigMap();
            } else {
                DeactiveBigMap();
            }
        }
    }

    //I��nlanma ekran�n� a�ar. Oyuncu ���nlanma ekran�nda hareket edemez, oyun durdurulur.
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

    //I��nlanma ekran�n�n kapat�r. Oyuncu ���nlanma ekran�n�n kapat�lmas�yla birlikte tekrar hareket edebilir
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
