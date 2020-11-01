using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Sprite[] tileSpriteArray;
    public char letter;
    public int boardIndex;

    public static char SPACE = ' ';

    private static char[] LETTER_ARRAY = { 'j', 'j', 'k', 'k', 'q', 'q', 'x', 'x', 'z', 'z', 'b', 'b', 'b', 'c', 'c', 'c', 'f', 'f', 'f', 'h', 'h', 'h', 'm', 'm', 'm', 'p', 'p', 'p', 'v', 'v', 'v', 'w', 'w', 'w', 'y', 'y', 'y','g', 'g', 'g', 'g','l', 'l', 'l', 'l', 'l', 'd', 'd', 'd', 'd', 'd', 'd', 's', 's', 's', 's', 's', 's', 'u', 'u', 'u', 'u', 'u', 'u', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 'n', 't', 't', 't', 't', 't', 't', 't', 't', 't', 'r', 'r', 'r', 'r', 'r', 'r', 'r', 'r', 'r', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'o', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'i', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e' };
    
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        // set to space tile '-' with transparent sprite
        if (letter == SPACE)
        {

        }
        else // set tile letter and sprite if lettered tile
        {
            letter = LETTER_ARRAY[Random.Range(0, LETTER_ARRAY.Length)];
            sprite.sprite = tileSpriteArray[(int) letter - 97]; // 97 is a
        }
    }

    public void Select()
    {
        sprite.color = Color.blue;
    }

    public void SetColor(Color color)
    {
        sprite.color = color;
    }

    // because of float precision, tile positions have to be corrected a bit after they settle
    private void CorrectPosition()
    {
        Vector2 currentPosition = transform.position;
        float roundedX = Mathf.Round(currentPosition.x);
        if (currentPosition.x != roundedX)
        {
            currentPosition.x = roundedX;
        }
        
        float roundedY = Mathf.Round(currentPosition.y);
        if (currentPosition.y != roundedY)
        {
            currentPosition.y = roundedY;
        }

        transform.position = currentPosition;
    }

    private void ClearWords()
    {
        /*
        Collider2D c = GetComponent<Collider2D>();
        Collider2D[] otherArray = new Collider2D[2];

        ContactFilter2D cf = new ContactFilter2D();
        cf.SetLayerMask(LayerMask.GetMask("Row", "Col"));

        int length = c.OverlapCollider(cf, otherArray);

        WordClear[] rowCol = new WordClear[2];
        rowCol[0] = null;
        rowCol[1] = null;

        for (int i = 0; i < length; i++)
        {
            if (otherArray[i] != null)
            {
                // there should only be 2
                if (otherArray[i].gameObject.layer == 8) 
                {
                    rowCol[0] = otherArray[i].gameObject.GetComponent<WordClear>();
                }
                else if (otherArray[i].gameObject.layer == 9)
                {
                    rowCol[1] = otherArray[i].gameObject.GetComponent<WordClear>();
                }
            }
        }

        for (int i = 0; i < rowCol.Length; i++)
        {
            if (rowCol[i] != null) 
            {
                rowCol[i].ClearWords();
            }
        }
        */
    }

    public void MoveTile(int index)
    {
        boardIndex = index;
        transform.position = new Vector2(GetTileX(), GetTileY());
    }

    public static int GetTileX(int index)
    {
        return Board.Column(index) - 4;
    }

    public static int GetTileY(int index)
    {
        return Board.Row(index) - 8;
    }

    public int GetTileX()
    {
        return Board.Column(boardIndex) - 4;
    }

    public int GetTileY()
    {
        return Board.Row(boardIndex) - 8;
    }
}
