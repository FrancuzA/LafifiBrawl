using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace General.Managers
{
    public class Bloodmanager : MonoBehaviour
    {
        [Header("Blood Settings")]
        [SerializeField] private int maxBlood = 10;
        [SerializeField] private int currentBlood = 0;
        private Image bloodBar;
        
        public bool inGame = true;
        private AudioManager _audioManager;

        private void Start()
        {
            Dependencies.Instance.RegisterDependency<Bloodmanager>(this);
            _audioManager = Dependencies.Instance.GetDependency<AudioManager>();
            bloodBar = GetComponent<Image>();
            StartCoroutine(BloodRegen());
            bloodBar.fillAmount = 0;
        
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
        public void AddBlood(int bloodAmount)
        {
            if (currentBlood >= maxBlood) return;
            currentBlood += bloodAmount;
            if (currentBlood > maxBlood) { currentBlood = maxBlood;}
            _audioManager.PlayOneShot(_audioManager.bloodRegenRef);
            LoadBloodBar();
        }

        public void SubtractBlood(int bloodAmount)
        {
            currentBlood -= bloodAmount;
            if (currentBlood < 0) { currentBlood = 0; }
            LoadBloodBar();
        }

        private void LoadBloodBar()
        {
            bloodBar.fillAmount = (float)currentBlood / maxBlood;
        }
    }
}
