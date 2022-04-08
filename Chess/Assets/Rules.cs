using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessRules;

public class Rules : MonoBehaviour
{
    DragAndDrop dad;
    Chess chess;

    public Rules()
    {
        dad = new DragAndDrop();
        chess = new Chess();
    }
    // Start is called before the first frame update
    void Start()
    {
        ShowFigures();
    }

    // Update is called once per frame
    void Update()
    {
        if (dad.Action())
        {
            string from = GetSquare(dad.pickPosition);
            string to = GetSquare(dad.dropPosition);
            string figure = chess.GetFigureAt((int)((dad.pickPosition.x-13) / 2.0)-1, (int)(dad.pickPosition.y/2.0)-1).ToString();
            string move = figure + from + to;
            if(chess != chess.Move(move))
            {
                if (figure == "P" && to[1] == '8' || figure == "p" && to[1] == '1')
                {
                    Figure figureTransform;
                    if (Char.IsUpper(figure, 0)) figureTransform = Figure.whiteQueen;
                    else figureTransform = Figure.blackQueen;
                    ChoosePanel();
                    Debug.Log("выбор фигуры");
                    chess = chess.PawmTransform(move, figureTransform);
                }
                else chess = chess.Move(move);
            }
            ShowFigures();

        } 
    }

    void ChoosePanel()
    {
        GameObject goChoosePanel = GameObject.Find("ChoosePanel");
        Debug.Log(dad.dropPosition.x);
        Debug.Log(dad.pickPosition.y);
        //(float)((int)(dad.pickPosition.x - 13) + 13.1
        goChoosePanel.transform.position = new Vector3(dad.dropPosition.x, dad.pickPosition.y - 1, goChoosePanel.transform.position.z);
        Debug.Log(goChoosePanel.transform.position.x);
        Debug.Log(goChoosePanel.transform.position.y);
    }

    string GetSquare(Vector2 position)
    {
        int x = Convert.ToInt32((position.x - 13) / 2.0);
        int y = Convert.ToInt32(position.y / 2.0);
        return ((char)('a' + x - 1)).ToString() + y.ToString();
    }

    void ShowFigures()
    {
        int nr =0;
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                string figure = chess.GetFigureAt(x,y).ToString();
                if (figure == ".") continue;
                PlaceFigure("box" + nr, figure, y, x);
                nr++;
            }
        for (; nr < 32; nr++) PlaceFigure("box" + nr, "q", 9, 9);
    }



    void PlaceFigure(string box, string figure, int x, int y)
    {
        GameObject goBox = GameObject.Find(box);
        GameObject goFigure = GameObject.Find(figure);
        GameObject goSquare = GameObject.Find("" + x + y);

        var spriteFigure = goFigure.GetComponent<SpriteRenderer>();
        var spriteBox = goBox.GetComponent<SpriteRenderer>();
        spriteBox.sprite = spriteFigure.sprite;

        goBox.transform.position= goSquare.transform.position;
    }
}

class DragAndDrop
{
    enum State
    {
        none,
        drag
    }

    public Vector2 pickPosition { get; private set; }
    public Vector2 dropPosition { get; private set; }

    State state;
    GameObject item;
    Vector2 offset;

    public DragAndDrop()
    {
        state = State.none;
        item = null;
    }

    public bool Action()
    {
        switch (state)
        {
            case State.none: 
                if(IsMouseButtonPressed()) PickUp();
                break;
            case State.drag:
                if (IsMouseButtonPressed()) Drag();
                else
                {
                    Drop();
                    return true;
                }

                break;
        }
        return false;
    }

    bool IsMouseButtonPressed()
    {
        return Input.GetMouseButton(0);
    }

    void PickUp()
    {
        Vector2 clickPosition = GetClickPosition();
        Transform clickedItem = GetItemAt(clickPosition);
        if(clickedItem == null) return;
        pickPosition = clickedItem.position;
        item = clickedItem.gameObject;
        state = State.drag;
        offset = pickPosition - clickPosition;
        Debug.Log("Picked up" + item.name);
    }

    Vector2 GetClickPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    Transform GetItemAt (Vector2 position)
    {
        RaycastHit2D[] figure = Physics2D.RaycastAll(position, position, 0.5f);
        if (figure.Length == 0) return null;
        return figure   [0].transform;
    }

    void Drag()
    {
        item.transform.position = GetClickPosition() + offset;
    }
    void Drop()
    {
        dropPosition = item.transform.position;
        state = State.none;
        item = null;
    }
}