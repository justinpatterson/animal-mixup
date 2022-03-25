using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    private Board board;
    private GameObject otherDot;
    private Vector2 FirstTouchPosition;
    private Vector2 FinalTouchPosition;
    private Vector2 tempPostion;
    public float swipeAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //move towards the target
            tempPostion = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPostion, .4f);
        }
        else
        {
            //Directly set the postion
            tempPostion = new Vector2(targetX, transform.position.y);
            transform.position = tempPostion;
            board.allDots[column, row] = this.gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //move towards the target
            tempPostion = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPostion, .4f);
        }
        else
        {
            //Directly set the postion
            tempPostion = new Vector2(transform.position.x, targetY);
            transform.position = tempPostion;
            board.allDots[column, row] = this.gameObject;
        }
    }

        private void OnMouseDown()
    {
            FirstTouchPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            //Debug.Log(FirstTouchPosition);

    }

    private void OnMouseUp()
    {
        FinalTouchPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        CalculateAngle();
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(FinalTouchPosition.y - FirstTouchPosition.y, FinalTouchPosition.x - FirstTouchPosition.x) * 180 / Mathf.PI;

        //Debug.Log(swipeAngle);
        MovePieces();
    }

    void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width)
        {
            //Right Swipe
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>(). column -=1;
            column += 1;

        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            //Up Swipe
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;

        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;

        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;

        }
    }

}

