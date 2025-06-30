using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.Launcher;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rain.Tests
{
    public class DemoAssetManager : MonoBehaviour
    {
        IEnumerator Start()
        {
            /*----------所有加载均会自动判断是Resources资产还是AssetBundle资产----------*/
            
            // 编辑器模式，无需每次修改资源都按F8
            RA.Asset.IsEditorMode = true;
            
            
            /*-------------------------------------同步加载-------------------------------------*/
            // 加载单个资产
            GameObject go = RA.Asset.Load<GameObject>("Cube");

            // assetName：资产名
            // subAssetName：子资产名，使用Multiple模式的Sprite图片则可使用
            // 指定加载模式REMOTE_ASSET_BUNDLE，加载远程AssetBundle资产，需要配置AssetRemoteAddress = "http://127.0.0.1:6789/remote"
            Sprite sprite = RA.Asset.Load<Sprite>("PackForest01", "PackForest01_12", AssetMgr.AssetAccessMode.REMOTE_ASSET_BUNDLE);
            
            // 加载此资源的全部资产
            RA.Asset.LoadAll("Cube");
            // 加载此资源的全部子资产
            RA.Asset.LoadSub("Cube");
            
            
            /*-------------------------------------异步加载-------------------------------------*/
            RA.Asset.LoadAsync<GameObject>("Cube", (go) =>
            {
                GameObject goo = Instantiate(go);
            });

            // async/await方式（无多线程，WebGL也可使用）
            // await RA.Asset.LoadAsync<GameObject>("Cube");
            // 或者
            // BaseLoader load = RA.Asset.LoadAsync<GameObject>("Cube");
            // await load;
            
            // 协程方式
            yield return RA.Asset.LoadAsync<GameObject>("Cube");
            // 或者
            BaseLoader load2 = RA.Asset.LoadAsync<GameObject>("Cube");
            yield return load2;
            GameObject go2 = load2.GetAssetObject<GameObject>();
            
            // 加载此资源的全部资产
            BaseLoader loaderAll = RA.Asset.LoadAllAsync("Cube");
            yield return loaderAll;
            Dictionary<string, Object> allAsset = loaderAll.GetAllAssetObject();
            
            // 加载此资源的全部子资产
            BaseLoader loaderSub = RA.Asset.LoadSubAsync("Atlas");
            yield return loaderSub;
            Dictionary<string, Sprite> allAsset2 = loaderSub.GetAllAssetObject<Sprite>();
            
            
            /*-------------------------------------加载文件夹内首层资产-------------------------------------*/
            // 加载文件夹内首层资产（不遍历所有文件夹）
            RA.Asset.LoadDir("NewFolder");
            
            // async/await方式（无多线程，WebGL也可使用）
            // BaseDirLoader loadDir = RA.Asset.LoadDirAsync("NewFolder", () => { });
            // await loadDir;
            
            // 加载文件夹内资产
            BaseDirLoader loadDir2 = RA.Asset.LoadDirAsync("NewFolder", () => { });
            yield return loadDir2;
            
            // 你可以查看所有资产的BaseLoader
            List<BaseLoader> loaders = loadDir2.Loaders;
            
            // 也可以这样设置查看加载进度
            foreach (var item in RA.Asset.LoadDirAsyncCoroutine("NewFolder"))
            {
                yield return item;
            }

            // 也可以这样
            var loadDir3 = RA.Asset.LoadDirAsyncCoroutine("NewFolder").GetEnumerator();
            while (loadDir3.MoveNext())
            {
                yield return loadDir3.Current;
            }
            
            
            /*-------------------------------------其他功能-------------------------------------*/
            // 获取此资源的全部资产
            Dictionary<string, Object> allAsset3 = RA.Asset.GetAllAssetObject("Cube");
            
            // 只获取指定类型
            Dictionary<string, Sprite> allAsset4 = RA.Asset.GetAllAssetObject<Sprite>("Atlas");
            
            // 获取单个资产
            GameObject go3 = RA.Asset.GetAssetObject<GameObject>("Cube");
            
            // 获取加载进度
            float loadProgress = RA.Asset.GetLoadProgress("Cube");

            // 获取所有加载器的进度
            float loadProgress2 = RA.Asset.GetLoadProgress();

            // 同步卸载资产
            RA.Asset.Unload("Cube", false); //根据AbPath卸载资产，如果设置为 true，完全卸载。

            // 异步卸载资产
            RA.Asset.UnloadAsync("Cube", false, () =>
            {
                // 卸载资产完成
            });
            
            
            /*-------------------------------------其他类型加载示例-------------------------------------*/
            // 加载场景，别忘了加载天空盒材质，不然会变紫色，并且这种方式不能加载Resources目录中的场景（不过可以手动放入Build Setting处）
            RA.Asset.Load("Scene");
            SceneManager.LoadScene("Scene");
            
            // 使用图集首先需要，加载图集
            RA.Asset.Load("SpriteAtlas");
            
            // 假如将图集与图片改为同一AB名，则无需预先加载图集
            RA.Asset.LoadAsync<Sprite>("PackForest_2", sprite =>
            {
                RLog.Log(sprite);
            });
            
            // 图片加载需要小心区分Texture2D和Sprite，当资源被当成Texture2D加载后，则加载不出Sprite类型
            RA.Asset.Load<Texture2D>("PackForest_2");
        }
    }
}
