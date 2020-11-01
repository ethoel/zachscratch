using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordClear : MonoBehaviour
{
    public bool isRow;
    private int tileCount;
    public TextAsset wordlistTxt;
    private HashSet<string> wordSet;

    // Start is called before the first frame update
    void Start()
    {
        if (isRow) {
            tileCount = 8;
        }
        else
        {
            tileCount = 16;
        }
        wordSet = new HashSet<string>(wordlistTxt.text.Split('\n'));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {

    }

    public void ClearWords()
    {

        //char[] letterArray = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };

        // initialize row array
        Tile[] orderedTileArray = new Tile[tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            orderedTileArray[i] = null;
        }

        Collider2D c = GetComponent<Collider2D>();
        Collider2D[] tileArray = new Collider2D[tileCount];

        // create contact filter
        //int layer = 0; // default
        //int layermask = 1 << layer; // bit shift layer mask, i.e. 00000001 for layer 0 default
        ContactFilter2D cf = new ContactFilter2D();
        cf.SetLayerMask(LayerMask.GetMask("Tile"));

        int length = c.OverlapCollider(cf, tileArray);
        //Debug.Log("Length " + length + ".");
        for (int i = 0; i < length; i++) {
            Tile currentTile = tileArray[i].gameObject.GetComponent<Tile>();
            //Debug.Log("Tile " + currentTile.letter + ".");
            //letterArray[((int) Mathf.Round(currentTile.transform.position.x)) + 4] = currentTile.letter;
            if (isRow)
            {
                orderedTileArray[((int) Mathf.Round(currentTile.transform.position.x)) + 4] = currentTile;
            }
            else
            {
                //Debug.Log("Y position = " + ((((int) Mathf.Round(currentTile.transform.position.y)) * (-1)) + 7));
                orderedTileArray[(((int) Mathf.Round(currentTile.transform.position.y)) * (-1)) + 7] = currentTile;
            }
        }

        checkRow(orderedTileArray);
    }

    private struct WordIndex
    {
        public int start;
        public int length;
        public WordIndex(int start, int length)
        {
            this.start = start;
            this.length = length;
        }
    }

    private void checkRow(Tile[] row)
    {
        // make a string
        string rowString = "";
        for (int i = 0; i < row.Length; i++)
        {
            if (row[i] != null)
            {
                rowString += row[i].letter;
            }
            else
            {
                rowString += " ";
            }
        }

        //Debug.Log("Checking rowString: '" + rowString + "'");
        WordIndex wordIndex = checkRowString(0, tileCount, rowString);

        //Debug.Log("Found Word: '" + rowString.Substring(wordIndex.start, wordIndex.length) + "'");

        for (int i = wordIndex.start, j = 0; j < wordIndex.length; i++, j++)
        {
            Destroy(row[i].gameObject);
        }
    }

    private WordIndex checkRowString(int start, int length, string rowString)
    {
        for (int i = length; i > 3; i--)
        {
            for (int j = 0; j + i <= rowString.Length; j++)
            {
                //Debug.Log("Checking Word: '" + rowString.Substring(j, i) + "'");
                if (wordSet.Contains(rowString.Substring(j, i)))
                {
                    //Debug.Log("Found Word: '" + rowString.Substring(j, i) + "'");
                    return new WordIndex(j, i); // word found
                }
            }
        }
        //Debug.Log("No word found.");
        return new WordIndex(0, 0); // no word found
    }
}
