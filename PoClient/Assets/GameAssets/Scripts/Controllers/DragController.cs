using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragController : MonoBehaviour
{
    PlayerInput _playerInput;
    InputAction _touchPressAction;
    InputAction _touchPositionAction;


    GameObject _currObject = null;
    bool _isDragging;
    Vector2 _dragOffset;


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _touchPressAction = _playerInput.actions["TouchPress"];
        _touchPositionAction = _playerInput.actions["TouchPosition"];

    }

    // Update is called once per frame
    void Update()
    {
        if (_isDragging == false)
        {
            if (_touchPressAction.WasPressedThisFrame())
            {
                GameObject targetCard = CheckHitCard();
                if (targetCard != null)
                {
                    _currObject = targetCard;
                    _isDragging = true;
                    _dragOffset = (Vector2)_currObject.transform.position - GetTouchWorldPoint();
                }
            }
        }
        if (_touchPressAction.IsPressed() && _currObject != null)
        {
            DragObject();
        }

        if (_touchPressAction.WasReleasedThisFrame() && _currObject != null)
        {
            DropObject();
        }
    }

    void DragObject()
    {
        Vector2 pos = GetTouchWorldPoint() + _dragOffset;
        Vector3 curPos = new Vector3(pos.x, pos.y, -1.0f);
        _currObject.transform.position = curPos;
    }
    void DropObject()
    {
        _isDragging = false;
        _currObject = null;
    }


    GameObject CheckHitCard()
    {
        int layerMask = LayerMask.GetMask("Card");
        RaycastHit2D hit = Physics2D.GetRayIntersection(GetTouchScreenRay(), Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            return hit.transform.gameObject;
        }
        return null;
    }

    private Vector2 GetTouchWorldPoint()
    {
        Vector2 touchPos = _touchPositionAction.ReadValue<Vector2>();
        return Camera.main.ScreenToWorldPoint(touchPos);
    }

    Ray GetTouchScreenRay()
    {
        Vector2 touchPos = _touchPositionAction.ReadValue<Vector2>();
        return Camera.main.ScreenPointToRay(touchPos);
    }
}
