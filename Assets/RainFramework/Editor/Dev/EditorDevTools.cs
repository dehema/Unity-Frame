using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Rain.UI.Editor
{

    public class EditorDevTools : EditorWindow
    {
        EditorDevTools_Style editorDevTools_Style = null;
        private List<EditorDevTools_Base> rootPages = new List<EditorDevTools_Base>();
        // 当前选中的模块路径 从子元素到根元素
        private List<EditorDevTools_Base> selectedPages = new List<EditorDevTools_Base>();



        [MenuItem("开发工具/开发工具 &d", priority = 0)]
        static void ShowWindow()
        {
            EditorDevTools window = GetWindow<EditorDevTools>("Rain开发工具", typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow"));
            window.minSize = new Vector2(700, 700);
        }


        private void OnEnable()
        {
            InitializePages();
        }

        private void OnGUI()
        {
            InitStyle();
            DrawNavigationRecursive(rootPages, 0);
            DrawCurrentContent();
        }

        /// <summary>
        /// 初始化并应用样式到所有页面
        /// </summary>
        void InitStyle()
        {
            // 如果样式未初始化，则创建新样式
            if (editorDevTools_Style == null)
                editorDevTools_Style = new EditorDevTools_Style(GUI.skin);
            
            // 递归设置样式到所有页面
            SetStyleRecursively(rootPages, editorDevTools_Style);
        }
        
        /// <summary>
        /// 递归设置样式到页面及其子页面
        /// </summary>
        /// <param name="pages">要设置样式的页面列表</param>
        /// <param name="style">要应用的样式</param>
        private void SetStyleRecursively(List<EditorDevTools_Base> pages, EditorDevTools_Style style)
        {
            foreach (var page in pages)
            {
                page.SetStyle(style);
                if (page.subPages != null && page.subPages.Count > 0)
                    SetStyleRecursively(page.subPages, style);
            }
        }

        private void FillPageData()
        {
            var EditorDevTools_Dev = new EditorDevTools_Dev(this);
            rootPages.Add(EditorDevTools_Dev);

            var resourceBuildingScene = new EditorDevTools_Scene(this);
            rootPages.Add(resourceBuildingScene);

            var resourceBuildingUI = new EditorDevTools_UI(this);
            rootPages.Add(resourceBuildingUI);
        }

        private void InitializePages()
        {
            FillPageData();
            for (int i = 0; i < rootPages.Count; i++)
            {
                EditorDevTools_Base page = rootPages[i];
                page.SetLevel(0);
                page.index = i;
            }

            // 默认选中第一个根模块
            if (rootPages.Count > 0)
            {
                SelectPage(rootPages[0]);
            }
        }

        public EditorDevTools_Base GetSelectPage(int _level)
        {
            int level = 0;
            EditorDevTools_Base page = null;
            do
            {
                foreach (var item in page == null ? rootPages : page.subPages)
                {
                    if (item.isActive)
                    {
                        if (level == _level)
                            return item;
                        page = item;
                        break;
                    }
                }
                level++;
            }
            while (level <= _level);
            return page;
        }

        // 选择模块，更新选中路径
        private void SelectPage(EditorDevTools_Base _page)
        {
            // 重置选中路径
            selectedPages.Clear();

            // 添加子模块
            List<EditorDevTools_Base> pathList = new List<EditorDevTools_Base>();
            EditorDevTools_Base current = _page;

            while (current.subPages.Count > 0)
            {
                EditorDevTools_Base selectedSubPage = null;
                foreach (var _subPage in current.subPages)
                {
                    if (_subPage.isActive)
                    {
                        selectedSubPage = _subPage;
                        break;
                    }
                }
                if (selectedSubPage == null)
                {
                    selectedSubPage = current.subPages[0];
                }
                selectedPages.Insert(0, selectedSubPage);
                current = selectedSubPage;
            }
            current = _page;
            // 添加当前模块
            selectedPages.Add(current);

            // 添加其所有父模块
            while (current.parent != null)
            {
                selectedPages.Add(current.parent);
                current = current.parent;
            }

            // 激活选中路径上的所有模块
            foreach (var _selectPage in selectedPages)
            {
                _selectPage.isActive = true;
            }
        }

        // 递归重置所有模块状态
        private void ResetPageStates(List<EditorDevTools_Base> _pages)
        {
            foreach (var _page in _pages)
            {
                _page.isActive = false;
            }
        }

        // 递归绘制导航菜单
        private void DrawNavigationRecursive(List<EditorDevTools_Base> _pages, int _level)
        {
            if (_pages.Count == 0) return;

            // 填充pageNames
            string[] pageNames = new string[_pages.Count];
            for (int i = 0; i < _pages.Count; i++)
            {
                EditorDevTools_Base page = _pages[i];
                pageNames[i] = page.pageName;

            }
            // 使用Toolbar实现导航按钮
            EditorDevTools_Base selectPage = GetSelectPage(_level);
            int selectedIndex = selectPage.index;
            int newSelectedIndex = GUILayout.Toolbar(selectedIndex, pageNames, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            // 如果索引发生变化
            if (selectedIndex != newSelectedIndex)
            {
                ResetPageStates(_pages);
                SelectPage(_pages[newSelectedIndex]);
            }

            // 递归绘制下一级
            if (selectedPages.Count > _level)
            {
                DrawNavigationRecursive(selectPage.subPages, _level + 1);
            }
        }


        // 绘制当前选中模块的内容
        private void DrawCurrentContent()
        {
            GUILayout.Space(10);

            if (selectedPages.Count == 0)
            {
                GUILayout.Label("请选择一个功能模块", EditorStyles.helpBox);
                return;
            }

            selectedPages.First().DrawContent();
        }
    }
}