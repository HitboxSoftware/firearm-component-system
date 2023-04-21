using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Firearms.Modules
{
    public class FirearmVFX : FirearmModule
    {
        #region --- VARIABLES ---

        [SerializeField] private ParticleSystem caseEjection;

        private GameObject _muzzleFlashPrefab;

        #endregion

        #region --- METHODS ---

        private void UpdateVFX()
        {
            if (Firearm == null) return;
            
            // Get Muzzle Flash
            foreach (FirearmComponentAdditionalData additionalData in Firearm.BarrelNode.component.additionalData)
            {
                if (additionalData.GetType() != typeof(FirearmComponentVFXData)) continue;

                _muzzleFlashPrefab = ((FirearmComponentVFXData)additionalData).prefab;
            }
        }

        private void PlayShootVFX()
        {
            // Muzzle Flash
            GameObject muzzleFlashObj = Instantiate(_muzzleFlashPrefab);
            Transform muzzleFlashT = muzzleFlashObj.transform;
            muzzleFlashT.parent = Firearm.BarrelNode.transform;
            muzzleFlashT.forward = muzzleFlashT.parent.forward;
            muzzleFlashT.localPosition = ((FirearmComponentBarrel)Firearm.BarrelNode.component).bulletOrigin;
            
            Destroy(muzzleFlashObj, 5);
            
            // Casing Ejection
            if (caseEjection != null)
            {
                caseEjection.Emit(1);
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            
            Firearm.Modified += UpdateVFX;

            Firearm.Shoot += PlayShootVFX;
        }
        
        protected override void UnsubscribeEvents()
        {
            base.SubscribeEvents();
            
            Firearm.Modified -= UpdateVFX;

            Firearm.Shoot -= PlayShootVFX;
        }

        #endregion
    }

}