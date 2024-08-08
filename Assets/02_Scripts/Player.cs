using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MoveType
{
    Circle,
    Square
}
public class Player : MonoBehaviour
{
    public static Player Instance;
    [SerializeField] private GameObject rectangleDraw;
    private List<Soldier> soldiers = new List<Soldier>();
    private Vector2 mousePosBeginHold;
    private Vector2 rectangleDrawPosition;
    private Vector2 leftBottomPos;
    private Vector2 rightTopPos;
    private Vector2 sizeScale;
    private RaycastHit2D[] hits;
    [SerializeField] private float spaceBetweenSoldier=1.1f;
    [SerializeField] private int countCircle=6;
    [SerializeField] private bool canRandomDirectionPos;
    [SerializeField] private float spaceRandomDirection = 0.1f;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetActiveSelectedArea(false);
        InputManager.Instance.SubEventInput(EventInputCategory.MouseDownLeft, () => SetMouseBeginHold());
        InputManager.Instance.SubEventInput(EventInputCategory.MouseHoldLeft, () => { DrawSelectArea(mousePosBeginHold, InputManager.Instance.mousePoistion); });
        InputManager.Instance.SubEventInput(EventInputCategory.MouseUpLeft, () => { SelectAllSoildierOnAreaSelected(); });
        InputManager.Instance.SubEventInput(EventInputCategory.MouseDownRight, () => { SwitchMoveType(); });
    }
    public void DrawSelectArea(Vector2 mousePosBegin, Vector2 mousePosEnd)
    {
        SetActiveSelectedArea(true);
        leftBottomPos.x = Math.Min(mousePosBegin.x, mousePosEnd.x);
        leftBottomPos.y = Math.Min(mousePosBegin.y, mousePosEnd.y);
        rightTopPos.x = Math.Max(mousePosBegin.x, mousePosEnd.x);
        rightTopPos.y = Math.Max(mousePosBegin.y, mousePosEnd.y);
        sizeScale = rightTopPos - leftBottomPos;
        rectangleDraw.transform.localScale = sizeScale;
        rectangleDrawPosition.x = leftBottomPos.x + sizeScale.x / 2;
        rectangleDrawPosition.y = leftBottomPos.y + sizeScale.y / 2;
        rectangleDraw.transform.position = rectangleDrawPosition;
    }
    private void SetMouseBeginHold()
    {
        this.mousePosBeginHold = InputManager.Instance.mousePoistion;
    }
    private Vector2 newPosSoldier;
    private Vector2 randomDirectionPos;
    private int countSoldierCurrent;
    private int index;
    private int indexCurrentCircle;
    private int radiusCircle;
    private MoveType moveType = MoveType.Circle;
    public void MoveAllSoldiersCircle()
    {
        if (soldiers.Count > 0)
        {
            soldiers[0].StartMoveToPos(InputManager.Instance.mousePoistion);
            countSoldierCurrent = 0;
            index = 1;
            while(index<soldiers.Count)
            {
                if(index>countSoldierCurrent) countSoldierCurrent += countCircle;
                radiusCircle = countSoldierCurrent / countCircle;
                for (indexCurrentCircle = 0; indexCurrentCircle < countSoldierCurrent; indexCurrentCircle++,index++)
                {
                    if (index == soldiers.Count) return;
                    newPosSoldier = Quaternion.Euler(0, 0, 360 / countSoldierCurrent *(indexCurrentCircle+1)) * Vector2.right * radiusCircle* spaceBetweenSoldier;
                    newPosSoldier = InputManager.Instance.mousePoistion + newPosSoldier;
                    if (canRandomDirectionPos) RandomDirectionPos(); else randomDirectionPos = Vector2.zero;
                    soldiers[index].StartMoveToPos(newPosSoldier+ randomDirectionPos);
                }
            }
        }      
    }
    private int width;
    private int height;
    public void MoveAllSoldiersSquare()
    {
        if (soldiers.Count > 0)
        {
            width = height = (int)Math.Ceiling(Math.Sqrt(soldiers.Count));
            index = 0;
            for (int i = 0; i < height; i++)
                for(int j = 0; j < width; j++,index++)
                {
                    if (index == soldiers.Count) return;
                    newPosSoldier.x = j* spaceBetweenSoldier;
                    newPosSoldier.y = i* spaceBetweenSoldier;
                    newPosSoldier = InputManager.Instance.mousePoistion - newPosSoldier + Vector2.one*width*0.65f/2;
                    if (canRandomDirectionPos) RandomDirectionPos(); else randomDirectionPos = Vector2.zero;
                    soldiers[index].StartMoveToPos(newPosSoldier+ randomDirectionPos);
                }
        }
    }
    private void RandomDirectionPos()
    {
        randomDirectionPos.x= Random.Range(-spaceRandomDirection, spaceRandomDirection);
        randomDirectionPos.y= Random.Range(-spaceRandomDirection, spaceRandomDirection);
    }
    public void SwitchMoveType()
    {
        if (soldiers.Count <= 0) return;
        if (moveType == MoveType.Circle)
        {
            MoveAllSoldiersCircle();
            moveType = MoveType.Square;
        }
        else
        {
            MoveAllSoldiersSquare();
            moveType = MoveType.Circle;
        }
    }
    public void RemoveAllSelectedList()
    {
        foreach (var selected in soldiers) selected.UnSelected();
        soldiers.Clear();
    }
    public void SelecSoldier(Soldier solider)
    {
        solider.Selected();
        soldiers.Add(solider);
    }
    public void SelectAllSoildierOnAreaSelected()
    {
        RemoveAllSelectedList();
        hits = Physics2D.BoxCastAll(rectangleDraw.transform.position, sizeScale, 0, Vector3.forward);
        if(hits.Length == 0&&Vector2.Distance(mousePosBeginHold,InputManager.Instance.mousePoistion)<0.005f)
        {
            hits = Physics2D.RaycastAll(InputManager.Instance.mousePoistion, Vector3.forward);
            foreach (var hit in hits)
            {
                if (hit.collider.transform.TryGetComponent<ISelected>(out ISelected selected))
                {
                    SelecSoldier(hit.collider.transform.GetComponent<Soldier>());
                    SetActiveSelectedArea(false);
                    return;
                }
            }
        }
        AddHitsISelected();
    }
    private void AddHitsISelected()
    {
        foreach (var hit in hits)
        {
            if (hit.collider.transform.TryGetComponent<ISelected>(out ISelected selected))
            {
                SelecSoldier(hit.collider.transform.GetComponent<Soldier>());
            }
        }
        SetActiveSelectedArea(false);
    }
    public void SetActiveSelectedArea(bool status)
    {
        rectangleDraw.SetActive(status);
    }
}
