using UnityEngine;

namespace Hitbox.Firearms.RailSystem
{
    [CreateAssetMenu(fileName = "New Rail Attachment Profile", menuName="Hitbox/Firearms/FCS/Components/Rail/Attachment Profile")]
    public class FirearmRailAttachmentProfile : ScriptableObject
    {
        #region --- VARIABLES ---

        public int size;
        public GameObject prefab;

        #endregion
    }

}