using UnityEngine;

//Oyuncunun dash yeteneði sýrasýnda veya hasar aldýktan sonra veya hasar almama iksirinin alýnmasýndan sonra
//aktif edilecek hasar almama güçlendiricisidir.
//Belirlenen sürelerde oyuncu hiçbir kaynaktan hiçbir þekilde hasar almaz.

//Oyun esnasýnda görsel olarak hem UI hem de ýþýklandýrma olarak görülebilir.
//Diðer toplanabilir nesnelerin özelliklerine sahiptir. CoinPickup.cs incelenebilir.

public class PotionInvincibility : MonoBehaviour {
    public float invincibleLength = 2;
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
            SoundManager.instance.shield.Play();
            PlayerAttributeController.instance.MakeInvincible(invincibleLength);
            Destroy(gameObject);
        }
    }


}
