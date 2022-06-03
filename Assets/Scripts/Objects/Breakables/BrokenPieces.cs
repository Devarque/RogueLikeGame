using UnityEngine;

//Kutular k�r�ld�ktan sonra olu�acak olan k�r�k par�alar i�in olu�turulan Script

public class BrokenPieces : MonoBehaviour {
    public float moveSpeed = 3f;            //Par�alar�n etrafa sa��lma h�z�
    private Vector3 moveDirection;          //Par�alar�n hareket y�n�

    public float deacceleration = 5f;       //par�alar�n yava�lama ivmesi

    public float lifeTime = 3f;             //par�alar�n saydamla�ma s�resi

    public SpriteRenderer theSR;            //saydamla�ma i�in kullan�lacak olan SpriteRenderer
    public float fadeOutSpeed = 2.5f;       //saydamla�ma h�z�

    void Start() {
        moveDirection.x = Random.Range(-moveSpeed, moveSpeed);
        moveDirection.y = Random.Range(-moveSpeed, moveSpeed);
    }

    //Par�alar olu�tuktan bellir bir s�re sonra yava��a saydamla�arak oyundan silinirler.
    void Update() {
        transform.position += moveDirection * Time.deltaTime;
        moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, deacceleration * Time.deltaTime);
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0) {
            theSR.color = new Color(theSR.color.r, theSR.color.g, theSR.color.b, Mathf.MoveTowards(theSR.color.a, 0f, fadeOutSpeed * Time.deltaTime));
            if (theSR.color.a <= 0f) {
                Destroy(gameObject);
            }
        }
    }
}
