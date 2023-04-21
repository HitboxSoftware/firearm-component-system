using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Firearms;
using TMPro;
using UnityEngine;

public class FirearmUIAmmoCounter : MonoBehaviour
{
    #region --- VARIABLES ---

    private Firearm firearm;
    [SerializeField] private TextMeshProUGUI counter;

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Update()
    {
        UpdateCounter();
    }

    #endregion

    #region --- METHODS ---

    public void Init(Firearm firearm)
    {
        this.firearm = firearm;
    }

    public void UpdateCounter()
    {
        if (firearm.MagazineNode == null || firearm.MagazineNode.component == null)
        {
            counter.text = "No Mag!";
            return;
        }

        if (firearm.MagazineNode.component.GetType() != typeof(FirearmComponentMagazine))
        {
            counter.text = "Bad Type";
            return;
        }

        FirearmComponentMagazine magComp = (FirearmComponentMagazine)firearm.MagazineNode.component;
        FirearmComponentMagazineRuntime magRuntime = (FirearmComponentMagazineRuntime)firearm.MagazineNode.componentRuntime;

        counter.text = $"{magRuntime.magazineStack.Count}/{magComp.magazineCap}";
    }

    #endregion
}
