using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Rain.UI.Editor
{

    public class EditorDevTools : EditorWindow
    {
        static EditorDevTools_Style editorDevTools_Style;
        private List<EditorDevTools_Base> rootModules = new List<EditorDevTools_Base>();
        // 当前选中的模块路径（记录从根到叶子的完整路径）
        private Stack<EditorDevTools_Base> selectedPath = new Stack<EditorDevTools_Base>();


        [MenuItem("开发工具/开发工具", priority = 0)]
        static void ShowWindow()
        {
            EditorDevTools window = GetWindow<EditorDevTools>("Rain开发工具", typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow"));
            window.minSize = new Vector2(700, 700);
            editorDevTools_Style = new EditorDevTools_Style();
        }

        private void OnEnable()
        {
            // 初始化分页时传递当前窗口实例（this）
            InitializeModules();
        }

        private void InitializeModules()
        {
            // 创建一个三级结构的示例：UI -> 建筑UI -> 防御建筑
            var defenseBuildingUI = new EditorDevTools_Dev(this, editorDevTools_Style);
            rootModules.Add(defenseBuildingUI);

            var resourceBuildingUI = new EditorDevTools_UI(this, editorDevTools_Style);
            rootModules.Add(resourceBuildingUI);

            // 默认选中第一个根模块
            if (rootModules.Count > 0)
            {
                SelectModule(rootModules[0]);
            }
        }

        // 选择模块，更新选中路径
        private void SelectModule(EditorDevTools_Base module)
        {
            // 先清空选中路径
            selectedPath.Clear();
            
            // 构建从根到当前模块的路径
            var pathList = new List<EditorDevTools_Base>();
            EditorDevTools_Base current = module;
            
            // 先添加当前模块
            pathList.Add(current);
            
            // 然后添加其所有父模块
            while (current.Parent != null)
            {
                pathList.Add(current.Parent);
                current = current.Parent;
            }
            
            // 将路径反转并压入栈中（从根到叶子）
            for (int i = pathList.Count - 1; i >= 0; i--)
            {
                selectedPath.Push(pathList[i]);
            }
            
            // 更新所有模块的激活状态
            UpdateModuleStates();
        }

        // 更新所有模块的激活状态
        private void UpdateModuleStates()
        {
            // 重置所有模块状态
            ResetModuleStates(rootModules);

            // 激活选中路径上的所有模块
            foreach (var module in selectedPath)
            {
                module.isActive = true;
            }
        }

        // 递归重置所有模块状态
        private void ResetModuleStates(List<EditorDevTools_Base> modules)
        {
            foreach (var module in modules)
            {
                module.isActive = false;
                ResetModuleStates(module.subModules);
            }
        }

        Vector2 scrollPosition;
        private void OnGUI()
        {
            //EditorGUILayout.BeginScrollView(scrollPosition);
            DrawNavigationRecursive(rootModules, 0);
            DrawCurrentContent();
            //EditorGUILayout.EndScrollView();
        }

        // 递归绘制导航菜单
        private void DrawNavigationRecursive(List<EditorDevTools_Base> modules, int level)
        {
            var modulesToDraw = GetModulesForCurrentLevel(modules, level);
            if (modulesToDraw.Count == 0) return;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Space(level * 15);

            // 保存当前选中的模块
            EditorDevTools_Base selectedModule = null;
            foreach (var module in modulesToDraw)
            {
                if (module.isActive)
                {
                    selectedModule = module;
                    break;
                }
            }

            // 绘制带分割线的Toggle组
            for (int i = 0; i < modulesToDraw.Count; i++)
            {
                var module = modulesToDraw[i];

                // 绘制Toggle
                bool isSelected = module.isActive;
                bool newState = GUILayout.Toggle(isSelected, module.pageName, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

                // 如果状态发生变化
                if (newState != isSelected)
                {
                    // 先重置同级所有模块的状态
                    foreach (var otherModule in modulesToDraw)
                    {
                        otherModule.isActive = false;
                    }

                    if (newState)
                    {
                        SelectModule(module);
                    }
                    else if (selectedModule == module)
                    {
                        // 如果是取消选中当前选中的模块
                        // 如果有父模块，则选中父模块
                        if (module.Parent != null)
                        {
                            SelectModule(module.Parent);
                        }
                        else if (level == 0 && rootModules.Count > 0)
                        {
                            // 如果是根模块，则选中第一个根模块
                            // 这里可以选择不同的逻辑，比如不选中任何模块
                            SelectModule(rootModules[0]);
                        }
                    }
                }

                // 不是最后一个元素则绘制分割线
                if (i < modulesToDraw.Count - 1)
                {
                    GUILayout.Label("|", EditorStyles.label, GUILayout.Width(5));
                }
            }

            GUILayout.EndHorizontal();

            // 递归绘制下一级
            if (selectedPath.Count > level)
            {
                DrawNavigationRecursive(modulesToDraw, level + 1);
            }
        }
        private List<EditorDevTools_Base> GetModulesForCurrentLevel(List<EditorDevTools_Base> modules, int level)
        {
            if (level == 0) return modules;

            if (selectedPath.Count >= level)
            {
                var list = selectedPath.Reverse().ToList();
                return list[level - 1].subModules;
            }

            return new List<EditorDevTools_Base>();
        }

        // 绘制当前选中模块的内容
        private void DrawCurrentContent()
        {
            GUILayout.Space(10);

            if (selectedPath.Count == 0)
            {
                GUILayout.Label("请选择一个功能模块", EditorStyles.helpBox);
                return;
            }

            selectedPath.Peek().DrawContent();
        }
    }
}