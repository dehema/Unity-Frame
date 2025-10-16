using System.Collections.Generic;

namespace Rain.UI
{
    public class UIViewConfig
    {
        public Dictionary<string, LayerConfig> layer;
        public Dictionary<string, Dictionary<string, ViewConfig>> view;

        public Dictionary<string, ViewConfig> allViewConfig = new Dictionary<string, ViewConfig>();
    }
}
