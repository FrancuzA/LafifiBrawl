using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace General.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public EventReference bloodRegenRef;
        public EventReference backpackSoundRef;
        public EventReference AngelikaUlt;
        public EventReference RatUlt;
        public EventReference GrzegorzUlt;
        public EventReference CleanMABelleyUlt;
        public EventReference LafifiUlt;
        public EventReference KonLongUlt;
        public EventReference Attack;
        
        private List<EventInstance> _eventInstances = new List<EventInstance>();
        private EventInstance _backpackSoundInstance;
        private EventInstance AngelikaUltInstance;
        private EventInstance RatUltInstance;
        private EventInstance GrzegorzUltInstance;
        private EventInstance CleanMABelleyUltInstance;
        private EventInstance LafifiUltInstance;
        private EventInstance KonLongUltInstance;
        private EventInstance AttackInstance;
        
        
        private void Awake()
        {
            if (Dependencies.Instance.GetDependency<AudioManager>() != null)
            {
                Destroy(gameObject);
                return;
            }
            Dependencies.Instance.RegisterDependency(this);
            DontDestroyOnLoad(gameObject);
            
            _backpackSoundInstance = CreateEventInstance(backpackSoundRef);
            AngelikaUltInstance = CreateEventInstance(AngelikaUlt);
            RatUltInstance = CreateEventInstance(RatUlt);
            GrzegorzUltInstance = CreateEventInstance(GrzegorzUlt);
            CleanMABelleyUltInstance = CreateEventInstance(CleanMABelleyUlt);
            LafifiUltInstance = CreateEventInstance(LafifiUlt);
            KonLongUltInstance = CreateEventInstance(KonLongUlt);
            AttackInstance = CreateEventInstance(Attack);

        }

        private void OnDestroy()
        {
            foreach (var eventInstance in _eventInstances)
            {
                ReleaseEventInstance(eventInstance);
            }
            Dependencies.Instance.UnregisterDependency<AudioManager>();
        }

        public void PlayOneShot(EventReference eventRef, Vector3 soundPos = default)
        {
            RuntimeManager.PlayOneShot(eventRef, soundPos);
        }

        public void PlayAngelikaUlt()
        {
            PlayEventInstance(AngelikaUltInstance);
        }

        public void PlayRatUlt()
        {
            PlayEventInstance(RatUltInstance);
        }
        public void PlayCleanMaBelleyUlt()
        {
            PlayEventInstance(CleanMABelleyUltInstance);
        }
        public void PlayKonLongUlt()
        {
            PlayEventInstance(KonLongUltInstance);
        }
        public void PlayGrzegorzUlt()
        {
            PlayEventInstance(GrzegorzUltInstance);
        }
        public void PlayLafifiUlt()
        {
            PlayEventInstance(LafifiUltInstance);
        }

        public void PlayAttackSound() 
        {
            PlayEventInstance(AttackInstance);
        }
        public void PlayBackpackSound()
        {
            PlayEventInstance(_backpackSoundInstance);
        }
        
        public void StopBackpackSound()
        {
            StopEventInstance(_backpackSoundInstance);
        }

        private EventInstance CreateEventInstance(EventReference eventRef, Vector3 soundPos = default)
        {
            var eventInstance = RuntimeManager.CreateInstance(eventRef);
            eventInstance.set3DAttributes(soundPos.To3DAttributes());
            _eventInstances.Add(eventInstance);
            return eventInstance;
        }
        
        private void PlayEventInstance(EventInstance eventInstance)
        {
            eventInstance.start();
        }
        
        private void StopEventInstance(EventInstance eventInstance)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        
        private void ReleaseEventInstance(EventInstance eventInstance)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            eventInstance.release();
        }
    }
}