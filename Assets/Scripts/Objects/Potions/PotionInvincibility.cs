using UnityEngine;

//Oyuncunun dash yetene�i s�ras�nda veya hasar ald�ktan sonra veya hasar almama iksirinin al�nmas�ndan sonra
//aktif edilecek hasar almama g��lendiricisidir.
//Belirlenen s�relerde oyuncu hi�bir kaynaktan hi�bir �ekilde hasar almaz.

//Oyun esnas�nda g�rsel olarak hem UI hem de ���kland�rma olarak g�r�lebilir.
//Di�er toplanabilir nesnelerin �zelliklerine sahiptir. CoinPickup.cs incelenebilir.

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
