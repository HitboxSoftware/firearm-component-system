using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hitbox.Firearms
{
    public class Firearm : MonoBehaviour
    {
        #region --- VARIABLES ---
        
        // --- FIREARM DATA ---
        // Firearm Profile
        public FirearmProfile profile;
        
        // - Search Tags -
        // Tags used to find different core components, and determine their behaviour.
        public FirearmComponentTag chamberTag;
        public FirearmComponentTag magazineTag;
        // Whilst not necessary, Components with these tags should only really contain Barrel Components.
        public FirearmComponentTag[] barrelTags = Array.Empty<FirearmComponentTag>();

        // - Object Data -
        // Top-Level Firearm Nodes
        public FirearmComponentNode[] nodes = Array.Empty<FirearmComponentNode>();
        // Firearm can only shoot if this list is empty, calculated by BuildRequirements function.
        public List<FirearmComponentNode> requiredUnfilledSlots = new ();

        // --- CORE COMPONENTS ---
        public FirearmComponentNode ChamberNode { get; private set; }
        public FirearmComponentNode MagazineNode { get; private set; }
        public FirearmComponentNode BarrelNode { get; private set; }

        // --- EVENTS ---
        public event Action Shoot;
        public event Action Modified;
        
        // --- ACTIONS ---
        public bool isShooting;
        public bool isLocked;
        public bool triggerHeld;
        public bool boltLatched;
        
        private float _shootTimer;
        private bool CanFire => requiredUnfilledSlots.Count == 0;


        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            Build();
            
            FirearmData newData = GetFirearmData();
            string jsonData = JsonConvert.SerializeObject(newData, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            Debug.Log(jsonData);
            FirearmData loadedData = JsonConvert.DeserializeObject<FirearmData>(jsonData, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            
            Clear();

            StartCoroutine(LoadFirearm(loadedData));
        }
        
        private void Update()
        {
            if (profile == null)
            {
                Debug.LogError("Error: Firearm in Scene has no profile!");
                return;
            }
            
            switch (isShooting)
            {
                case false when triggerHeld: // Resetting Held Trigger
                    triggerHeld = false;
                    break;
                case true when profile.fireMode == FireMode.Automatic && Time.time > _shootTimer: // Automatic Fire
                    Fire();
                    break;
                case true when !triggerHeld && Time.time > _shootTimer: // Semi-Automatic Fire
                    Fire();
                    triggerHeld = true;
                    break;
            }
        }

        #endregion

        #region --- METHODS ---

        public bool AttachComponent(FirearmComponentNode target, FirearmComponent component)
        {
            if (target == null || component == null)
            {
                Debug.LogError("Error: Unable to Attach Component, Target or Component is null");
                return false;
            }

            target.Install(component);
            BuildRequirements();
            Modified?.Invoke();
            return true;
        }

        public virtual void Build()
        {
            // Building Nodes & Components
            foreach (FirearmComponentNode firearmComponentNode in nodes)
            {
                if (firearmComponentNode.component != null)
                {
                    firearmComponentNode.componentRuntime = firearmComponentNode.component.GenerateRuntime(firearmComponentNode);
                }
            }
            
            BuildRequirements();
            Modified?.Invoke();
        }

        public virtual void Clear()
        {
            MagazineNode = null;
            BarrelNode = null;
            ChamberNode = null;
            
            foreach (FirearmComponentNode node in nodes)
            {
                node.Clear();
            }
        }

        public virtual void BuildRequirements()
        {
            // Resetting Core Nodes
            ChamberNode = null;
            MagazineNode = null;
            BarrelNode = null;
            
            // Getting Core Nodes
            foreach (FirearmComponentNode node in nodes)
            {
                // Getting Chamber Node
                FirearmComponentNode chamberNode = GetNodeWithTag(node, chamberTag);
                if (chamberNode != null)
                {
                    ChamberNode = chamberNode;
                }
                
                // Getting Magazine Node
                FirearmComponentNode magazineNode = GetNodeWithTag(node, magazineTag);
                if (magazineNode != null)
                {
                    MagazineNode = magazineNode;
                }
                
                // Getting Barrel Node
                if (BarrelNode == null)
                {
                    FirearmComponentNode barrelNode = GetNodeWithTags(node, barrelTags);
                    if (barrelNode is { component: FirearmComponentBarrel barrel })
                    {
                        BarrelNode = barrel.GetBarrelEnd(barrelNode, barrelTags);
                    }
                }

            }

            // Calculating Required Slots
            requiredUnfilledSlots.Clear();
            foreach (FirearmComponentNode node in nodes)
            {
                if(node == null) continue;
                requiredUnfilledSlots.AddRange(node.GetRequiredNodes());
            }
            
            // !IMPORTANT! TEMPORARY!
            Modified?.Invoke();   
        }

        public virtual bool Fire()
        {
            if (isLocked) return false;
            if (!CanFire) return false;
            if (ChamberNode == null) return false;
            if (ChamberNode.component is not FirearmRound round)
            {
                if (boltLatched && _shootTimer + 0.1f < Time.time)
                {
                    // TODO: Dry Fire Logic
                    boltLatched = false;
                }
                return false;
            }

            if (BarrelNode == null) return false;
            
            _shootTimer = Time.time + 60f / profile.fireRate;
            boltLatched = true;

            round.Shoot(this);

            Shoot?.Invoke();

            // If firearm is not (semi-)automatic, stop now.
            if (profile.fireMode is not (FireMode.Automatic or FireMode.SemiAutomatic)) return true;
            
            // Clear Chamber, Only if Automatic (Bolt-Action needs manual action before releasing spent cartridge.)
            ChamberNode.Clear();

            ChamberRound(); 

            return true;
        }

        // Chamber from attached Magazine
        public virtual bool ChamberRound()
        {
            if (ChamberNode == null) return false;
            if (MagazineNode == null || MagazineNode.component == null) return false;

            if (MagazineNode.component.GetType() != typeof(FirearmComponentMagazine) ||
                MagazineNode.componentRuntime.GetType() != typeof(FirearmComponentMagazineRuntime))
            {
                Debug.LogError("Error: Unable to chamber new round from magazine, magazine is not of type FirearmComponentMagazine");
                return false;
            }

            // New Round from Magazine
            FirearmRound newRound = ((FirearmComponentMagazine)MagazineNode.component).UnloadRound(
                (FirearmComponentMagazineRuntime)MagazineNode.componentRuntime);
            
            if (newRound != null)
            {
                ChamberNode.component = newRound;
            }

            return true;
        }
        
        // Chamber given round.
        public virtual bool ChamberRound(FirearmRound round)
        {
            if (ChamberNode == null) return false;
            
            if (round != null)
            {
                ChamberNode.component = round;
            }

            return true;
        }


        #region Utility

        public static FirearmComponentNode GetNodeWithTag(FirearmComponentNode startNode, FirearmComponentTag searchTag)
        {
            if (startNode.requiredTag == searchTag) return startNode;
            if (startNode.component == null || startNode.componentRuntime == null) return null;

            foreach (FirearmComponentNode node in startNode.componentRuntime.nodes)
            {
                if (node.requiredTag == searchTag) return node;
                FirearmComponentNode foundNode = GetNodeWithTag(node, searchTag);

                if (foundNode != null) return foundNode;
            }

            return null;
        }
        
        public static FirearmComponentNode GetNodeWithTags(FirearmComponentNode startNode, FirearmComponentTag[] searchTags)
        {
            if (searchTags.Contains(startNode.requiredTag)) return startNode;
            if (startNode.component == null || startNode.componentRuntime == null) return null;

            foreach (FirearmComponentNode node in startNode.componentRuntime.nodes)
            {
                if (searchTags.Contains(node.requiredTag)) return node;
                FirearmComponentNode foundNode = GetNodeWithTags(node, searchTags);

                if (foundNode != null) return foundNode;
            }

            return null;
        }
        
        public static FirearmComponentNode GetChildNodeWithTags(FirearmComponentNode parentNode, FirearmComponentTag[] searchTags)
        {
            if (parentNode == null) return null;
            if (parentNode.component == null || parentNode.componentRuntime == null) return null;
            
            foreach (FirearmComponentNode node in parentNode.componentRuntime.nodes)
            {
                if (node == null) continue;

                if (searchTags.Contains(node.requiredTag)) return node;
            }

            return null;
        }

        #endregion
        
        #region Serialisation

        public FirearmData GetFirearmData()
        {
            if (nodes == null)
            {
                Debug.LogError("Error: Unable to save data, Firearm is not fully initialized");
                return new FirearmData(Array.Empty<FirearmComponentData>());
            }
            
            List<FirearmComponentData> nodeData = new List<FirearmComponentData>();
            for (int i = 0; i < nodes.Length; i++)
            {
                FirearmComponentNode node = nodes[i];
                
                if (node != null && node.component != null)
                {
                    nodeData.Add(FirearmComponentData.DataFromComponent(node.component, node.componentRuntime, i));
                }
            }

            FirearmData newData = new FirearmData(nodeData.ToArray());
            return newData;
        }

        public IEnumerator LoadFirearm(FirearmData data)
        {
            foreach (FirearmComponentData dataNode in data.nodes)
            {
                yield return StartCoroutine(nodes[dataNode.index].LoadFromData(dataNode));
            }
            
            BuildRequirements();
            Modified?.Invoke();
        }

        #endregion

        #endregion
        
        #region --- EVENTS ---
        
        public virtual void OnShoot()
        {
            Shoot?.Invoke();
        }

        public virtual void OnModified()
        {
            Modified?.Invoke();
        }

        #endregion
    }

    // READONLY COULD CAUSE ISSUES WITH DESERIALIZING (ONLY WHEN NESTED)
    public struct FirearmData
    {
        public FirearmComponentData[] nodes;

        public FirearmData(FirearmComponentData[] nodes)
        {
            this.nodes = nodes;
        }
    }

}