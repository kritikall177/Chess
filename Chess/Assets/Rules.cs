using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ChessRules;

public class Rules : MonoBehaviour
{
    DragAndDrop dad;

    public Rules()
    {
        dad = new DragAndDrop();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dad.Action(); 
    }
}

class DragAndDrop
{
    enum State
    {
        none,
        drag
    }

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
        item = clickedItem.gameObject;
        state = State.drag;
        offset = (Vector2)clickedItem.position - clickPosition;
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
        return figure[0].transform;
    }

    void Drag()
    {
        item.transform.position = GetClickPosition() + offset;
    }
    void Drop()
    {
        state = State.none;
        item = null;
    }
}