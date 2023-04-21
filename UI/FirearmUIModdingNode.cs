using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirearmUIModdingNode : MonoBehaviour, IPointerClickHandler
{
    #region --- VARIABLES ---

    public FirearmComponentNode node;
    
    [SerializeField] private Sprite emptyNode;
    [SerializeField] private Sprite filledNode;

    [Header("Components")]
    [SerializeField] private Image uiImage; 
    private bool active;
    
    // Events
    public static event Action<FirearmUIModdingNode> MouseClick;

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Update()
    {
        if (active && uiImage.sprite != filledNode)
        {
            uiImage.sprite = filledNode;
        } else if (!active && uiImage.sprite != emptyNode)
        {
            uiImage.sprite = emptyNode;
        }
    }

    #endregion

    #region --- METHODS ---

    public void UpdateNode(bool enable) => active = enable;

    #endregion
    
    #region --- UI EVENTS ---

    public void OnPointerClick(PointerEventData eventData)
    {
        MouseClick.Invoke(this);
    }

    #endregion
}
