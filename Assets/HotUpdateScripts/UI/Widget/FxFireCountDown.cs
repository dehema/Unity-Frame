using UnityEngine;

/// <summary>
/// 火焰倒计时特效控制器
/// 基于力量值控制火焰、火花和烟雾特效
/// </summary>
public class FxFireCountDown : MonoBehaviour
{
    [Tooltip("火焰强度值（0-1）")]
    [Range(0f, 1f)]
    [SerializeField] private float power = 0f;
    
    private float _lastPower = 0f;

    [Header("特效组件")]
    [Tooltip("火焰特效")]
    [SerializeField] private ParticleSystem ps_fire;
    
    [Tooltip("火花特效")]
    [SerializeField] private ParticleSystem ps_ember;
    
    [Tooltip("烟雾特效")]
    [SerializeField] private ParticleSystem ps_smoke;
    
    [Header("参数设置")]
    [Tooltip("烟雾显示的阈值")]
    [SerializeField] private float smokeShowThreshold = 0.7f;
    
    [Tooltip("火焰最小缩放")]
    [SerializeField] private float minFireScale = 0.6f;
    
    [Tooltip("火焰最大缩放")]
    [SerializeField] private float maxFireScale = 1f;
    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        _lastPower = power;
        
        // 确保组件存在
        if (ps_fire == null || ps_ember == null || ps_smoke == null)
        {
            Debug.LogError($"{gameObject.name}: 缺少粒子系统组件引用！");
            enabled = false;
            return;
        }
        
        // 初始化时隐藏烟雾
        ps_smoke.gameObject.SetActive(false);
        
        // 初始化时刷新一次所有特效
        RefreshEffects();
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    private void Update()
    {
        // 只在power值变化时才刷新特效，减少不必要的计算
        if (!Mathf.Approximately(_lastPower, power))
        {
            RefreshEffects();
            _lastPower = power;
        }
    }

    /// <summary>
    /// 设置火焰强度值
    /// </summary>
    /// <param name="val">强度值（0-1）</param>
    public void SetPowerValue(float val)
    {
        // 限制值范围
        power = Mathf.Clamp01(val);
        
        // 立即刷新特效
        RefreshEffects();
        _lastPower = power;
    }

    /// <summary>
    /// 刷新所有特效
    /// </summary>
    private void RefreshEffects()
    {
        // 更新火焰特效
        UpdateFireEffect();
        
        // 更新火花特效
        UpdateEmberEffect();
        
        // 更新烟雾特效
        UpdateSmokeEffect();
    }
    
    /// <summary>
    /// 更新火焰特效
    /// </summary>
    private void UpdateFireEffect()
    {
        // 计算火焰缩放比例
        float fireScale = Mathf.Lerp(minFireScale, maxFireScale, power);
        ps_fire.transform.localScale = new Vector3(fireScale, fireScale, 1f);
    }
    
    /// <summary>
    /// 更新火花特效
    /// </summary>
    private void UpdateEmberEffect()
    {
        // 获取主模块
        var main = ps_ember.main;
        
        // 调整火花生命周期
        float emberStartLifeTime = Mathf.Lerp(0.5f, 0.76f, power);
        main.startLifetime = new ParticleSystem.MinMaxCurve(emberStartLifeTime);
        
        // 调整火花速度
        float emberStartSpeed = Mathf.Lerp(2f, 6f, power);
        main.startSpeed = new ParticleSystem.MinMaxCurve(emberStartSpeed);
        
        // 调整火花发射频率
        float emissionRate = Mathf.Lerp(6f, 15f, power);
        var emission = ps_ember.emission;
        emission.rateOverTime = emissionRate;
    }
    
    /// <summary>
    /// 更新烟雾特效
    /// </summary>
    private void UpdateSmokeEffect()
    {
        // 调整烟雾位置
        float smokePosY = Mathf.Lerp(0.8f, 1.2f, power);
        ps_smoke.transform.localPosition = new Vector3(0f, smokePosY, 0f);
        
        // 调整烟雾缩放
        float smokeScale = Mathf.Max(0f, Mathf.Lerp(0f, 1f, power) - 0.2f);
        ps_smoke.transform.localScale = new Vector3(smokeScale, smokeScale, smokeScale);
        
        // 调整烟雾粒子参数
        var main = ps_smoke.main;
        main.startSpeed = Mathf.Lerp(1f, 3f, Mathf.Max(0f, power - 0.5f));
        main.startLifetime = Mathf.Lerp(3f, 0.6f, Mathf.Max(0f, power - 0.5f));
        
        // 根据阈值控制烟雾显隐
        if (_lastPower < smokeShowThreshold && power >= smokeShowThreshold)
        {
            ps_smoke.gameObject.SetActive(true);
        }
        else if (_lastPower >= smokeShowThreshold && power < smokeShowThreshold)
        {
            ps_smoke.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 获取当前火焰强度
    /// </summary>
    public float GetPower()
    {
        return power;
    }
}
