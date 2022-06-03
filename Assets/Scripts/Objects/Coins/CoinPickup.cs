using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Düþmanlardan veya kutulardan düþen altýnlarýn oyuncu tarafýndan toplanabilmesi için
//Oluþturulan Script

public class CoinPickup : MonoBehaviour {
    public SOPlayerData playerData;     //Oyuncu altýn verilerini güncellemek için SOPlayerData nesnesi

    public int coinValue = 1;           //Altýn deðeri

    public float waitTimeForCollect;    //toplayabilmek için oluþtuktan sonra bekleme süresi

    public float moveSpeed = 3;         //oluþma anýnda altýnlar rastgele bir yönde kýsa bir mesafe hareket ederler. O hareketin hýzý
    private Vector3 moveDirection;      //hareket yönü
    public float deacceleration = 5f;   //yavaþlama ivmesi

    private bool canMove = true;

    private void Start() {
        //ALtýn düþtüðü anda rastgele x ve y yönleri seçilir

        moveDirection.x = Random.Range(-moveSpeed, moveSpeed);
        moveDirection.y = Random.Range(-moveSpeed, moveSpeed);
    }


    //Seçilen yönde girilen süre kadar herhangi bir nesne ile temas olana kadar hareket ederler.
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


    //Herhangi bir nesne ile temaslarý sýrasýnda hareket hýzlarý sýfýrlanýr. 
    //Duvarlar ve tuzaklarýn içerisine altýnlarýn girmemesi saðlanýr.
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
