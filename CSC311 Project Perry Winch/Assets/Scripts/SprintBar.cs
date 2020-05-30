using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprintBar : MonoBehaviour
{
    private PlayerBehavior player;
    private Image sprintBar;

    private void Start()
    {
        sprintBar = transform.Find("IMG_Stamina").GetComponent<Image>();
        player = FindObjectOfType<PlayerBehavior>();
    }
    private void Update()
    {
        sprintBar.fillAmount = player.stamina / 100f;
    }

}
