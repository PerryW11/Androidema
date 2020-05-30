using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLights : MonoBehaviour
{
    Renderer rend;
    private bool isChanging = false;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if(!isChanging)
        {
            CallChangeColor();
        }
    }

    private void CallChangeColor()
    {
        StartCoroutine(ChangeColor());
    }

    IEnumerator ChangeColor()
    {
        if(!isChanging)
        {
            isChanging = true;
            rend.material.SetColor("_Color", Random.ColorHSV());
            yield return new WaitForSeconds(2f);
            isChanging = false;
        }   
    }
}
