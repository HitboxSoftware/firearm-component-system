using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Firearms;
using Hitbox.WeaponSystem;
using UnityEngine;

namespace Hitbox.WeaponSystem
{
    public class FirearmInputManager : MonoBehaviour
    {
        #region --- VARIABLES ---

        private InputManager inputManager;
        private Firearm firearm;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            inputManager = InputManager.Instance;
            firearm = GetComponent<Firearm>();
        }

        private void Update()
        {
            // Firearm Input
            firearm.isShooting = inputManager.PlayerInputActions.Gameplay.Shoot.IsPressed();
        }

        #endregion
    }

}