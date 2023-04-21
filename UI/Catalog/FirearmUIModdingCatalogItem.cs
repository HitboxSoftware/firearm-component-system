using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirearmUIModdingCatalogItem : MonoBehaviour, IPointerClickHandler
{
    #region --- VARIABLES ---

    // Events
    public static event Action<FirearmUIModdingCatalogItem> MouseClick;

    public string title;
    
    //Objects
    [SerializeField] private TextMeshProUGUI uiText;

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Start()
    {
        uiText.text = title;
    }

    #endregion

    #region --- METHODS ---

    

    #endregion
    
    #region --- UI EVENTS ---

    public void OnPointerClick(PointerEventData eventData)
    {
        MouseClick.Invoke(this);
    }

    #endregion
}
