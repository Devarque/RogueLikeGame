using UnityEngine;
using Random = UnityEngine.Random;

//Oyuncunun sa�l���n� artt�rmay� sa�layan sa�l�k iksiri i�in olu�turulmu� Script
//�ksirin al�nmas� halinde oyuncunun sa�l��� belirlenen miktarda yenilenir.

//Di�er toplan�labilir nesnelerin sahip oldu�u �zelliklere sahiptir. Detaylar i�in CoinPickup.cs incelenebilir

public class PotionHealth : MonoBehaviour {
    public bool isBig;
    public int smallHealAmount = 1;
    public float waitTimeForCollect = 0.5f;

    public float moveSpeed = 3;
    private Vector3 moveDirection;
    public float deacceleration = 5f;

    private bool canMove = true;

    private void Start() {
        moveDirection.x = Random.Range(-moveSpeed, moveSpeed);
        moveDirection.y = Random.Range(-moveSpeed, moveSpeed);
    }

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

    private void OnTriggerEnter2D(Collider2D other) {
        canMove = false;
        if (other.tag == "Player" && waitTimeForCollect <= 0) {
            if (isBig) {
                PlayerAttributeController.instance.HealPlayer(PlayerAttributeController.instance.maxHealth);
            } else {
                PlayerAttributeController.instance.HealPlayer(smallHealAmount);
            }
            SoundManager.instance.healthRefill.Play();
            Destroy(gameObject);
        }
    }

}
