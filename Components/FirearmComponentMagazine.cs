using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Hitbox.Firearms;
using Hitbox.Firearms.Integrations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "New Firearm Magazine", menuName = "Hitbox/Firearms/FCS/Components/Magazine")]
public class FirearmComponentMagazine : FirearmComponentAttachment
{
    #region --- VARIABLES ---

    public int magazineCap = 30;
    
    // TEMP
    public FirearmRound projectile;

    #endregion

    #region --- METHODS ---

    public FirearmRound UnloadRound(FirearmComponentMagazineRuntime runtime)
    {
        if (runtime.magazineStack.Count == 0) return null;

        return (FirearmRound)runtime.magazineStack.Pop();
    }

    public override FirearmComponentRuntime GetRuntime => new FirearmComponentMagazineRuntime();
    
    public override FirearmComponentRuntime GenerateRuntime(FirearmComponentNode parentNode)
    {
        FirearmComponentMagazineRuntime runtime = (FirearmComponentMagazineRuntime)base.GenerateRuntime(parentNode);

        for (int i = 0; i < magazineCap; i++)
        {
            runtime.magazineStack.Push(projectile);
        }

        return runtime;
    }

    public override FirearmComponentRuntime GenerateRuntime(FirearmComponentNode parentNode, FirearmComponentRuntime savedRuntime)
    {
        FirearmComponentRuntime runtime =  base.GenerateRuntime(parentNode, savedRuntime);

        if (runtime.GetType() != typeof(FirearmComponentMagazineRuntime)) return runtime;
        if (savedRuntime.GetType() != typeof(FirearmComponentMagazineRuntime)) return runtime;

        FirearmComponentMagazineRuntime magazineRuntime = (FirearmComponentMagazineRuntime)runtime;

        magazineRuntime.magazineStack = ((FirearmComponentMagazineRuntime)savedRuntime).magazineStack;

        return magazineRuntime;
    }

    #endregion
}

public class FirearmComponentMagazineRuntime : FirearmComponentRuntime
{
    [NonSerialized] public Stack<FirearmComponent> magazineStack = new Stack<FirearmComponent>();

    public string[] magazineData;

    #region --- SERIALIZATION CALLBACKS ---
    
    [OnSerializing]
    internal void OnSerializingMethod(StreamingContext context)
    {
        FirearmComponent[] stackArray = magazineStack.Reverse().ToArray();

        List<string> stackReferences = new List<string>();
        foreach (FirearmComponent component in stackArray)
        {
            stackReferences.Add(component.reference.AssetGUID);
        }

        magazineData = stackReferences.ToArray();    
    }
    
    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        foreach (string reference in magazineData)
        {
            Addressables.LoadAssetAsync<FirearmComponent>(reference).Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    magazineStack.Push(handle.Result);
                }
            };
        }
    }

    #endregion
    
    
}