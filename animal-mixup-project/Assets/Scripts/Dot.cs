using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private Board board;
    private GameObject otherDot;
    private Vector2 FirstTouchPosition;
    private Vector2 FinalTouchPosition;
    private Vector2 tempPostion;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, .2f);
            Debug.Log("Should turn gray...");
        }
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //move towards the target
            tempPostion = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPostion, .6f);
        }
            if(board.allDots[column, row] != this.gameObject)
        {
            board.allDots[column, row] = this.gameObject;
        }
        else
        {
            //Directly set the postion
            tempPostion = new Vector2(targetX, transform.position.y);
            transform.position = tempPostion;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //move towards the target
            tempPostion = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPostion, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            //Directly set the postion
            tempPostion = new Vector2(transform.position.x, targetY);
            transform.position = tempPostion;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        Debug.Log("About to Check Move Coroutine");
        if(otherDot != null)
        {
            Debug.Log("Found other dot...");
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                Debug.Log("Neither dot is matched yet...");
                //otherDot.GetComponent<Dot>().previousRow = otherDot.GetComponent<Dot>().row;
                //otherDot.GetComponent<Dot>().previousColumn = otherDot.GetComponent<Dot>().column;

                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;

                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
                Debug.Log("Resuming game, no match found...");
            }
            else
            {
                Debug.Log("Will try to destroy and remove dots from board");
                board.DestroyMatches();
                
            }
            otherDot = null;
        }
        
    }

        private void OnMouseDown()
    {
        Debug.Log("dot was clicked");
        if (board.currentState == GameState.move)
        {
            
            FirstTouchPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
           
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("dot was UN-clocked");
        if (board.currentState == GameState.move)
        {
            
            FinalTouchPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        float absYSwipe = Mathf.Abs(FinalTouchPosition.y - FirstTouchPosition.y);
        float absXSwipe = Mathf.Abs(FinalTouchPosition.x - FirstTouchPosition.x);
        if (absYSwipe > swipeResist || absXSwipe > swipeResist)
        {
            swipeAngle = Mathf.Atan2(FinalTouchPosition.y - FirstTouchPosition.y, FinalTouchPosition.x - FirstTouchPosition.x) * 180 / Mathf.PI;
            Debug.Log("Swipe angle is " + swipeAngle.ToString());
            MovePieces();
            board.currentState = GameState.wait;
        }
        else
        {
            Debug.Log("Neither " + absYSwipe + " NOR " + absXSwipe + " exceeded " + swipeResist);
            board.currentState = GameState.move;
        }
        
        
    }

    void MovePieces()
    {

        previousRow = row;
        previousColumn = column;
        Debug.Log("BEFORE SWIPE, I AM " + row + ", " + column);
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width -1)
        {
            //Right Swipe
            otherDot = board.allDots[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>(). column -=1;
            column += 1;

        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height -1)
        {
            //Up Swipe
            otherDot = board.allDots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;

        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe
            otherDot = board.allDots[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;

        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            otherDot = board.allDots[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;

        }
        Debug.Log("AFTER SWIPE, I AM " + row + ", " + column);
        StartCoroutine(CheckMoveCo());
    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

}

