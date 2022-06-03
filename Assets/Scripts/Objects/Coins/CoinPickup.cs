using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//D��manlardan veya kutulardan d��en alt�nlar�n oyuncu taraf�ndan toplanabilmesi i�in
//Olu�turulan Script

public class CoinPickup : MonoBehaviour {
    public SOPlayerData playerData;     //Oyuncu alt�n verilerini g�ncellemek i�in SOPlayerData nesnesi

    public int coinValue = 1;           //Alt�n de�eri

    public float waitTimeForCollect;    //toplayabilmek i�in olu�tuktan sonra bekleme s�resi

    public float moveSpeed = 3;         //olu�ma an�nda alt�nlar rastgele bir y�nde k�sa bir mesafe hareket ederler. O hareketin h�z�
    private Vector3 moveDirection;      //hareket y�n�
    public float deacceleration = 5f;   //yava�lama ivmesi

    private bool canMove = true;

    private void Start() {
        //ALt�n d��t��� anda rastgele x ve y y�nleri se�ilir

        moveDirection.x = Random.Range(-moveSpeed, moveSpeed);
        moveDirection.y = Random.Range(-moveSpeed, moveSpeed);
    }


    //Se�ilen y�nde girilen s�re kadar herhangi bir nesne ile temas olana kadar hareket ederler.
    private void Update() {
        if (waitTimeForCollect > 0) {
            waitTimeForCollect -= Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (canMove) {
            transform.position += moveDirection * Time.deltaTime;
            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, deacceleration * Time.deltaTime);
        }
    }


    //Herhangi bir nesne ile temaslar� s�ras�nda hareket h�zlar� s�f�rlan�r. 
    //Duvarlar ve tuzaklar�n i�erisine alt�nlar�n girmemesi sa�lan�r.
    private void OnTriggerEnter2D(Collider2D other) {
        canMove = false;

        if (other.tag == "Player" && waitTimeForCollect <= 0) {
            LevelManager.instance.GetCoins(coinValue);
            playerData.currentCoins += coinValue;
            SoundManager.instance.coin.Play();
            Destroy(gameObject);
        } else if (other.tag == "Wall") {
            moveDirection = Vector3.zero;
        }
    }
}
