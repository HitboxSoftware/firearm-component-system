using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Firearms
{
    public abstract class FirearmRound : FirearmComponent
    {
        #region VARIABLES

        public FirearmCalibre calibre;
        
        #endregion

        #region METHODS
        
        public abstract void Shoot(Firearm firearm);

        #endregion
    }

}