using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TT_RTS_sample_scene_gallery : MonoBehaviour
{
    [Header("基本设置")]
    public Transform parentObject; // 包含子物体的父节点
    public Camera photoCamera; // 用于拍照的相机
    public string savePath = "Assets/CapturedImages/"; // 保存路径

    [Header("排列设置")]
    public Vector3 arrangementAxis = Vector3.right; // 排列方向（默认X轴）
    public float spacing = 5f; // 子物体之间的间隔

    [Header("相机设置")]
    public float cameraDistance = 10f; // 相机到物体的距离
    public Vector3 cameraOffset = Vector3.up; // 相机高度偏移
    public float fieldOfView = 25f; // 相机视野

    [Header("图片设置")]
    public int resolutionWidth = 1024; // 图片宽度
    public int resolutionHeight = 1024; // 图片高度

    private List<Transform> directChildren = new List<Transform>();
    private RenderTexture tempRenderTexture;
    private Texture2D captureTexture;

    private void Awake()
    {
        // 验证必要引用
        if (parentObject == null)
            parentObject = transform;

        if (photoCamera == null)
            photoCamera = GetComponent<Camera>();

        // 初始化渲染纹理和纹理
        tempRenderTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        captureTexture = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGBA32, false);
    }

    [ContextMenu("开始拍摄流程")]
    public void StartPhotographyProcess()
    {
        // 收集所有直接子物体（不包含孙节点）
        CollectDirectChildren();

        if (directChildren.Count == 0)
        {
            Debug.LogWarning("没有找到直接子物体");
            return;
        }

        // 排列子物体
        ArrangeChildren();

        // 准备相机
        SetupCamera();

        // 创建保存目录
        EnsureSaveDirectoryExists();

        // 为每个子物体拍照
        CaptureAllObjects();

        Debug.Log($"拍摄完成，共保存 {directChildren.Count} 张图片到 {savePath}");
    }

    // 收集所有直接子物体
    private void CollectDirectChildren()
    {
        directChildren.Clear();

        for (int i = 0; i < parentObject.childCount; i++)
        {
            Transform child = parentObject.GetChild(i);
            if (child.gameObject.activeInHierarchy)
            {
                directChildren.Add(child);
            }
        }
    }

    // 排列子物体
    private void ArrangeChildren()
    {
        Vector3 startPosition = new Vector3(1000, 0, 1000);

        for (int i = 0; i < directChildren.Count; i++)
        {
            // 计算位置：起始位置 + 偏移量
            Vector3 newPosition = startPosition + arrangementAxis * i * spacing;
            directChildren[i].position = newPosition;

            // 让物体面向相机
            FaceObjectToCamera(directChildren[i]);
        }
    }

    // 让物体面向相机
    private void FaceObjectToCamera(Transform target)
    {
        Vector3 lookDirection = photoCamera.transform.position - target.position;
        lookDirection.y = 0; // 保持Y轴方向不变，避免倾斜
        target.rotation = Quaternion.LookRotation(lookDirection);
    }

    // 准备相机设置
    private void SetupCamera()
    {
        if (photoCamera == null) return;

        // 设置相机参数
        photoCamera.fieldOfView = fieldOfView;
        photoCamera.clearFlags = CameraClearFlags.SolidColor;
        photoCamera.backgroundColor = new Color(0, 0, 0, 0); // 透明背景
        photoCamera.targetTexture = tempRenderTexture;
        photoCamera.transform.position = photoCamera.transform.position + new Vector3(0, 0.2f, 0);
    }

    // 确保保存目录存在
    private void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    // 为所有物体拍照
    private void CaptureAllObjects()
    {
        foreach (var child in directChildren)
        {
            CaptureSingleObject(child);
        }
    }

    // 为单个物体拍照
    private void CaptureSingleObject(Transform target)
    {
        // 移动相机到目标位置
        PositionCameraForTarget(target);

        // 隐藏其他物体
        HideOtherObjects(target);

        // 渲染并捕获图像
        photoCamera.Render();
        RenderTexture.active = tempRenderTexture;
        captureTexture.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        captureTexture.Apply();

        // 保存图像
        SaveTextureAsPNG(captureTexture, target.name);

        // 显示所有物体
        ShowAllObjects();
    }

    // 移动相机到适合拍摄目标的位置
    private void PositionCameraForTarget(Transform target)
    {
        if (photoCamera == null) return;

        // 计算相机位置：在目标的反方向一定距离处
        Vector3 cameraPosition = target.position - arrangementAxis * cameraDistance + cameraOffset;
        photoCamera.transform.position = cameraPosition;

        // 让相机面向目标
        photoCamera.transform.LookAt(target.position + cameraOffset);
        target.transform.LookAt(photoCamera.transform);
    }

    // 隐藏其他物体
    private void HideOtherObjects(Transform targetToShow)
    {
        foreach (var child in directChildren)
        {
            child.gameObject.SetActive(child == targetToShow);
        }
    }

    // 显示所有物体
    private void ShowAllObjects()
    {
        foreach (var child in directChildren)
        {
            child.gameObject.SetActive(true);
        }
    }

    // 保存纹理为PNG
    private void SaveTextureAsPNG(Texture2D texture, string objectName)
    {
        byte[] bytes = texture.EncodeToPNG();
        string fileName = $"{objectName}.png";
        string fullPath = Path.Combine(savePath, fileName);

        File.WriteAllBytes(fullPath, bytes);

        // 在编辑器中刷新资源窗口
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private void OnDestroy()
    {
        // 清理资源
        if (tempRenderTexture != null)
            Destroy(tempRenderTexture);

        if (captureTexture != null)
            Destroy(captureTexture);
    }
}
