using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Firearms.Modules
{
    [RequireComponent(typeof(Firearm))]
    public abstract class FirearmModule : MonoBehaviour
    {
        #region --- VARIABLES ---

        private protected Firearm Firearm;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Awake()
        {
            Firearm = GetComponent<Firearm>();
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

        protected virtual void SubscribeEvents()
        {
            
        }

        protected virtual void UnsubscribeEvents()
        {
            
        }

        #endregion
    }

}