using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EventInputCategory
{
    MouseDownLeft,
    MouseDownRight,
    MouseUpLeft,
    MouseUpRight,
    MouseHoldLeft,
    MouseHoldRight
}
public class EventAction
{
    public event Action eventAction;
    public void RunAction()
    {
        eventAction?.Invoke();
    }
}
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public bool doNotInteractWithUI;
    public Vector2 mousePoistion;
    private new Camera camera;
    private Dictionary<EventInputCategory, EventAction> eventInputDic = new Dictionary<EventInputCategory, EventAction>();
    private bool holdingMouseLeft;
    private bool holdingMouseRight;
    private void Awake()
    {
        camera = Camera.main;
        AddDictionaryEvent();
        Instance = this;    
    }
    private void AddDictionaryEvent()
    {
        eventInputDic.Add(EventInputCategory.MouseDownLeft, new EventAction());
        eventInputDic.Add(EventInputCategory.MouseDownRight, new EventAction());
        eventInputDic.Add(EventInputCategory.MouseUpLeft, new EventAction());
        eventInputDic.Add(EventInputCategory.MouseUpRight, new EventAction());
        eventInputDic.Add(EventInputCategory.MouseHoldLeft, new EventAction());
        eventInputDic.Add(EventInputCategory.MouseHoldRight, new EventAction());
    }
    private Vector3 ConvertScreenToWorldPoint(Vector3 position)
    {
        return camera.ScreenToWorldPoint(position);
    }
    public void SubEventInput(EventInputCategory eventInput, Action action)
    {
        eventInputDic[eventInput].eventAction += action;
    }
    public void RemoveEventInput(EventInputCategory eventInput, Action action)
    {
        eventInputDic[eventInput].eventAction -= action;
    }
    private void Update()
    {
        if(doNotInteractWithUI) if (EventSystem.current.IsPointerOverGameObject()) return;
        mousePoistion = ConvertScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            eventInputDic[EventInputCategory.MouseDownLeft].RunAction();
            holdingMouseLeft = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
            eventInputDic[EventInputCategory.MouseDownRight].RunAction();
            holdingMouseRight = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            eventInputDic[EventInputCategory.MouseUpLeft].RunAction();
            holdingMouseLeft = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            eventInputDic[EventInputCategory.MouseUpRight].RunAction();
            holdingMouseRight = false;
        }      
    }
    private void LateUpdate()
    {
        if (holdingMouseLeft) eventInputDic[EventInputCategory.MouseHoldLeft].RunAction();
        if (holdingMouseRight) eventInputDic[EventInputCategory.MouseHoldRight].RunAction();
    }
}



