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
        
        private List<EventInstance> _eventInstances = new List<EventInstance>();
        private EventInstance _backpackSoundInstance;
        
        
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