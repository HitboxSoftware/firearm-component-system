using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Component Tag", menuName = "Hitbox/Firearms/FCS/Tag")]
public class FirearmComponentTag : ScriptableObject
{
    #region --- VARIABLES ---

    public FirearmComponent[] compatibleComponents;

    #endregion
}
