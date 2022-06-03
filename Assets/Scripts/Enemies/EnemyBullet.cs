using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Düþmanlarýn ateþ edeceði mermiler için script.

public class EnemyBullet : MonoBehaviour
{
    public float speed = 4;     //Mermi hareket hýzý
    private Vector3 direction;  //mermi hareket yönü

    void Start()
    {
        direction = PlayerController.instance.transform.position - transform.position;
        direction.Normalize();
    }

    //Mermi ateþ edilen yönde harekete baþlar
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    //Mermi oyuncu tagine sahip olan nesneyle temas gerçekleþtirdiðinde oyuncunun
    //saðlýðý 1 azaltýlýr. Mermi nesnesi sahneden silinir.
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            PlayerAttributeController.instance.DamagePlayer(1);
        }
        Destroy(gameObject);
    }
}
