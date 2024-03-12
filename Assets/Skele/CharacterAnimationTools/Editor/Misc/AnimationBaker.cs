using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExtMethods;

namespace MH.Skele.CAT
{
    /// <summary>
    /// given a clip, bake the clip frame by frame
    /// </summary>
    public class AnimationBaker : EditorWindow
    {
        // configurable data
        private List<Transform> m_Roots = new List<Transform>(); //when click "Collect", will collect all transform under each root;

        // data
        private List<Transform> m_Trs = new List<Transform>(); //the target transforms, will undo.record them for each frame;
        private Transform[] m_TrsArr;
        private bool m_baking = false;
        private object m_uaw = null; //the unity animation window

        [MenuItem("Window/Skele/AnimationBaker")]
        public static void OpenWindow()
        {
            var wnd = EditorWindow.GetWindow(typeof(AnimationBaker));
            EUtil.SetEditorWindowTitle(wnd, "AnimationBaker");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Set the Roots:");
            for(int i=0; i<m_Roots.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    var root = (Transform)EditorGUILayout.ObjectField(m_Roots[i], typeof(Transform), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_Roots[i] = root;

                    }
                    if(EUtil.Button("-", "delete entry", Color.red, GUILayout.Width(20f)))
                    {
                        m_Roots.RemoveAt(i);
                        GUIUtility.ExitGUI();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(40f);
                if (GUILayout.Button("Add Root Entry"))
                {
                    m_Roots.Add(null);
                }
                GUILayout.Space(40f);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            if (GUILayout.Button(m_baking ? "Baking" : "Not Baking", EditorStyles.toolbarButton))
            {
                m_baking = ! m_baking;

                if (m_baking)
                {
                    m_Trs.Clear();
                    foreach (var oneRoot in m_Roots)
                    {
                        for (var ie = oneRoot.GetRecurEnumerator(); ie.MoveNext(); )
                        {
                            m_Trs.Add(ie.Current);
                        }
                    }

                    m_TrsArr = m_Trs.ToArray();
                    m_uaw = EUtil.GetUnityAnimationWindow();
                    if (null == m_uaw)
                    {
                        m_baking = false;
                    }
                    
                }
            }

        }

        void Update()
        {
            if (!m_baking || m_TrsArr == null)
                return;

            Undo.RecordObjects(m_TrsArr, "baking");
        }
    }
}
