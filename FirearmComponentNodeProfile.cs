using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Component Node", menuName = "Hitbox/Firearms/FCS/Node Profile")]
public class FirearmComponentNodeProfile : ScriptableObject
{
    #region --- VARIABLES ---

    // Relative to Parent.
    public Vector3 nodePosition;

    public FirearmComponent childComponent;
    
    // --- Node Data ---
    // Attachment must have these tags to be attached.
    public FirearmComponentTag requiredTag;

    // If true, this slot must be filled to perform an action on the parent.
    public bool essential;

    // If true, don't show this slot in UI.
    public bool isHidden;

    #endregion
    
    #region --- METHODS ---

    

    #endregion
}
