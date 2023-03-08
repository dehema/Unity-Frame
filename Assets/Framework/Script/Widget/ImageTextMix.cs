using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

//思路如下
//1.现在原文本字符的基础上加一个空格，获取一个空格所占的宽度，有的文本组件开启了bestFit字段，所以加一个空格会导致计算出的宽度小于正常宽度，但实际情况可以忽略不计。
//2.替换所有的占位字符为不小于其宽度数量的空格，如果这张图占30个像素，但是这个文本组件的设置一个空格才20像素，则替换为两个空格，同时记录下替换空格的索引
//3.根据空格索引的位置生成图片，图片的位置是 索引空格的光标位置（左上角）+宽度（空格数量*空格宽度/2）-高度（单行高度/2）

//之前的问题在于图片的位置是一个一个生成的，第一张图片生成之后，替换的空格字符就会导致整个文本组件的设置改变，应该把数据处理好，最后一起生成图片

/// <summary>
/// 图文混合脚本（需要配合图文配置和多语言文本）
/// </summary>
[RequireComponent(typeof(Text))]
public class ImageTextMix : MonoBehaviour
{
    Text text;
    public string mixID;
    ImageTextMixUnitConfig config;
    RectTransform rect;
    string textStr;
    //空格宽度
    float spaceUnitLenth;
    TextGenerator generator;
    int spaceNum;
    List<int> replaceStrIndex = new List<int>();

    List<GameObject> createImages = new List<GameObject>();

    private void Awake()
    {
        text = GetComponent<Text>();
        rect = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        config = ConfigMgr.Ins.imageTextMix.Mix[mixID];
        textStr = LangMgr.Ins.Get(config.tid);
        GetSpaceUnitWidth();
        InitGenerator();
        InitStr();
        CreateImages();
    }

    private void GetSpaceUnitWidth()
    {
        string _textStr = " " + textStr.Remove(0, 1);
        generator = new TextGenerator();
        var size = rect.rect.size;
        var settings = text.GetGenerationSettings(size);
        generator.Populate(_textStr, settings);
        spaceUnitLenth = generator.characters[0].charWidth;
    }

    void InitStr()
    {
        replaceStrIndex.Clear();
        string replaceStr;
        for (int i = 0; i < config.imgConfigs.Count; i++)
        {
            TIMix_Image imgConfig = config.imgConfigs[i];
            int charIndex = 0;
            replaceStr = GetReplaceStr(imgConfig, i, ref charIndex);
            string spaceIndexStr = "{" + i + "}";
            textStr = textStr.Replace(spaceIndexStr, replaceStr);
            InitGenerator();
            if (string.IsNullOrEmpty(replaceStr)) continue;
            replaceStrIndex.Add(charIndex);
        }
        text.text = textStr;
    }

    private void InitGenerator()
    {
        generator = new TextGenerator();
        var size = rect.rect.size;
        var settings = text.GetGenerationSettings(size);
        generator.Populate(textStr, settings);
    }

    private string GetReplaceStr(TIMix_Image _imgConfig, int _index, ref int charIndex)
    {
        charIndex = textStr.IndexOf("{" + _index + "}");
        if (generator.characters.Count <= charIndex)
        {
            return "";
        }
        //计算空格数量 
        spaceNum = Mathf.CeilToInt(_imgConfig.width / spaceUnitLenth);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < spaceNum; i++)
        {
            sb.Append(" ");
        }
        return sb.ToString();
    }

    void CreateImages()
    {
        foreach (var item in createImages)
        {
            Destroy(item);
        }
        createImages.Clear();
        for (int i = 0; i < replaceStrIndex.Count; i++)
        {
            int charIndex = replaceStrIndex[i];
            TIMix_Image _imgConfig = config.imgConfigs[i];
            //创建图片
            GameObject go = Tools.Ins.Create2DGo("img1", transform);
            createImages.Add(go);
            Image img = go.AddComponent<Image>();
            img.preserveAspect = true;
            img.sprite = Resources.Load<Sprite>(_imgConfig.imagePath);
            RectTransform imgRect = go.GetComponent<RectTransform>();
            imgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _imgConfig.width);
            imgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _imgConfig.height);
            //获取位置
            Vector2 cursorPos = generator.characters[charIndex].cursorPos;
            cursorPos += new Vector2(spaceNum * (spaceUnitLenth / 2), -generator.lines[0].height / 2);
            imgRect.anchoredPosition = cursorPos;
        }
    }
}
