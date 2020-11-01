using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Board : MonoBehaviour
{
    public static int ROW_NUM = 8; // number of tiles per row
    public static int COL_NUM = 16; // number of tiles per col
    
    public Tile tilePrefab;
    public Tile spacePrefab;
    public Tile[] board; // a flattened 8 x 16 grid
    public TextAsset wordlistTxt;
    
    private HashSet<string> wordSet;
    private int score;
    private int clears;
    private int selectedIndex;
    private int endIndex;
    private bool mouseDown;

    public static int Column(int index)
    {
        return index % ROW_NUM;
    }

    public static int Row(int index)
    {
        return index / ROW_NUM;
    }

    private int Index(int row, int column)
    {
        if (0 <= row && row < COL_NUM && 0 <= column && column < ROW_NUM)
        {
            return row * ROW_NUM + column;
        }
        else
        {
            return -1;
        }
    }

    private int IndexOfPointer()
    {
        Vector2 pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int row = (int) Mathf.Floor(pointerPosition.y) + 8;
        int col = (int) Mathf.Floor(pointerPosition.x) + 4;
        //Debug.Log("row: " + row);
        //Debug.Log("col: " + col);
        return Index(row, col);
    }

    // Start is called before the first frame update
    void Start()
    {
        wordSet = new HashSet<string>(wordlistTxt.text.Split('\n'));
        // init array and list
        board = new Tile[ROW_NUM * COL_NUM]; // 8 x 16 grid
        for (int i = 0; i < board.Length; i++) 
        {
            // fill board with spaces
            PlaceTile(spacePrefab, i);
        }
        
        // init
        score = 0;
        clears = 0;
        mouseDown = false;
        selectedIndex = -1;
        endIndex = -1;
        
        StartCoroutine(SpawnTiles());
    }

    // Update is called once per frame
    void Update()
    {
        // process input
        bool oldMouseDown = mouseDown;
        int indexOfPointer = UpdateMouseDown(); // sets mouseDown

        // process pointer input if on a letter tile, otherwise ignore
        if (indexOfPointer >= 0)
        { 
            bool newClick = !oldMouseDown && mouseDown;
            bool newUnclick = oldMouseDown && !mouseDown;

            int oldSelectedIndex = selectedIndex; // both are -1 if not set
            if (newClick) selectedIndex = indexOfPointer;

            int oldEndIndex = endIndex; // both are -1 if not set
            if (mouseDown && selectedIndex >= 0) UpdateEndIndex(indexOfPointer); // sets endIndex

            //DEBUG
            if (endIndex >= 0 && selectedIndex == -1) Debug.Log("WE HAVE A PROBLEM");
            
            //update board
            if (newClick)
            {
                // if no tiles were previously selected and a space is selected
                if (oldSelectedIndex == -1 && selectedIndex >= 0 && board[selectedIndex].letter == Tile.SPACE)
                {
                    selectedIndex = endIndex = -1; // reset
                    //Debug.Log("Space tile clicked without prior");
                }
                // if tiles were previously selected and the first tile is clicked
                else if (oldSelectedIndex >= 0 && selectedIndex == oldSelectedIndex)
                {
                    UpdateTiles(oldSelectedIndex, oldEndIndex, Deselect); // deselect the tiles
                    selectedIndex = endIndex = -1; // reset
                }
                // if tiles were previously selected and the last tile is clicked
                else if (oldSelectedIndex >= 0 && selectedIndex == oldEndIndex)
                {
                    CheckWord(oldSelectedIndex, oldEndIndex);
                    selectedIndex = endIndex = -1; // reset
                }
                // if tiles were previously selected and new first tile is clicked
                else if (oldSelectedIndex >= 0)
                {
                    //INITIATE SWAP HERE! REPLACE CODE BELOW. SWAPPING JUST ONE FOR NOW
                    if (selectedIndex >= 0 && oldSelectedIndex == oldEndIndex) // only one tile selected
                    {
                        Swap(selectedIndex, oldSelectedIndex);
                        UpdateTiles(selectedIndex, endIndex, Deselect);
                        selectedIndex = endIndex = -1; // reset
                    }
                    else
                    {
                        UpdateTiles(oldSelectedIndex, oldEndIndex, Deselect); // deselect the old tiles
                        UpdateTiles(selectedIndex, endIndex, Select); // select the new tile
                    }
                }
                // no tiles previously selected and new first tile is clicked
                else if (selectedIndex >= 0)
                {
                    UpdateTiles(selectedIndex, endIndex, Select); // select the new tile
                }
            }
            else if (mouseDown)
            {
                // if a tile was previously selected and that tile was not a space tile
                if (selectedIndex >= 0 && board[selectedIndex].letter != Tile.SPACE)
                {
                    // update selection with movement of mouse
                    UpdateTiles(selectedIndex, endIndex, Select);
                }
            }
            else if (newUnclick)
            {

            }
            else // mouse not down
            {

            }
        }
    }

    private int UpdateMouseDown()
    {
        int index = IndexOfPointer();

        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
        }
        //Debug.Log("Clicked index " + index);
        return index;
    }

    private void CheckWord(int selectedIndex, int endIndex)
    {
        string wordString = "";
        bool isRow;
        if (Row(selectedIndex) == Row(endIndex))
        {
            isRow = true;
            for (int i = Column(selectedIndex); i <= Column(endIndex); i++)
            {
                wordString += board[Index(Row(selectedIndex), i)].letter;
            }
        }
        else
        {
            isRow = false;
            for (int i = Row(selectedIndex); i >= Row(endIndex); i--)
            {
                wordString += board[Index(i, Column(selectedIndex))].letter;
            }
        }

        if (wordSet.Contains(wordString))
        {
            score += wordString.Length * wordString.Length;
            clears++;

            // check for adjoining words
            if (isRow) UpdateTiles(selectedIndex, endIndex, CheckCol);
            else UpdateTiles(selectedIndex, endIndex, CheckRow);

            Debug.Log("Score: " + score);
            UpdateTiles(selectedIndex, endIndex, ClearTile);
        }
        else
        {
            UpdateTiles(selectedIndex, endIndex, Deselect);
        }
    }

    private void CheckRow(int index)
    {
        int row = Row(index);
        int anchor = Column(index); // this position in string must be included in word
        // make a string
        string rowString = RowString(row);

        //Debug.Log("Checking rowString: '" + rowString + "'");
        (int start, int length) wordIndex  = CheckRowString(0, ROW_NUM, rowString, anchor);

        Debug.Log("Found Word: '" + rowString.Substring(wordIndex.start, wordIndex.length) + "'");

        for (int i = wordIndex.start, j = 0; j < wordIndex.length; i++, j++)
        {
            // replace tiles with blanks
            //PlaceTile(spacePrefab, row * ROW_NUM + i);
        }
    }

    private void CheckCol(int index)
    {

    }

    private void ClearTile(int index)
    {
        PlaceTile(spacePrefab, index);
    }

    private void UpdateEndIndex(int index)
    {
        // for each tile in between selected tile and current tile
        // if in a straight line, top to bottom or left to right, and contiguous
        endIndex = selectedIndex; // they are equal unless extended
        if (Row(selectedIndex) == Row(index))
        {
            for (int i = Column(selectedIndex); i <= Column(index) && board[Index(Row(selectedIndex), i)].letter != Tile.SPACE; i++)
            {
                //board[Index(Row(selectedIndex), i)].Select();
                endIndex = Index(Row(selectedIndex), i);
            }
        }
        else if (Column(selectedIndex) == Column(index))
        {
            for (int i = Row(selectedIndex); i >= Row(index) && board[Index(i, Column(selectedIndex))].letter != Tile.SPACE; i--)
            {
                //board[Index(i, Column(selectedIndex))].Select();
                endIndex = Index(i, Column(selectedIndex));
            }
        }
    }

    private delegate void UpdateTileFunc(int index);
    private void UpdateTiles(int selectedIndex, int endIndex, UpdateTileFunc UpdateTile)
    {
        if (Row(selectedIndex) == Row(endIndex))
        {
            for (int i = Column(selectedIndex); i <= Column(endIndex); i++)
            {
                UpdateTile(Index(Row(selectedIndex), i));
            }
        }
        else
        {
            for (int i = Row(selectedIndex); i >= Row(endIndex); i--)
            {
                UpdateTile(Index(i, Column(selectedIndex)));
            }
        }
    }

    private void Select(int index)
    {
        board[index].Select();
    }

    private void Deselect(int index)
    {
        board[index].SetColor(Color.white);
    }

    IEnumerator SpawnTiles() {
        bool gameOver = false;

        // spawn starting tiles
        for (int i = 0; i < 16; i++)
        {
            int index = RandomSpace();
            PlaceTile(tilePrefab, index);
        }

        while(!gameOver)
        {
            int index = RandomSpace();
            if (index < 0)
            {
                //game over
                Debug.Log("Game Over Man!");
                gameOver = true;
            }
            else
            {
                PlaceTile(tilePrefab, index);
                float waitSeconds = 3f;
                if (clears > 10) waitSeconds = 2.5f;
                if (clears > 20) waitSeconds = 2f;
                if (clears > 30) waitSeconds = 1.5f;
                if (clears > 40) waitSeconds = 1.25f;
                if (clears > 50) waitSeconds = 1f;
                if (clears > 60) waitSeconds = 0.75f;
                if (clears > 70) waitSeconds = 0.5f;
                if (clears > 80) waitSeconds = 0.25f;
                yield return new WaitForSeconds(waitSeconds);
            }
        }
    }

    // spawn a new tile from given prefab at given board index and initiate and add to board
    // destroy tile that was in that position if there was one
    private void PlaceTile(Tile prefab, int index)
    {
        Tile oldTile = board[index];
        int x = Tile.GetTileX(index);
        int y = Tile.GetTileY(index);
        Tile newTile = (Tile) Instantiate(prefab, new Vector2(x, y), Quaternion.identity);
        newTile.boardIndex = index;
        board[index] = newTile;
        if (oldTile != null) Destroy(oldTile.gameObject);
    }

    private int RandomSpace()
    {
        int index = -1;
        List<int> remainingSpaces = new List<int>();
        foreach (Tile tile in board)
        {
            if (tile.letter == Tile.SPACE)
            {
                remainingSpaces.Add(tile.boardIndex);
            }
        }

        if (remainingSpaces.Count > 0)
        {
            int random = Random.Range(0, remainingSpaces.Count);
            index = remainingSpaces[random];
        }
        return index;
    }

    private void Swap(int index, int otherIndex)
    {
        // swap positions of tile on array board
        Tile temp = board[index];
        board[index] = board[otherIndex];
        board[otherIndex] = temp;

        // move tiles visually
        board[index].MoveTile(index);
        board[otherIndex].MoveTile(otherIndex);

        //CheckRow(Row(selectedTile.boardIndex));
        //CheckRow(Row(tile.boardIndex));
    }

    private string RowString(int row)
    {
        string rowString = "";
        for (int i = row * ROW_NUM; i < row * ROW_NUM + ROW_NUM; i++)
        {
            rowString += board[i].letter;
        }
        return rowString;
    }

    private string ColString(int col)
    {
        string colString = "";
        for (int i = col; i < col + COL_NUM * ROW_NUM; i += ROW_NUM)
        {
            colString += board[i].letter;
        }
        return colString;
    }

    private (int start, int length) CheckRowString(int start, int length, string rowString, int anchor)
    {
        for (int i = length; i > 1; i--)
        {
            for (int j = 0; j + i <= rowString.Length; j++)
            {
                //Debug.Log("Checking Word: '" + rowString.Substring(j, i) + "'");
                if (j <= anchor && anchor < j + i && wordSet.Contains(rowString.Substring(j, i)))
                {
                    //Debug.Log("Found Word: '" + rowString.Substring(j, i) + "'");
                    return (j, i); // word found which contains anchor
                }
            }
        }
        //Debug.Log("No word found.");
        return (0, 0); // no word found
    }

    /*
    public void OldCheckRow(int row)
    {
        // make a string
        string rowString = RowString(row);

        //Debug.Log("Checking rowString: '" + rowString + "'");
        WordIndex wordIndex = CheckRowString(0, ROW_NUM, rowString);

        Debug.Log("Found Word: '" + rowString.Substring(wordIndex.start, wordIndex.length) + "'");

        for (int i = wordIndex.start, j = 0; j < wordIndex.length; i++, j++)
        {
            // replace tiles with blanks
            PlaceTile(spacePrefab, row * ROW_NUM + i);
        }
    }
    */
}
