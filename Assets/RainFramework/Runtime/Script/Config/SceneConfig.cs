using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rain.UI;

namespace Rain.Core
{

    public class AllSceneConfig
    {
        public Dictionary<string, SceneConfig> scenes;
    }

    public class SceneConfig
    {
        public string sceneName;
        public string comment;
    }
}
