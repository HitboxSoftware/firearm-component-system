using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hitbox.Firearms.Integrations
{
    [CreateAssetMenu(fileName = "New Firearm Cartridge", menuName="Hitbox/Firearms/FCS/Attachments/Cartridges/Projectile")]
    public class FirearmProjectile : FirearmRound
    {
        #region --- VARIABLES ---
        
        public FirearmProjectileProfile projectileProfile;

        #endregion

        #region --- METHODS ---

        public override void Shoot(Firearm firearm)
        {
            if (firearm.BarrelNode == null) return;
            if (projectileProfile == null || projectileProfile.projectilePrefab == null) return;

            GameObject projectile = Instantiate(projectileProfile.projectilePrefab);
            Transform projectileTransform = projectile.transform;
            
            Vector3 localPos = firearm.BarrelNode.transform.localPosition + ((FirearmComponentBarrel)firearm.BarrelNode.component).bulletOrigin;
            Transform barrelTransform;
            Vector3 origin = (barrelTransform = firearm.BarrelNode.transform).TransformPoint(localPos);

            projectileTransform.position = origin;
            projectileTransform.forward = barrelTransform.forward;
        }

        #endregion
    }

}