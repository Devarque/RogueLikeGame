using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//oyun içerisnde geçen sürenin data ya aktarýlmasýný saðlayan script
//ekranda gözüken UI oyun içi syaç alaný da güncellenir
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
