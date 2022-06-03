using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Oyun s�ras�nda kullac�n�n istatistiklerini tutan Scriptable Object nesnesi.
//Oyuncu �ld���nde veya oyunu bitirdi�inde ekranda de�erlerin g�sterilmesi i�in olu�turuldu.

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data")]
public class SOPlayerData : ScriptableObject {
    public int currentHealth = 6;   //Oyuncunun anl�k sa�l���   
    public int maxHealth = 6;       //Oyuncunun maksiumum sa�l���
    public int currentCoins = 0;    //Oyuncunun anl�k alt�n say�s�

    public TimeSpan playTime;       //Oyun oynama s�resi. Oynamaya ba�land��� andan itibaren artmaya ba�lar

    public string totalTime;        
    public int totalEnemyKilled = 0;    //�ld�r�len toplam d��man say�s�
    public int totalDamageTaken = 0;    //Al�nan toplam hasar
    public int totalDamageDealt = 0;    //Verilen toplam hasar


    
}


