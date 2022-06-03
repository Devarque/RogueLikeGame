using UnityEngine;


//Oyuncunun silahlar�ndan ate� edilecek mermiler i�in script.
public class PlayerBullet : MonoBehaviour {
    public float speed = 7.5f;      //mermi hareket h�z�
    public Rigidbody2D theRB;       //mermi rigidbody komponenti

    public GameObject impactEffect; //merminin temas edilebilir objelerle temas� an�nda aktifle�ecek temas efekti.

    public int damageToGive = 5;    //merminin verece�i hasar

    void Update() {
        //merminin hareket y�n� ve h�z� belirlenir.
        theRB.velocity = transform.right * speed;
    }

    //D��manlarla temas halinde hasar verip temas efektinin aktif olmas�yla yok olur. Di�er durumlarda efekt aktif olur ve yok olur
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
