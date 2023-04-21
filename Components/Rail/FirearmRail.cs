using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Firearms;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hitbox.Firearms.RailSystem
{
    public class FirearmRail : MonoBehaviour
    {
        #region --- VARIABLES ---

        public int slotCount;
        public float slotSize;

        [SerializeField] private Dictionary<int, FirearmRailAttachment> slots = new Dictionary<int, FirearmRailAttachment>();
        
        //TEMP 
        [FormerlySerializedAs("tempComponent")] [SerializeField] private FirearmRailAttachmentProfile tempAttachment;
        [SerializeField] private int tempSlot;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            GenerateSlots();
            if (tempAttachment != null)
            {
                AttachItem(tempSlot, tempAttachment);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (slots == null || slots.Count == 0) return;
            foreach (int slot in slots.Keys)
            {
                Gizmos.color = slots[slot] == false ? Color.green : Color.red;

                Gizmos.DrawSphere(GetSlotWorldPos(slot), slotSize / 4);
            }
        }

        #endregion

        #region --- METHODS ---

        #region Generation

        public void GenerateSlots()
        {
            slots ??= new Dictionary<int, FirearmRailAttachment>();
            slots.Clear();
            for (int i = 0; i < slotCount; i++)
            {
                slots.Add(i, null);
            }
        }

        #endregion

        #region Rail Manipulation

        public bool AttachItem(int slot, FirearmRailAttachmentProfile attachment)
        {
            if (!CanAttach(slot, attachment.size)) return false;

            if (attachment.prefab == null)
            {
                Debug.LogError("Error: Unable to attach item, no prefab on attachment profile");
                return false;
            }
            
            // Create new attachment object from profile prefab
            GameObject attachmentObj = Instantiate(attachment.prefab, transform);

            // Update attachment position & rotation
            attachmentObj.transform.localPosition = GetSlotLocalPos(slot);
            attachmentObj.transform.rotation = transform.rotation;

            // Get or add RailAttachment script
            if (!attachmentObj.TryGetComponent(out FirearmRailAttachment attachmentScript))
            {
                attachmentScript = attachmentObj.AddComponent<FirearmRailAttachment>();
            }

            // Update taken slots with new attachment data.
            int[] takenSlots = new int[attachment.size];
            for (int i = slot; i < slot + attachment.size; i++)
            {
                slots[i] = attachmentScript;
                takenSlots[i - slot] = i;
            }

            // Update attachments taken slots, used when removing attachment from rail.
            attachmentScript.takenSlots = takenSlots;
            
            return true;
        }

        public FirearmRailAttachmentProfile RemoveItem(int slot)
        {
            if (!slots.ContainsKey(slot)) return null;

            // Get attachment from slot
            FirearmRailAttachment attachment = slots[slot];

            // Remove attachment references from taken slots.
            foreach (int takenSlot in attachment.takenSlots)
            {
                slots[takenSlot].profile = null;
            }

            // Get profile from attachment object
            FirearmRailAttachmentProfile attachmentProfile = attachment.profile;

            // Destroy attachment object
            Destroy(attachment.gameObject);
            
            return attachmentProfile;
        }

        #endregion

        #region Utility

        private Vector3 GetSlotWorldPos(int slot)
        {
            Transform railTransform = transform;
            return railTransform.position + (railTransform.forward * (slot * slotSize));
        }

        private Vector3 GetSlotLocalPos(int slot)
        {
            return (Vector3.forward * (slot * slotSize));
        }
        
        public bool CanAttach(int slot, int size)
        {
            for (int i = slot; i < slot + size; i++)
            {
                if (!slots.ContainsKey(i)) return false;
                if (slots[i] != null) return false;
            }

            return true;
        }

        #endregion

        #endregion
    }

}