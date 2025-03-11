using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragController : MonoBehaviour
{
    BottomPlayerController BottomPlayerCont { get; set; }

    CardComponent _currCard;

    PlayerInput _playerInput;
    InputAction _touchPressAction;
    InputAction _touchPositionAction;

    InputAction _mousePressAction;
    InputAction _mousePositionAction;

    Action _OnTouch;
    Action _OnClick;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _touchPressAction = _playerInput.actions["TouchPress"];
        _touchPositionAction = _playerInput.actions["TouchPosition"];

        _mousePressAction = _playerInput.actions["MousePress"];
        _mousePositionAction = _playerInput.actions["MousePosition"];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BottomPlayerCont = FindFirstObjectByType<BottomPlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (_touchPressAction.WasPerformedThisFrame())
        {
            Debug.Log("Touch action was pressed");
            GameObject targetCard = TouchHitCard();
            if (targetCard != null)
            {
                CardComponent cardComp = targetCard.GetComponent<CardComponent>();
                OnCardClick(cardComp);
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Mouse was pressed");
        }
        if (Mouse.current.leftButton.isPressed)
        {
            Debug.Log("Mouse pressed");
        }
        //Vector3 mpos = Mouse.current.position.ReadValue();
        //Debug.Log(mpos);

        if (_mousePressAction.WasPerformedThisFrame())
        {
            Debug.Log("Mouse action was pressed");
            GameObject targetCard = ClickHitCard();
            if (targetCard != null)
            {
                CardComponent cardComp = targetCard.GetComponent<CardComponent>();
                OnCardClick(cardComp);
            }
        }
    }

    void OnCardClick(CardComponent cardComp)
    {
    }

    GameObject TouchHitCard()
    {
        int layerMask = LayerMask.GetMask("Card");
        RaycastHit2D hit = Physics2D.GetRayIntersection(GetTouchScreenRay(), Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            return hit.transform.gameObject;
        }
        return null;
    }

    Ray GetTouchScreenRay()
    {
        Vector2 touchPos = _touchPositionAction.ReadValue<Vector2>();
        return Camera.main.ScreenPointToRay(touchPos);
        //return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    GameObject ClickHitCard()
    {
        int layerMask = LayerMask.GetMask("Card");
        RaycastHit2D hit = Physics2D.GetRayIntersection(GetClickScreenRay(), Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            return hit.transform.gameObject;
        }
        return null;
    }

    Ray GetClickScreenRay()
    {
        Vector2 mousePos = _mousePositionAction.ReadValue<Vector2>();
        return Camera.main.ScreenPointToRay(mousePos);
        //return Camera.main.ScreenPointToRay(Input.mousePosition);
    }



}
