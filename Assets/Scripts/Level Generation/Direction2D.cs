using System.Collections.Generic;
using UnityEngine;

//RandomWalker algoritmas�n�n �al��mas� s�ras�nda se�ilecek y�nleri tutan statik s�n�f ve statik listeler.
//Temel d�rt y�n ve �apraz y�nler farkl� listelerde tutulur.

public static class Direction2D {
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), //yukar�
        new Vector2Int(1, 0), //sa�
        new Vector2Int(0, -1), //a�a��
        new Vector2Int(-1, 0) // sol
    };

    public static List<Vector2Int> allDirectionsList = new List<Vector2Int>
    {
        //�apraz y�nler de dahil olmak �zere y�nler
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1 ,0)
    };

    //Temel d�rt y�nden rastgele se�ilen bir tane y�n� d�nd�ren fonksiyon
    public static Vector2Int GetRandomCardinalDirection() {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];      
    }

}
