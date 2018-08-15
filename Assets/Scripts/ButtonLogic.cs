using UnityEngine;
using System.Collections;
using System;

public class ButtonLogic : MonoBehaviour
{
    public Action onEnterFunction;
    public Action onExitFunction;
    public Action onClickFunction;

    private void OnMouseEnter()
    {
        if (onEnterFunction != null)
            onEnterFunction();
    }

    private void OnMouseExit()
    {
        if (onExitFunction != null)
            onExitFunction();
    }

    private void OnMouseDown()
    {
        if (onClickFunction != null)
            onClickFunction();
    }
}
