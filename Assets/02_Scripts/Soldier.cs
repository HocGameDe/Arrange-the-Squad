using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ISelected
{
    public void Selected();
    public void UnSelected();
}
public class Soldier : MonoBehaviour,ISelected
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject selectedBackground;
    private void Start()
    {
        selectedBackground.SetActive(false);
    }
    private void MoveToPos(Vector3 newPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, newPos, moveSpeed*Time.deltaTime);
    }
    private IEnumerator IMove(Vector3 newPos)
    {
        while(transform.position != newPos)
        {
            MoveToPos(newPos);
            yield return null;
        }
    }
    public void StartMoveToPos(Vector3 newPos)
    {
        StopAllCoroutines();
        StartCoroutine(IMove(newPos));
    }

    public void Selected()
    {
        selectedBackground.SetActive(true);
    }
    public void UnSelected()
    {
        selectedBackground.SetActive(false);
    }
}

