using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//oyun i�erisnde ge�en s�renin data ya aktar�lmas�n� sa�layan script
//ekranda g�z�ken UI oyun i�i sya� alan� da g�ncellenir
public class TotalTime : MonoBehaviour
{
    public Text timer;
    public SOPlayerData data;


    void Update()
    {
        timer.text = data.totalTime;
        if(data.currentHealth <= 0) {
        }
    }
}
