using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bloodmanager : MonoBehaviour
{
    private float maxBlood = 10f;
    public float currentBlood = 0f;
    private Image bloodBar;
    public bool inGame = true;
    public EventReference BloodRegenRef;
    private EventInstance BloodRegenSound;

    private void Start()
    {
        Dependencies.Instance.RegisterDependency<Bloodmanager>(this);
        bloodBar = GetComponent<Image>();
        StartCoroutine(StandardBloodRegen());
        bloodBar.fillAmount = 0;
        
    }

    IEnumerator StandardBloodRegen()
    {
        while (inGame)
        {
            yield return new WaitForSecondsRealtime(3f);
            AddBlood(1f);
        }
    }
    public void AddBlood(float bloodAmount)
    {
        if (currentBlood >= maxBlood) return;

        BloodRegenSound = RuntimeManager.CreateInstance(BloodRegenRef);
        currentBlood += bloodAmount;
        if (currentBlood > maxBlood) { currentBlood = maxBlood; }
        BloodRegenSound.start();
        BloodRegenSound.release();
        LoadBloodBar();
    }

    public void SubstractBlood(float bloodAmount)
    {
        currentBlood -= bloodAmount;
        if (currentBlood < 0) { currentBlood = 0; }
        LoadBloodBar();
    }

    public void LoadBloodBar()
    {
        bloodBar.fillAmount = currentBlood/10;
    }
}
