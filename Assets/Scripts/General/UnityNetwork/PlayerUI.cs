using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace General.UnityNetwork
{
    public class PlayerUI : MonoBehaviour
    {
        //[SerializeField] private Canvas playerCanvas;
        [SerializeField] private Image healthBar;
        [SerializeField] private Player playerScript;

        /*private void Awake()
    {
        playerCanvas.worldCamera = Camera.main;
    }*/

        private void Update()
        {
            if (playerScript && healthBar)
            {
                healthBar.fillAmount = playerScript.playerHealth.Value / 100f;
            }
        }
    }
}
