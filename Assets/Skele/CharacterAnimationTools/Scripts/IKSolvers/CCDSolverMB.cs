using System;
using System.Collections.Generic;
using UnityEngine;
using ExtMethods;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MH.Constraints
{
    /// <summary>
    /// the container for CCDSolver,
    /// 
    /// this MB should be put on the endJoint gameObject, so it can handle different settings under a single tree-structure
    /// </summary>
    public class CCDSolverMB : BaseSolverMB
    {
		#region "configurable data"
	    // configurable data

        [SerializeField][Tooltip("the target object")]
        private Transform m_target;

        [SerializeField][Tooltip("the bone count")]
        private int m_boneCount = 2;
        [SerializeField][Tooltip("when the endJoint and target are within this distance, the calc is taken as success")]
        private float m_distThreshold = 0.00001f;
        [SerializeField][Tooltip("damp limits the rotate delta in one iteration")]
        private bool m_setUseDamp = true;
        [SerializeField][Tooltip("global damp limit for joints under this solver")]
        private float m_setGlobalDamp = 10f;
        [SerializeField][Tooltip("if target is specified, then apply the rotation of target on endJoint ")]
        private bool m_useTargetRotation = false;

        [SerializeField][Tooltip("how to recover if the IK cannot reach reasonable solution to given target")]
        private CCDSolver.RevertOption m_revertOpt = CCDSolver.DEF_RevertOpt;


        [SerializeField][Tooltip("the weight of constraints")]
        private float m_influence = 1f;

        //////////////////////////////////////////////////////////
        //need to prepare for revert when deactivate constraint, record data like
        [SerializeField][Tooltip("TrInitInfo for ANCESTOR bones on the chain, exclude this tr")]
        private List<TrInitInfo> m_initInfos = new List<TrInitInfo>();

	
	    #endregion "configurable data"
	
		#region "data"
	    // data

        private CCDSolver m_solver;
	
	    #endregion "data"
	
		#region "unity event handlers"

        public override void DoAwake()
        {
            base.DoAwake();
            m_cstack.ExecOrder = Mathf.Max(m_cstack.ExecOrder, 100);
            GetSolver();
        }

        public override void DoUpdate()
        {
            base.DoUpdate();

            for (int i = 0; i < m_initInfos.Count; ++i)
                m_initInfos[i].UpdateInitInfo();
            for (int i = 0; i < m_initInfos.Count; ++i)
                m_initInfos[i].RevertToInitInfo();

            CCDSolver solver = GetSolver();

            if (m_target && m_influence != 0)
            {
                //Vector3 initPos = m_tr.position; 
                Vector3 targetPos = m_target.position;

                //if (solver.Target != targetPos)
                //    MUndo.RecordObjects(solver.GetJoints(), "CCDSolverMB.DoUpdate");

                solver.Target = targetPos;
                solver.Execute(m_influence);

                if( m_useTargetRotation )
                    m_tr.rotation = m_target.rotation;
            }

            for (int i = 0; i < m_initInfos.Count; ++i)
                m_initInfos[i].RecordLastLocInfo();

            //TODO?: find ConstraintStack on parent bones, and call RecordLastLocInfo on each one
        }

        void OnTransformParentChanged()
        {
            int realLevel = 0;
            if (!m_tr.HasParentLevel(m_boneCount, out realLevel))
                boneCount = realLevel;
            GetSolver(true); //force update solver;
        }
	
	    #endregion "unity event handlers"
	
		#region "public method"

        /// <summary>
        /// access bone count,
        /// when change, if decrease, revert those taken out bones; if increase, add new TrInitInfo and call ResetInitInfo
        /// </summary>
        public int boneCount
        {
            get { return m_boneCount; }
            set {
                int actualLevel = value;
                if (m_boneCount != value && Tr.HasParentLevel(value, out actualLevel))
                { //ensure there're enough parents up ahead
                    if (m_boneCount > value)
                    {  //decrease
                        for (int i = m_initInfos.Count - 1; i >= 0; --i)
                        {
                            m_initInfos[i].RevertToInitInfo();
                        }
                        m_initInfos.Resize(value);
                    }
                    else
                    { //increase
                        Transform lastTr = m_boneCount > 0 ? m_initInfos[m_boneCount - 1].tr : Tr; 
                        for (int i = m_initInfos.Count; i < value; ++i)
                        {
                            lastTr = lastTr.parent;
                            TrInitInfo newInitInfo = new TrInitInfo(lastTr);
                            newInitInfo.ResetInitInfo();
                            m_initInfos.Add(newInitInfo);
                        }
                    }

                    m_boneCount = value;
                    _InitSolver();
                }
            }
        }

        public Transform Tr
        {
            get
            {
                if (m_tr == null)
                    m_tr = transform;
                return m_tr;
            }
        }

        public override IKSolverType solverType
        {
            get { return IKSolverType.CCD; }
        }

        public UnityEngine.Transform Target
        {
            get {
                return m_target; 
            }
            set { 
                m_target = value; 
            }
        }

        public float distThres
        {
            get
            {
                if (m_solver != null)
                {
                    m_distThreshold = m_solver.distThres;
                }
                return m_distThreshold;
            }
            set
            {
                m_distThreshold = value;
                if (m_solver != null)
                {
                    m_solver.distThres = m_distThreshold;
                }
            }
        }

        public bool useDamp
        {
            get
            {
                if (m_solver != null)
                {
                    m_setUseDamp = m_solver.useDamp;
                }
                return m_setUseDamp;
            }
            set
            {
                m_setUseDamp = value;
                if (m_solver != null)
                {
                    m_solver.useDamp = m_setUseDamp;
                }
            }
        }

        public float globalDamp
        {
            get
            {
                if (m_solver != null)
                {
                    m_setGlobalDamp = m_solver.globalDamp;
                }
                return m_setGlobalDamp;
            }
            set
            {
                m_setGlobalDamp = value;
                if (m_solver != null)
                {
                    m_solver.globalDamp = m_setGlobalDamp;
                }
            }
        }

        public bool useTargetRotation
        {
            get { return m_useTargetRotation; }
            set { m_useTargetRotation = value; }
        }

        public CCDSolver.RevertOption revertOpt
        {
            get
            {
                if (m_solver != null)
                {
                    m_revertOpt = m_solver.revertOpt;
                }
                return m_revertOpt;
            }
            set
            {
                m_revertOpt = value;
                if (m_solver != null)
                {
                    m_solver.revertOpt = m_revertOpt;
                }
            }
        }
        public List<TrInitInfo> InitInfos
        {
            get { return m_initInfos; }
        }
        public override float Influence
        {
            get { return m_influence; }
            set { m_influence = value; }
        }


        public CCDSolver GetSolver(bool force = false)
        {
            if (force || m_solver == null || m_solver.Count != m_boneCount )
            {
                m_solver = null; //clear first

                if (m_boneCount > 0)
                {
                    m_solver = new CCDSolver();
                    _InitSolver();
                    _InitInitInfos();
                }                
            }

            return m_solver;
        }

        public override void OnConstraintActiveChanged()
        {
            base.OnConstraintActiveChanged();
            _RevertAllAncestorsInitInfos();
        }


	    #endregion "public method"
	
		#region "private method"

        private void _InitSolver()
        {
            if (m_solver != null)
            {
                int actualLevel = 0;
                if (!Tr.HasParentLevel(m_boneCount, out actualLevel))
                {
                    m_boneCount = actualLevel;
                }

                m_solver.Target = Tr.position;
                m_solver.useDamp = m_setUseDamp;
                m_solver.globalDamp = m_setGlobalDamp;
                m_solver.SetBones(Tr, m_boneCount);
            }            
        }

        private void _InitInitInfos()
        {
            m_initInfos.Clear();
            Transform lastTr = Tr;
            for (int i = 0; i < m_boneCount; ++i)
            {
                lastTr = lastTr.parent;
                TrInitInfo newInitInfo = new TrInitInfo(lastTr);
                newInitInfo.ResetInitInfo();
                m_initInfos.Add(newInitInfo);
            }
        }

        private void _RevertAllAncestorsInitInfos()
        {
            for (int i = 0; i < m_initInfos.Count; ++i)
            {
                m_initInfos[i].RevertToInitInfo();
            }
        }

	    #endregion "private method"
	
		#region "constant data"
	    // constant data
	
	    #endregion "constant data"

    }
}
