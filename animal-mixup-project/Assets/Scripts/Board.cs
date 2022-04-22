using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] Dots;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;


    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
       for (int i = 0; i < width; i ++){
            for(int j = 0; j < height; j++)
            {
                Vector2 tempPositon = new Vector2(i, j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPositon, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                int dotToUse = Random.Range(0, Dots.Length);

                int maxIterations = 0;
                while(MatchesAt(i, j, Dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, Dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                GameObject dot = Instantiate(Dots[dotToUse], tempPositon, Quaternion.identity);
                dot.GetComponent<Dot>(). row = j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }

        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if (allDots[column -1, row].tag == piece.tag && allDots[column -2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row -1].tag == piece.tag && allDots[column, row -2].tag == piece.tag)
            {
                return true;
            }
        }else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allDots[column, row - 1].tag == piece.tag && allDots[column, row -2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column-1, row].tag == piece.tag && allDots[column-2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }


    public void DestroyMatches()
    {
        for(int i = 0;i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    Debug.Log("Destroying match at " + i + ", " + j);
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        Debug.Log("Decreasing all dots by 1 row");
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for( int j = 0; j < height; j++)
            {
                if(allDots[i, j] == null)
                {
                    nullCount++;
                }else if(nullCount > 0){
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());

    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, Dots.Length);
                    GameObject piece = Instantiate(Dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    if(allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        Debug.Log("Refilling board...");
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            Debug.Log("Still finding matches, destroying...");
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }

        //are ALL fill board coroutines done?
        currentState = GameState.move;
    }
}
