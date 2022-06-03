using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Oyuncu koridora girdi�i anda aktif olan trigger sayesinde koridor b�y�k haritada g�z�k�r.
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
