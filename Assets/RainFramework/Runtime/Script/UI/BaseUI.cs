using Rain.UI;
using UnityEngine;
using Tools = Rain.Core.Tools;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Rain.UI
{
    public class BaseUI : MonoBehaviour
    {
        RectTransform m_rect;
        public RectTransform rect
        {
            get
            {
                if (m_rect == null)
                    m_rect = GetComponent<RectTransform>();
                return m_rect;
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected T GetComponentInChildren<T>(string _childName, Transform _trans = null)
        {
            Transform trans = FindChildNode(_trans != null ? _trans : transform, _childName);
            if (trans == null)
            {
                return default(T);
            }
            return trans.GetComponent<T>();
        }

        /// <summary>
        /// 获取子节点对象
        /// </summary>
        /// <param name="_childName"></param>
        /// <returns></returns>
        public GameObject GetChildNode(string _childName, Transform _tf = null)
        {
            Transform tr = FindChildNode(_tf != null ? _tf : transform, _childName);
            if (tr)
            {
                return tr.gameObject;
            }
            return null;
        }

        /// <summary>
        /// 查找子节点对象
        /// 内部使用递归
        /// </summary>
        /// <param name="goParent">父对象</param>
        /// <param name="_childName">查找子对象的名称</param>
        /// <returns></returns>
        protected Transform FindChildNode(Transform _trans, string _childName)
        {
            //查找结果
            Transform searchTrans = null;
            searchTrans = _trans.Find(_childName);
            if (searchTrans == null)
            {
                foreach (Transform trans in _trans)
                {
                    searchTrans = FindChildNode(trans, _childName);
                    if (searchTrans != null)
                    {
                        return searchTrans;
                    }
                }
            }
            return searchTrans;
        }

        public Vector3 AnchorPosToWorld(Vector2 _pos)
        {
            return Tools.Ins.AnchorPosToWorld(_pos, GetComponentInParent<Canvas>());
        }

        public Vector2 WorldPosToAnchor(Vector3 _pos)
        {
            return Tools.Ins.WorldPosToAnchor(_pos, GetComponentInParent<Canvas>());
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(BaseUI), true)]
public class BaseUI_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认的Inspector界面
        DrawDefaultInspector();

        // 添加分隔线
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("编辑器工具", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            // 添加重置相机位置按钮
            if (GUILayout.Button("导出UI"))
            {
                UIMgr.ActionExportUI_Editor?.Invoke();

                // 获取当前活跃的编辑阶段（Stage）
                var stage = StageUtility.GetCurrentStage();

                // 判断是否是预制体编辑阶段
                if (stage is PrefabStage prefabStage)
                {
                    // 保存预制体修改
                    PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabStage.prefabContentsRoot));
                }
            }
        }
    }
}
#endif