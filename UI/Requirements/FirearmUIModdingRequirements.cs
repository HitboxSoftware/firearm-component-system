using System.Collections;
using System.Collections.Generic;
using Hitbox.Firearms;
using TMPro;
using UnityEngine;

public class FirearmUIModdingRequirements : MonoBehaviour
{
    #region --- VARIABLES ---

    [Header("Components")] 
    public TextMeshProUGUI content;

    #endregion

    #region --- METHODS ---

    public void UpdateRequirements(Firearm firearm)
    {
        if (firearm.requiredUnfilledSlots == null ||
            firearm.requiredUnfilledSlots.Count == 0)
        {
            if (firearm.ChamberNode != null && firearm.ChamberNode.component == null)
            {
                content.text = "Firearm has essential parts but no bullet is in the chamber!";
                content.color = Color.yellow;
            }
            else
            {
                content.text = "Firearm is Ready!";
                content.color = Color.green;
            }
            return;
        }

        string requirementsList = "";
        foreach (FirearmComponentNode node in firearm.requiredUnfilledSlots)
        {
            requirementsList += $"{node.name} is essential & has no attached node!\n";
            content.color = Color.red;
        }

        content.text = requirementsList;
    }

    #endregion
}
