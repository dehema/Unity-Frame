
namespace Rain.UI
{
    public class ViewConfig
    {
        public string viewName;
        public string comment;
        /// <summary>
        /// 分组 Common/RTS
        /// </summary>
        public string group;
        public string layer = "NormalLayer";
        public bool hasBg = true;
        public bool bgClose = false;
        public string bgColor;
        public ViewShowMethod showMethod;
        public UILoader uiLoader;
    }
}
