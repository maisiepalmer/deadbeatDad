using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public GameObject target;

    void Start()
    {
        target = GameObject.FindWithTag("ExitDoor");
        SetVisible(false);
    }

    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform);
        }
    }

    public void SetTarget(string newTarget)
    {
        target = GameObject.Find(newTarget);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
