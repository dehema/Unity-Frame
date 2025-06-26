using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rain.Core;
using UnityEngine.UI;

public class RainDemo : MonoBehaviour
{
    Image imgBuild;
    void Start()
    {
        imgBuild = transform.Find("imgBuild").GetComponent<Image>();
        Sprite sprite = AssetManager.Ins.Load<Sprite>("UI/Building/beastmen_centigors1");
        //Sprite sprite = Resources.Load<Sprite>("UI/Building/beastmen_centigors1");
        imgBuild.sprite = sprite;
        //ÐÂÄ£¿é

        //AudioClip clip = RA.Asset.Load<AudioClip>("audio/Electronic high shot");
        //AudioManager.Ins.PlayAudioEffect(clip);

    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    GameObject testprefab = RA.Asset.Load<GameObject>("testprefab");
        //    if (testprefab)
        //    {
        //        GameObject.Instantiate(testprefab);
        //    }
        //    AudioManager.Ins.PlayAudioEffect("Electronic high shot");
        //}
    }
}
