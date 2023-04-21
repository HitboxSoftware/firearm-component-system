using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Firearms.Integrations
{
    [RequireComponent(typeof(Firearm))]
    public class FirearmFMODAudio : MonoBehaviour
    {
        #region --- VARIABLES ---

        // Components
        private Firearm firearm;

        // Audio Events
        private AudioEvent gunshot;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion
        
        #region --- METHODS ---

        private void Init()
        {
            firearm = GetComponent<Firearm>();
            
            GetAudioEvents();
        }

        private void GetAudioEvents()
        {
            // Shoot Event
            if (firearm.BarrelNode == null || firearm.BarrelNode.component == null) return;
            foreach (FirearmComponentAdditionalData aD in firearm.BarrelNode.component.additionalData)
            {
                if(aD.GetType() != typeof(FirearmComponentFMODData)) continue;

                gunshot = ((FirearmComponentFMODData)aD).audioEvent;
            }
        }

        #region Event Subscriptions

        public void SubscribeEvents()
        {
            firearm.Modified += GetAudioEvents;
            firearm.Shoot += PlayGunshot;
        }

        public void UnsubscribeEvents()
        {
            firearm.Modified -= GetAudioEvents;
            firearm.Shoot -= PlayGunshot;
        }

        #endregion

        public void PlayGunshot()
        {
            try { gunshot.Play(firearm.transform.position); }
            catch (NullReferenceException)
            {
                Debug.LogError("Error: Unable to play sound [Gunshot], no AudioEvent assigned. ", this);
            }
        }

        #endregion
    }

}