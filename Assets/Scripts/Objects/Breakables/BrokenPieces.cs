using UnityEngine;

//Kutular kýrýldýktan sonra oluþacak olan kýrýk parçalar için oluþturulan Script

public class BrokenPieces : MonoBehaviour {
    public float moveSpeed = 3f;            //Parçalarýn etrafa saçýlma hýzý
    private Vector3 moveDirection;          //Parçalarýn hareket yönü

    public float deacceleration = 5f;       //parçalarýn yavaþlama ivmesi

    public float lifeTime = 3f;             //parçalarýn saydamlaþma süresi

    public SpriteRenderer theSR;            //saydamlaþma için kullanýlacak olan SpriteRenderer
    public float fadeOutSpeed = 2.5f;       //saydamlaþma hýzý

    void Start() {
        moveDirection.x = Random.Range(-moveSpeed, moveSpeed);
        moveDirection.y = Random.Range(-moveSpeed, moveSpeed);
    }

    //Parçalar oluþtuktan bellir bir süre sonra yavaþça saydamlaþarak oyundan silinirler.
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
