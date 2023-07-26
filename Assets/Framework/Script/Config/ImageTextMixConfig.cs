using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ImageTextMixConfig : ConfigBase
{
    public Dictionary<string, ImageTextMixUnitConfig> Mix = new Dictionary<string, ImageTextMixUnitConfig>();

    public override void Init()
    {
        foreach (var mix in Mix)
        {
            Action<string> action = (str) =>
            {
                if (!string.IsNullOrEmpty(str))
                {
                    List<object> list = JsonConvert.DeserializeObject<List<object>>(str);
                    mix.Value.imgConfigs.Add(new TIMix_Image(list));
                }
            };
            action(mix.Value.img_c_1);
            action(mix.Value.img_c_2);
            action(mix.Value.img_c_3);
        }
    }
}

public class ImageTextMixUnitConfig
{
    public string tid;
    public string img_c_1;
    public string img_c_2;
    public string img_c_3;

    public List<TIMix_Image> imgConfigs = new List<TIMix_Image>();
}

public class TIMix_Image
{
    public string imagePath;
    public int width;
    public int height;

    public TIMix_Image(List<object> _objList)
    {
        imagePath = _objList[0] as string;
        width = int.Parse(_objList[1].ToString());
        height = int.Parse(_objList[2].ToString());
    }

    public TIMix_Image(object _imagePath, object _width, object _height)
    {
        imagePath = _imagePath as string;
        width = (int)_width;
        height = (int)_height;
    }
}