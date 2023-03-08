using UnityEngine;
using UnityEngine.UI;
using System;
//using Boo.Lang;

/// <summary>
/// 序列帧动画播放器
/// 支持UGUI的Image和Unity2D的SpriteRenderer
/// </summary>
public class FrameAnimator : MonoBehaviour
{
	/// <summary>
	/// 序列帧
	/// </summary>
	public Sprite[] Frames { get { return frames; } set { frames = value; } }

	[SerializeField] private Sprite[] frames = null;
	//public List<Sprite> frames = new List<Sprite>(50);
	/// <summary>
	/// 帧率，为正时正向播放，为负时反向播放
	/// </summary>
	public float Framerate { get { return framerate; } set { framerate = value; } }

	[SerializeField] private float framerate = 20.0f;

	/// <summary>
	/// 是否忽略timeScale
	/// </summary>
	public bool IgnoreTimeScale { get { return ignoreTimeScale; } set { ignoreTimeScale = value; } }

	[SerializeField] private bool ignoreTimeScale = true;

	/// <summary>
	/// 是否循环
	/// </summary>
	public bool Loop { get { return loop; } set { loop = value; } }

	[SerializeField] private bool loop = true;

	//动画曲线
	[SerializeField] private AnimationCurve curve = new AnimationCurve(new Keyframe(0, 1, 0, 0), new Keyframe(1, 1, 0, 0));

	/// <summary>
	/// 结束事件
	/// 在每次播放完一个周期时触发
	/// 在循环模式下触发此事件时，当前帧不一定为结束帧
	/// </summary>
	public event Action FinishEvent;

	//目标Image组件
	private Image image;
	//目标SpriteRenderer组件
	private SpriteRenderer spriteRenderer;
	//当前帧索引
	private int currentFrameIndex = 0;
	//下一次更新时间
	private float timer = 0.0f;
	//当前帧率，通过曲线计算而来
	private float currentFramerate = 20.0f;

	/// <summary>
	/// 重设动画
	/// </summary>
	public void Reset()
	{
		currentFrameIndex = framerate < 0 ? frames.Length - 1 : 0;
	}

	/// <summary>
	/// 从停止的位置播放动画
	/// </summary>
	public void Play()
	{
		this.enabled = true;
	}

	/// <summary>
	/// 暂停动画
	/// </summary>
	public void Pause()
	{
		this.enabled = false;
	}

	/// <summary>
	/// 停止动画，将位置设为初始位置
	/// </summary>
	public void Stop()
	{
		Pause();
		Reset();
	}

	//自动开启动画
	void Start()
	{
		image = this.GetComponent<Image>();
		spriteRenderer = this.GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
		if (image == null && spriteRenderer == null)
		{
			Debug.LogWarning("No available component found. 'Image' or 'SpriteRenderer' required.", this.gameObject);
		}
#endif
	}

	void Update()
	{
		//帧数据无效，禁用脚本
		if (frames == null || frames.Length == 0)
		{
			this.enabled = false;
		}
		else
		{
			//从曲线值计算当前帧率
			float curveValue = curve.Evaluate((float)currentFrameIndex / frames.Length);
			float curvedFramerate = curveValue * framerate;
			//帧率有效
			if (curvedFramerate != 0)
			{
				//获取当前时间
				float time = ignoreTimeScale ? Time.unscaledTime : Time.time;
				//计算帧间隔时间
				float interval = Mathf.Abs(1.0f / curvedFramerate);
				//满足更新条件，执行更新操作
				if (time - timer > interval)
				{
					//执行更新操作
					DoUpdate();
				}
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogWarning("Framerate got '0' value, animation stopped.");
			}
#endif
		}
	}

	//具体更新操作
	private void DoUpdate()
	{
		//计算新的索引
		int nextIndex = currentFrameIndex + (int)Mathf.Sign(currentFramerate);
		//索引越界，表示已经到结束帧
		if (nextIndex < 0 || nextIndex >= frames.Length)
		{
			//广播事件
			if (FinishEvent != null)
			{
				FinishEvent();
			}
			//非循环模式，禁用脚本
			if (loop == false)
			{
				currentFrameIndex = Mathf.Clamp(currentFrameIndex, 0, frames.Length - 1);
				this.enabled = false;
				return;
			}
		}
		//钳制索引
		currentFrameIndex = nextIndex % frames.Length;
		//更新图片
		if (image != null)
		{
			image.sprite = frames[currentFrameIndex];
		}
		else if (spriteRenderer != null)
		{
			spriteRenderer.sprite = frames[currentFrameIndex];
		}
		//设置计时器为当前时间
		timer = ignoreTimeScale ? Time.unscaledTime : Time.time;
	}
}

