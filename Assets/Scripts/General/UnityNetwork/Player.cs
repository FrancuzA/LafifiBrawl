using System;
using System.Collections;
using System.Collections.Generic;
using General;
using General.Managers;
using General.UnityNetwork;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    //Belly = 0
    //Grzegorz = 1
    //Kon = 2
    //Lafifi = 3
    //nAngelika = 4
    //Rat = 5
    
    [SerializeField] private GameObject bloodManager;
    [SerializeField] private Image playerImage;
    [SerializeField] private GameObject[] spawnButtons;
    [Tooltip("Belly = 0\nGrzegorz = 1\nKon = 2\nLafifi = 3\nAngelika = 4\nRat = 5")]
    [SerializeField] private UnitsStats[] unitsStats;
    private NetworkSpawner _networkSpawner;
    private AudioManager _audioManager;
    [SerializeField] private bool inGame = true;
    [SerializeField] private RectTransform rectTransform;
    
    private List<UnitsStats> equippedUnits;
    
    [Header("Stats")]
    
    [SerializeField] private Slider healthBarFill;
    [SerializeField] private Slider bloodBarFill;
    
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int maxBlood = 10;
    public NetworkVariable<float> currentHealth { get; } = new (100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentBlood { get; } = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _audioManager = Dependencies.Instance.GetDependency<AudioManager>();
        currentHealth.OnValueChanged += OnHealthChanged;
        currentBlood.OnValueChanged += OnBloodChanged;
        bloodBarFill.value = 0;
        healthBarFill.value = 0;
        
        if (IsOwner)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            currentHealth.Value = maxHealth;
            currentBlood.Value = 0;
            playerImage.color = Color.white;
            bloodManager.SetActive(true);
            spawnButtons[0].SetActive(true);
            spawnButtons[1].SetActive(true);
            StartCoroutine(BloodRegen());
        }
        else
        {
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            bloodManager.SetActive(false);
            spawnButtons[0].SetActive(false);
            spawnButtons[1].SetActive(false);
            //playerImage.color = Color.black;
        }
        
        UpdateBlood();
        UpdateHealth();
    }

    private void OnBloodChanged(float previousValue, float newValue)
    {
        UpdateBlood();
    }

    private void UpdateBlood()
    {
        bloodBarFill.value = currentBlood.Value / maxBlood;
    }

    private void OnHealthChanged(float previousValue, float newValue)
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        healthBarFill.value = currentHealth.Value / maxHealth;
    }

    IEnumerator BloodRegen(float waitTime = 3f)
    {
        var wait = new WaitForSeconds(waitTime);
        while (inGame)
        {
            yield return wait;
            AddBlood(1);
        }
    }

    public void TakeDamage(float damage)
    {
        if(!IsOwner) return;
        if (currentHealth.Value <= damage) { currentHealth.Value = 0; }
        else { currentHealth.Value -= damage; }
        if(currentHealth.Value <= 0)
        {
            Application.Quit();
        }
    }
    
    public void AddHealth(float healthToAdd)
    {
        if(!IsOwner) return;
        currentHealth.Value += healthToAdd;
        if (currentHealth.Value > maxHealth)
        {
            currentHealth.Value = maxHealth;
            return;
        }
    }
    
    public void AddBlood(float bloodToAdd)
    {
        if(!IsOwner) return;
        currentBlood.Value += bloodToAdd;
        if (currentBlood.Value > maxBlood) { 
            currentBlood.Value = maxBlood;
            return;
        }
        _audioManager.PlayOneShot(_audioManager.bloodRegenRef);
    }
    
    [ClientRpc]
    public void RemoveBloodClientRpc(float bloodToRemove)
    {
        if(!IsOwner) return;
        currentBlood.Value -= bloodToRemove;
        if (currentBlood.Value < 0) { currentBlood.Value = 0; }
    }

    #region Spawning

    public void SpawnUnitBelly()
    {
        RequestSpawnUnit(0);
    }
    
    public void SpawnUnitGrzegorz()
    {
        RequestSpawnUnit(1);
    }
    
    public void SpawnUnitKon()
    {
        RequestSpawnUnit(2);
    }
    
    public void SpawnUnitLafifi()
    {
        RequestSpawnUnit(3);
    }
    
    public void SpawnUnitAngelika()
    {
        RequestSpawnUnit(4);
    }
    
    public void SpawnUnitRat()
    {
        RequestSpawnUnit(5);
    }

    private void RequestSpawnUnit(ushort unitStatIndex, ServerRpcParams serverRpcParams = default)
    {
        if(currentBlood.Value < unitsStats[unitStatIndex].UltCost) return;
        NetworkSpawner.Singleton.SpawnUnitsForPlayerServerRpc(NetworkManager.Singleton.LocalClientId,
                unitStatIndex, this);
    }

    #endregion

}
