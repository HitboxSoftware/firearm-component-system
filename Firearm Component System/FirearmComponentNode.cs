using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hitbox.Firearms;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FirearmComponentNode : MonoBehaviour
{
    #region --- VARIABLES ---

    // Component installed on node.
    public FirearmComponent component;
    public FirearmComponentRuntime componentRuntime;
    
    // Attachment must have these tags to be attached.
    public FirearmComponentTag requiredTag;

    // If true, this slot must be filled to perform an action on the parent.
    public bool essential;

    // If true, don't show this slot in UI.
    public bool isHidden;

    #endregion

    #region --- METHODS ---

    public bool Install(FirearmComponent newComponent)
    {
        Clear();
        
        // If node profile doesn't have required tag.
        if (newComponent.tag != requiredTag) return false;

        component = newComponent;
        componentRuntime = newComponent.GenerateRuntime(this);

        return true;
    }

    public bool InstallOnChildNode(FirearmComponentNode node, FirearmComponent newComponent)
    {
        if (!componentRuntime.nodes.Contains(node)) return false;

        if (node.Install(newComponent)) return false;
        return true;
    }

    public void Clear()
    {
        if (componentRuntime != null)
        {
            foreach (FirearmComponentNode runtimeNode in componentRuntime.nodes)
            {
                Destroy(runtimeNode.gameObject);
            }
            
            if(component != null) component.CleanUp(componentRuntime);
            
            componentRuntime = null;
        }
        
        component = null;
    }

    public IEnumerable<FirearmComponentNode> GetRequiredNodes()
    {
        List<FirearmComponentNode> requiredNodes = new ();

        if (component != null && componentRuntime != null)
        {
            foreach (FirearmComponentNode componentNode in componentRuntime.nodes)
            {
                requiredNodes.AddRange(componentNode.GetRequiredNodes());
            }
        } 
        else if (essential)
        {
            requiredNodes.Add(this);
        }
        
        return requiredNodes.ToArray();
    }

    #region SERIALISATION

    public IEnumerator LoadFromData(FirearmComponentData data)
    {
        AsyncOperationHandle<FirearmComponent> handle =
            Addressables.LoadAssetAsync<FirearmComponent>(data.componentReference);

        yield return handle.WaitForCompletion();

        if (handle.Status != AsyncOperationStatus.Succeeded) yield break;
        
        component = handle.Result;
        componentRuntime = component.GenerateRuntime(this, data.runtimeData);

        List<FirearmComponentNode> childNodes = new List<FirearmComponentNode>();
        foreach (FirearmComponentNodeProfile nodeProfile in component.componentNodes)
        {
            GameObject childNodeObj = new GameObject(nodeProfile.name)
            {
                transform =
                {
                    parent = transform,
                    localPosition = nodeProfile.nodePosition,
                    rotation = transform.rotation
                }
            };
                    
            FirearmComponentNode childNode = childNodeObj.AddComponent<FirearmComponentNode>();
            childNode.essential = nodeProfile.essential;
            childNode.isHidden = nodeProfile.isHidden;
            childNode.requiredTag = nodeProfile.requiredTag;

            childNode.component = nodeProfile.childComponent;

            childNodes.Add(childNode);
        }
                
        foreach (FirearmComponentData componentData in data.childReferences)
        {
            FirearmComponentNode node = childNodes[componentData.index];
            
            yield return node.LoadFromData(componentData);
        }

        componentRuntime.nodes = childNodes.ToArray();
    }

    #endregion

    #endregion
}