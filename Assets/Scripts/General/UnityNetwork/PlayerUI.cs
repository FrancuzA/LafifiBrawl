using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Player playerScript;

    private void Awake()
    {
        playerCanvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        if (playerScript != null && healthSlider != null)
        {
            healthSlider.value = playerScript.playerHealth.Value / 100f;
        }
    }

}
