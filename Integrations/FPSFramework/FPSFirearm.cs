using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// - FPS FRAMEWORK -
using Kinemation.FPSFramework.Runtime.Core;
using FPSFireMode = Kinemation.FPSFramework.Runtime.Core.FireMode;

namespace Hitbox.Firearms.Integrations.FPSFramework
{
    public class FPSFirearm : Firearm
    {
        #region --- VARIABLES ---

        // --- FPS FRAMEWORK DATA ---
        public FPSFireMode fpsFireMode; // Translated from Hitbox FireMode upon firearm generation.
        [SerializeField] public WeaponAnimData gunData;
        [SerializeField] public RecoilAnimData recoilData;
        // ------

        
        // Runtime Scope Data
        private List<Transform> _scopes;
        private int _scopeIndex;
        
        // --- EVENTS ---
        public event Action EndShoot;
        
        #endregion

        #region --- METHODS ---

        public Transform GetScope()
        {
            _scopeIndex++;
            _scopeIndex = _scopeIndex > _scopes.Count - 1 ? 0 : _scopeIndex;
            return _scopes[_scopeIndex];
        }

        #region Overrides

        public override bool Fire()
        {
            bool fired = base.Fire();

            if (!fired)
            {
                EndShoot?.Invoke();
            }

            return fired;
        }

        public override void Build()
        {
            // Building Nodes & Components
            foreach (FirearmComponentNode firearmComponentNode in nodes)
            {
                if (firearmComponentNode.component != null)
                {
                    firearmComponentNode.componentRuntime = firearmComponentNode.component.GenerateRuntime(firearmComponentNode);
                }
                
                // TODO: Get if Component is Scope and add to Scope list.
            }
            
            BuildRequirements();
            OnModified();
        }

        #endregion
        
        #region --- EVENTS ---
        
        public virtual void OnEndShoot()
        {
            EndShoot?.Invoke();
        }
        
        #endregion

        #endregion
    }

}