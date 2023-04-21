using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FirearmUIModdingCatalog : MonoBehaviour
{
    #region --- VARIABLES ---

    [Header("Components")] 
    [SerializeField] private FirearmUIModdingManager manager;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject catalogItemPrefab;
    
    // --- RUNTIME ---
    private FirearmComponentNode targetNode;
    
    private Dictionary<FirearmUIModdingCatalogItem, FirearmComponent> catalogItems =
        new Dictionary<FirearmUIModdingCatalogItem, FirearmComponent>();

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void OnEnable()
    {
        FirearmUIModdingCatalogItem.MouseClick += FirearmUIModdingCatalogItemOnMouseClick;
    }
    
    private void OnDisable()
    {
        FirearmUIModdingCatalogItem.MouseClick -= FirearmUIModdingCatalogItemOnMouseClick;
    }

    #endregion

    #region --- METHODS ---

    public void PopulateCatalog(FirearmComponentNode target)
    {
        ClearCatalog();

        if (catalogItemPrefab == null)
        {
            Debug.LogError("Error: Unable to populate Firearm Modding Catalog, No Item Prefab assigned!");
            return;
        }
        targetNode = target;

        title.text = $"{targetNode.name} Attachments";

        if (target.requiredTag == null) return;
        foreach (FirearmComponent compatibleComponent in target.requiredTag.compatibleComponents)
        {
            GameObject catalogItemObj = Instantiate(catalogItemPrefab, transform);

            if (!catalogItemObj.TryGetComponent(out FirearmUIModdingCatalogItem catalogItem))
            {
                catalogItem = catalogItemObj.AddComponent<FirearmUIModdingCatalogItem>();
            }

            catalogItem.title = compatibleComponent.name;
            
            catalogItems.Add(catalogItem, compatibleComponent);
        }
    }

    public void ClearCatalog()
    {
        title.text = "Slot: None";

        foreach (FirearmUIModdingCatalogItem catalogItem in catalogItems.Keys)
        {
            Destroy(catalogItem.gameObject);
        }
        
        catalogItems.Clear();
    }

    #region EVENT METHODS

    private void FirearmUIModdingCatalogItemOnMouseClick(FirearmUIModdingCatalogItem catalogItem)
    {
        if (manager == null)
        {
            Debug.LogError("Error: Unable to attach FirearmComponent, no UIModdingManager attached.");
        }
        
        manager.AttachFirearmComponent(targetNode, catalogItems[catalogItem]);
    }

    #endregion

    #endregion
}
