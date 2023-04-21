using System.Collections;
using System.Collections.Generic;
using Hitbox.Projectiles;
using UnityEngine;


namespace Hitbox.Firearms.Integrations
{
    [CreateAssetMenu(menuName = "Hitbox/Firearms/Addons/Projectile Profile", fileName = "New Firearm Projectile Profile")]
    public class FirearmProjectileProfile : ProjectileProfile
    {
        #region --- VARIABLES ---

        [Header("Firearm Variables")] 
        public float damage;
        public int armourPierce;

        #endregion

        #region --- METHODS ---



        #endregion
    }
}