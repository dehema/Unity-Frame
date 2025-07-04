using UnityEditor;
using UnityEngine;

namespace Rain.Core.Editor
{
    public class CreatePoolsPreset
    {
        [MenuItem("Assets/【Rain预加载对象池）/（PoolsPreset.asset）", false, 1040)]
        private static void CreateScriptObject()
        {
            PoolsPreset config = ScriptableObject.CreateInstance<PoolsPreset>();
            ProjectWindowUtil.CreateAsset(config, "PoolsPreset.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
