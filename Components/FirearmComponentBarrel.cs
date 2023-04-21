using System;
using System.Linq;
using Hitbox.Firearms;
using UnityEngine;

[CreateAssetMenu(fileName = "New Component Barrel", menuName = "Hitbox/Firearms/FCS/Components/Barrel")]
public class FirearmComponentBarrel : FirearmComponent
{
    #region --- VARIABLES ---

    public Vector3 bulletOrigin;
    
    #endregion

    #region --- METHODS ---

    public FirearmComponentNode GetBarrelEnd(FirearmComponentNode startNode, FirearmComponentTag[] searchTags)
    {
        if (!searchTags.Contains(startNode.requiredTag)) return startNode;
        
        FirearmComponentNode currentNode = Firearm.GetChildNodeWithTags(startNode, searchTags);
        if (currentNode == null) return startNode;
        if (currentNode.component == null) return startNode;
        if (currentNode.component.GetType() != typeof(FirearmComponentBarrel)) return startNode;
        
        currentNode = GetBarrelEnd(currentNode, searchTags);

        return currentNode;
    }

    #endregion
}