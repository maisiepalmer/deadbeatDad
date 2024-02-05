using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public GameObject target;
    private string targetName = "";

    void Start()
    {
        SetVisible(false);
    }

    void Update()
    {
        if (target == null && targetName != "")
            target = GameObject.Find(targetName);

        if (target != null)
            transform.LookAt(target.transform);
    }

    public void SetTarget(string newTarget)
    {
        targetName = newTarget;
        target = GameObject.Find(newTarget);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
