using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggablePanel : MonoBehaviour {

    [SerializeField] Scrollbar creditScrollBar;
    [SerializeField] float dragSpeed = 0.001f;

    Vector3 mouseStartDragPos;
    bool isDragging = false;

    public void OnDragBegin()
    {
        isDragging = true;
        mouseStartDragPos = Input.mousePosition;
    }

    public void OnDragEnd()
    {
        isDragging = false;
    }

    void Update()
    {
        // Made dragging based on the value of slider which is between 0 ~ 1, that is why the drag speed is very small! 
        // Dividing by initial pos Y is to make the value smaller first then continue made it smaller by dragSpeed.
        if (isDragging)
        {
            creditScrollBar.value += (Input.mousePosition.y - mouseStartDragPos.y) / mouseStartDragPos.y * -dragSpeed;
        }
    }
}
