using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Firearms.Integrations.Projectiles
{
    public class FirearmTrajectoryVisualizer : MonoBehaviour
    {
        #region --- VARIABLES ---

        public bool updateTrajectory;

        private Vector3[] _trajectoryPoints;
        private readonly List<Vector3> _collisionPoints = new List<Vector3>();

        private Firearm _firearm;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            _firearm = GetComponent<Firearm>();
        }

        private void Update()
        {
            _trajectoryPoints = null;
            _collisionPoints.Clear();
            if (!updateTrajectory) return;
            if (_firearm.ChamberNode == null) return;
            if (_firearm.ChamberNode.component is not FirearmProjectile round)
            {
                return;
            }
            if (_firearm.BarrelNode == null) return;

            Vector3 localPos = _firearm.BarrelNode.transform.localPosition + ((FirearmComponentBarrel)_firearm.BarrelNode.component).bulletOrigin;
            Vector3 origin = _firearm.BarrelNode.transform.TransformPoint(localPos);
            _trajectoryPoints = round.projectileProfile.SimulateTrajectory(origin, _firearm.BarrelNode.transform.forward);

            _collisionPoints.Clear();
            if (_trajectoryPoints.Length > 2)
            {
                for (int i = 0; i < _trajectoryPoints.Length - 1; i++)
                {
                    if (Physics.Linecast(_trajectoryPoints[i], _trajectoryPoints[i + 1], out RaycastHit hit))
                        _collisionPoints.Add(hit.point);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            if (_trajectoryPoints is { Length: > 0 })
            {
                for (int i = 1; i < _trajectoryPoints.Length; i++)
                {
                    Gizmos.DrawLine(_trajectoryPoints[i - 1], _trajectoryPoints[i]);
                }
            }

            foreach (Vector3 collisionPoint in _collisionPoints)
            {
                Gizmos.DrawSphere(collisionPoint, 0.15f);
            }
        }

        #endregion

        #region --- METHODS ---



        #endregion
    }

}