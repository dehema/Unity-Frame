public enum ViewLayer
{ 
    /// <summary>
    /// 普通UI
    /// </summary>
	NormalLayer = 0,
    /// <summary>
    /// 游戏菜单UI
    /// </summary>
	MenuLayer = 5,
    /// <summary>
    /// 弹窗UI
    /// </summary>
	DialogLayer = 10,
    /// <summary>
    /// 提示UI
    /// </summary>
	TipsLayer = 20,
    /// <summary>
    /// 引导UI
    /// </summary>
	GuideLayer = 25,
    /// <summary>
    /// 阻挡点击UI
    /// </summary>
	PreventLayer = 27,
    /// <summary>
    /// 光标特效
    /// </summary>
	CursorLayer = 28,
}

public class ViewGroup
{
	public const string Common = "Common";
	public const string RTS = "RTS";
}

public class ViewName
{
    /// <summary>
    /// 调试弹窗
    /// </summary>
	public const string DebugView = "DebugView";
    /// <summary>
    /// 奖励弹窗
    /// </summary>
	public const string BlockView = "BlockView";
    /// <summary>
    /// 资源栏
    /// </summary>
	public const string TopView = "TopView";
    /// <summary>
    /// loading
    /// </summary>
	public const string LoadingView = "LoadingView";
    /// <summary>
    /// 示例UI
    /// </summary>
	public const string ExampleView = "ExampleView";
    /// <summary>
    /// 提示弹窗UI
    /// </summary>
	public const string TipsView = "TipsView";
    /// <summary>
    /// 图文混合
    /// </summary>
	public const string ImageTextMixView = "ImageTextMixView";
    /// <summary>
    /// 读取页面
    /// </summary>
	public const string LoadSceneView = "LoadSceneView";
    /// <summary>
    /// 光标特效
    /// </summary>
	public const string CursorEffectView = "CursorEffectView";
    /// <summary>
    /// 暂停游戏菜单
    /// </summary>
	public const string EscView = "EscView";
    /// <summary>
    /// 设置
    /// </summary>
	public const string SettingView = "SettingView";
    /// <summary>
    /// 游戏胜利
    /// </summary>
	public const string GameWinView = "GameWinView";
    /// <summary>
    /// 游戏失败
    /// </summary>
	public const string GameOverView = "GameOverView";
    /// <summary>
    /// UI组件示例
    /// </summary>
	public const string UIControlDemoView = "UIControlDemoView";
    /// <summary>
    /// UI示例
    /// </summary>
	public const string DemoView = "DemoView";
    /// <summary>
    /// 翻卡牌
    /// </summary>
	public const string CardMatchGameView = "CardMatchGameView";
    /// <summary>
    /// 倒计时
    /// </summary>
	public const string CountDownView = "CountDownView";
    /// <summary>
    /// RTS单位
    /// </summary>
	public const string RTSUnitTestView = "RTSUnitTestView";
}