using System.Collections.Generic;
using UnityEngine;

//RandomWalker algoritmasýnýn çalýþmasý sýrasýnda seçilecek yönleri tutan statik sýnýf ve statik listeler.
//Temel dört yön ve çapraz yönler farklý listelerde tutulur.

public static class Direction2D {
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), //yukarý
        new Vector2Int(1, 0), //sað
        new Vector2Int(0, -1), //aþaðý
        new Vector2Int(-1, 0) // sol
    };

    public static List<Vector2Int> allDirectionsList = new List<Vector2Int>
    {
        //çapraz yönler de dahil olmak üzere yönler
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1 ,0)
    };

    //Temel dört yönden rastgele seçilen bir tane yönü döndüren fonksiyon
    public static Vector2Int GetRandomCardinalDirection() {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];      
    }

}
