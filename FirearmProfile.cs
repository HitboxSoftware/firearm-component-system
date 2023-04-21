using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hitbox.Firearms
{
    [CreateAssetMenu(fileName = "New Firearm Profile", menuName = "Hitbox/Firearms/Profile")]
    public class FirearmProfile : ScriptableObject
    {
        #region --- VARIABLES ---

        // Firearm Statistics
        public GameObject baseFirearmObject;
        
        #region - Details -
        
        public float fireRate = 0.1f;
        public int burstAmount;
        public FireMode fireMode;

        #endregion
    
        #region - Recoil -

        //Default Weapon Recoil
        public float recoilX = -1;
        public float recoilY = 1;
        public float recoilZ = 0.35f;

        public float recoilPosMinus = 0.03f;

        //ADS Recoil
        public float aimRecoilX = -0.25f;
        public float aimRecoilY = 0.25f;
        public float aimRecoilZ = 0.15f;

        public float aimRecoilPosMinus = 0.01f;

        //Settings
        public float snappiness = 6;
        public float returnSpeed = 2;
        public float cameraRecoilMultiplier = 4;

        #endregion

        #region - Positions -

        public Vector3 defaultPos;
        public Vector3 aimPos;
        public Vector3 highReadyPos;
        public Vector3 lowReadyPos;

        #endregion

        #region - Smoothing -

        public float amount = 0.005f;
        public float smoothAmount = 6f;
        public float maxAmount = 0.06f;

        public float rotationAmount = 0.005f;

        #endregion

        #endregion
    }

    public enum FireMode
    {
        Automatic,
        SemiAutomatic,
        Burst,
        Single
    }

}