using UnityEngine;


//Oyuncunun silahlarýndan ateþ edilecek mermiler için script.
public class PlayerBullet : MonoBehaviour {
    public float speed = 7.5f;      //mermi hareket hýzý
    public Rigidbody2D theRB;       //mermi rigidbody komponenti

    public GameObject impactEffect; //merminin temas edilebilir objelerle temasý anýnda aktifleþecek temas efekti.

    public int damageToGive = 5;    //merminin vereceði hasar

    void Update() {
        //merminin hareket yönü ve hýzý belirlenir.
        theRB.velocity = transform.right * speed;
    }

    //Düþmanlarla temas halinde hasar verip temas efektinin aktif olmasýyla yok olur. Diðer durumlarda efekt aktif olur ve yok olur
    private void OnTriggerEnter2D(Collider2D other) {
        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);

        if (other.tag == "Enemy" && other.GetComponent<EnemyController>().health > 0) {
            other.GetComponent<EnemyController>().DamageEnemy(damageToGive);
        }

        if (other.tag == "BossTag") {
            if (other.gameObject.GetComponentInChildren<SpriteRenderer>().isVisible) {
                BossController.instance.TakeDamage(damageToGive);
            }
        }
    }

}
