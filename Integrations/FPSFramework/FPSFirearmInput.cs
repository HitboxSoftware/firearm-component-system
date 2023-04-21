using System.Collections;
using System.Collections.Generic;
using Hitbox.Firearms.Integrations.FPSFramework;
using Hitbox.WeaponSystem;
using UnityEngine;

public class FPSFirearmInput : MonoBehaviour
{
    #region --- VARIABLES ---

    private PlayerInputActions _playerInputActions;
    private FPSFirearm firearm;

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Start()
    {
        _playerInputActions = InputManager.Instance.PlayerInputActions;
        firearm = GetComponent<FPSFirearm>();
    }

    private void Update()
    {
        // Firearm Input
        firearm.isShooting = _playerInputActions.Gameplay.Shoot.IsPressed();

        if (_playerInputActions.Gameplay.Shoot.WasReleasedThisFrame())
        {
            firearm.OnEndShoot();
        }
    }

    #endregion
}
