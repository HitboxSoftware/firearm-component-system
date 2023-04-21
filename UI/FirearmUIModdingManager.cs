using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Hitbox.Firearms;
using UnityEngine;

public class FirearmUIModdingManager : MonoBehaviour
{
    #region --- VARIABLES ---

    public Camera targetCamera;
    public Firearm targetFirearm;

    [Header("Components")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private FirearmUIModdingCatalog catalog;
    [SerializeField] private FirearmUIModdingRequirements requirements;
    [SerializeField] private FirearmUIAmmoCounter ammoCounter;

    // Data
    private Dictionary<FirearmComponentNode, FirearmUIModdingNode> nodeObjects = new ();

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Start()
    {
        GenerateUIComponents();
    }

    private void Update()
    {
        UpdateUIComponents();
    }
    
    private void OnEnable()
    {
        OnTargetFirearmModified();

        FirearmUIModdingNode.MouseClick += FirearmUIModdingNodeOnMouseClick;
        targetFirearm.Modified += OnTargetFirearmModified;
    }
    
    private void OnDisable()
    {
        ClearUIComponents();
        FirearmUIModdingNode.MouseClick -= FirearmUIModdingNodeOnMouseClick;
        targetFirearm.Modified -= OnTargetFirearmModified;
    }

    #endregion

    #region --- METHODS ---

    public void GenerateUIComponents()
    {
        ClearUIComponents();
        
        if(ammoCounter != null)
            ammoCounter.Init(targetFirearm);
        
        if (targetFirearm == null) return;

        foreach (FirearmComponentNode node in targetFirearm.nodes)
        {
            if (node == null) return;
            
            GenerateLinkObject(node);
        }
    }

    public void ClearUIComponents()
    {
        if (nodeObjects == null) return;
        
        foreach (FirearmUIModdingNode node in nodeObjects.Values)
        {
            Destroy(node.gameObject);
        }

        nodeObjects.Clear();
    }

    public void UpdateUIComponents()
    {
        foreach (FirearmComponentNode link in nodeObjects.Keys)
        {
            if (link == null)
            {
                ClearUIComponents();
                GenerateUIComponents();
                break;
            }
            
            nodeObjects[link].transform.position = targetCamera.WorldToScreenPoint(link.transform.position);
            nodeObjects[link].UpdateNode(link.component != null);
        }
    }

    private void GenerateLinkObject(FirearmComponentNode node)
    {
        if (node.isHidden)
        {
            if (node.componentRuntime is not { nodes: { } }) return;
            foreach (FirearmComponentNode nodeChild in node.componentRuntime.nodes)
            {
                GenerateLinkObject(nodeChild);
            }

            return;
        }
        
        GameObject nodeObj = Instantiate(nodePrefab, transform);
        Transform nodeTransform = nodeObj.GetComponent<Transform>();
        nodeTransform.position = targetCamera.WorldToScreenPoint(node.transform.position);

         if(!nodeObj.TryGetComponent(out FirearmUIModdingNode modNode))
         {
             modNode = nodeObj.AddComponent<FirearmUIModdingNode>();
         }

         modNode.node = node;
        
        nodeObjects.Add(node, modNode);
        
        if (node.component != null)
        {
            modNode.UpdateNode(true);
            
            // If ComponentRuntime && ComponentRuntime.nodes is null
            if (node.componentRuntime is not { nodes: { } }) return;
            
            foreach (FirearmComponentNode nodeLink in node.componentRuntime.nodes)
            {
                if (nodeLink == null) continue;
                    
                GenerateLinkObject(nodeLink);
            }
        }
    }

    public void AttachFirearmComponent(FirearmComponentNode node, FirearmComponent component)
    {
        if (node.requiredTag != component.tag) return;

        if (node.component == component)
        {
            node.Clear();
            targetFirearm.BuildRequirements();
        }
        else
        {
            targetFirearm.AttachComponent(node, component);
        }
    }

    #region EVENT METHODS

    private void FirearmUIModdingNodeOnMouseClick(FirearmUIModdingNode obj)
    {
        if (catalog == null) return;
        catalog.PopulateCatalog(obj.node);
    }
    
    private void OnTargetFirearmModified()
    {
        GenerateUIComponents();
        if(requirements != null)
            requirements.UpdateRequirements(targetFirearm);
    }


    #endregion

    #endregion
}
