using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Oyuncu koridora girdiði anda aktif olan trigger sayesinde koridor büyük haritada gözükür.
//Koridoru kapatan maske deaktif edilir.
public class CorridorCollider : MonoBehaviour
{
    [SerializeField] private Corridor corridor;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            corridor.corridorMask.gameObject.SetActive(false);
        }
    }

}
