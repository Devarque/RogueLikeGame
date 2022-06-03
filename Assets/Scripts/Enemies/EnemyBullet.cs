using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//D��manlar�n ate� edece�i mermiler i�in script.

public class EnemyBullet : MonoBehaviour
{
    public float speed = 4;     //Mermi hareket h�z�
    private Vector3 direction;  //mermi hareket y�n�

    void Start()
    {
        direction = PlayerController.instance.transform.position - transform.position;
        direction.Normalize();
    }

    //Mermi ate� edilen y�nde harekete ba�lar
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    //Mermi oyuncu tagine sahip olan nesneyle temas ger�ekle�tirdi�inde oyuncunun
    //sa�l��� 1 azalt�l�r. Mermi nesnesi sahneden silinir.
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            PlayerAttributeController.instance.DamagePlayer(1);
        }
        Destroy(gameObject);
    }
}
