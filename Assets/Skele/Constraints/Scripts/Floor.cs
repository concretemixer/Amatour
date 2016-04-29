using System;
using System.Collections.Generic;
using UnityEngine;

namespace MH.Constraints
{
    /// <summary>
    /// limit the owner to stay at the specified side of a plane
    /// </summary>
    public class Floor : BaseConstraint
    {
        #region "configurable data"

        [SerializeField][Tooltip("the target transform")]
        private Transform m_target;
        [SerializeField][Tooltip("lock the position on plane to eliminate sliding when constrained")]
        private bool m_sticky = false;
        [SerializeField][Tooltip("not only use target's position, but also the target's rotation")]
        private bool m_useRotation = false;
        [SerializeField][Tooltip("the limit space")]
        private EAxisD m_ePlaneDir = EAxisD.Y;
        [SerializeField][Tooltip("use offset?")]
        private bool m_useOffset = false;
        [SerializeField][Tooltip("offset value, only effect when m_useOffset is true")]
        private float m_offset = 0;
        [SerializeField][Tooltip("the weight of constraints")]
        private float m_influence = 1f;

        #endregion "configurable data"

        #region "data"

        private bool m_sticked = false;
        private Vector3 m_stickyPt = new Vector3(float.NaN, float.NaN, float.NaN);

        #endregion "data"

        #region "unity event handlers"

        #endregion "unity event handlers"

        #region "props"
        public UnityEngine.Transform Target
        {
            get { return m_target; }
            set { m_target = value; }
        }
        public bool Sticky
        {
            get { return m_sticky; }
            set { m_sticky = value; }
        }
        public bool IsSticked
        {
            get { return m_sticked; }
        }
        public bool UseRotation
        {
            get { return m_useRotation; }
            set { m_useRotation = value; }
        }
        public EAxisD PlaneDir
        {
            get { return m_ePlaneDir; }
            set { m_ePlaneDir = value; }
        }
        public bool UseOffset
        {
            get { return m_useOffset; }
            set { m_useOffset = value; }
        }
        public float Offset
        {
            get { return m_offset; }
            set { m_offset = value; }
        }
        public override float Influence
        {
            get { return m_influence; }
            set { m_influence = value; }
        }

        #endregion "props"

        #region "public method"
        // public method

        public override void DoAwake()
        {
            base.DoAwake();
        }

        public override void DoUpdate()
        {
            base.DoUpdate();

            if (!m_target)
                return; //do nothing if no target is specified

            Vector3 initPos = m_tr.position;
            Vector3 targetPos = m_target.position;
            Vector3 endPos = initPos;

            Vector3 normal = _GetNormal();

            // fix the targetPos
            Vector3 planePt = targetPos;
            if( m_useOffset )
                planePt += normal * m_offset;

            // apply effect & offset
            if( _IsAtBackSideOfPlane(initPos, normal, planePt) ) 
            { //and do projection if initPos is at the back side of the plane

                Vector3 projectedPt = Vector3.ProjectOnPlane(initPos - planePt, normal) + planePt;

                if (m_sticky)
                {
                    if (m_sticked)
                    {
                        projectedPt = m_stickyPt;
                    }
                    else
                    {
                        m_sticked = true;
                        m_stickyPt = projectedPt;
                    }
                }

                endPos = projectedPt;

                // apply influence
                if (!Mathf.Approximately(m_influence, 1f))
                {
                    endPos = Misc.Lerp(initPos, endPos, m_influence);
                }
            }
            else
            {
                m_sticked = false;
            }

            
            m_tr.position = endPos;
        }

        /// <summary>
        /// whether initPos is at the back-side of the plane specified by [normal, planePt]
        /// </summary>
        private bool _IsAtBackSideOfPlane(Vector3 initPos, Vector3 normal, Vector3 planePt)
        {
            float D = -Vector3.Dot(normal, planePt);
            float v = Vector3.Dot(normal, initPos) + D;
            return v < 0;
        }

        /// <summary>
        /// calculate the normal vector of the plane,
        /// account for the useRotation, ePlaneDir
        /// </summary>
        private Vector3 _GetNormal()
        {
            Vector3 n = Vector3.up;
            switch (m_ePlaneDir)
            {
                case EAxisD.X: n = Vector3.right; break;
                case EAxisD.Y: n = Vector3.up; break;
                case EAxisD.Z: n = Vector3.forward; break;
                case EAxisD.InvX: n = Vector3.left; break;
                case EAxisD.InvY: n = Vector3.down; break;
                case EAxisD.InvZ: n = Vector3.back; break;
                default: Dbg.LogErr("Floor._GetNormal: unexpected EAxisD: {0}", m_ePlaneDir); break;
            }

            if (m_useRotation)
                n = m_target.rotation * n;

            return n.normalized;
        }

        public override void DoDrawGizmos()
        {
            base.DoDrawGizmos();

            if (m_target)
            {
                _DrawLine(m_tr, m_target);
            }
        }

        #endregion "public method"

        #region "private method"
        // private method

        #endregion "private method"

        #region "constant data"
        // constant data



        #endregion "constant data"
    }
}
