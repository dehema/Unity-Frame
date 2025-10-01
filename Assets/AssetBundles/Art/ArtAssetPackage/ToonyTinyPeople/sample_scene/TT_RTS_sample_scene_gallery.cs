using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TT_RTS_sample_scene_gallery : MonoBehaviour
{
    [Header("��������")]
    public Transform parentObject; // ����������ĸ��ڵ�
    public Camera photoCamera; // �������յ����
    public string savePath = "Assets/CapturedImages/"; // ����·��

    [Header("��������")]
    public Vector3 arrangementAxis = Vector3.right; // ���з���Ĭ��X�ᣩ
    public float spacing = 5f; // ������֮��ļ��

    [Header("�������")]
    public float cameraDistance = 10f; // ���������ľ���
    public Vector3 cameraOffset = Vector3.up; // ����߶�ƫ��
    public float fieldOfView = 25f; // �����Ұ

    [Header("ͼƬ����")]
    public int resolutionWidth = 1024; // ͼƬ���
    public int resolutionHeight = 1024; // ͼƬ�߶�

    private List<Transform> directChildren = new List<Transform>();
    private RenderTexture tempRenderTexture;
    private Texture2D captureTexture;

    private void Awake()
    {
        // ��֤��Ҫ����
        if (parentObject == null)
            parentObject = transform;

        if (photoCamera == null)
            photoCamera = GetComponent<Camera>();

        // ��ʼ����Ⱦ���������
        tempRenderTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        captureTexture = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGBA32, false);
    }

    [ContextMenu("��ʼ��������")]
    public void StartPhotographyProcess()
    {
        // �ռ�����ֱ�������壨��������ڵ㣩
        CollectDirectChildren();

        if (directChildren.Count == 0)
        {
            Debug.LogWarning("û���ҵ�ֱ��������");
            return;
        }

        // ����������
        ArrangeChildren();

        // ׼�����
        SetupCamera();

        // ��������Ŀ¼
        EnsureSaveDirectoryExists();

        // Ϊÿ������������
        CaptureAllObjects();

        Debug.Log($"������ɣ������� {directChildren.Count} ��ͼƬ�� {savePath}");
    }

    // �ռ�����ֱ��������
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

    // ����������
    private void ArrangeChildren()
    {
        Vector3 startPosition = new Vector3(1000, 0, 1000);

        for (int i = 0; i < directChildren.Count; i++)
        {
            // ����λ�ã���ʼλ�� + ƫ����
            Vector3 newPosition = startPosition + arrangementAxis * i * spacing;
            directChildren[i].position = newPosition;

            // �������������
            FaceObjectToCamera(directChildren[i]);
        }
    }

    // �������������
    private void FaceObjectToCamera(Transform target)
    {
        Vector3 lookDirection = photoCamera.transform.position - target.position;
        lookDirection.y = 0; // ����Y�᷽�򲻱䣬������б
        target.rotation = Quaternion.LookRotation(lookDirection);
    }

    // ׼���������
    private void SetupCamera()
    {
        if (photoCamera == null) return;

        // �����������
        photoCamera.fieldOfView = fieldOfView;
        photoCamera.clearFlags = CameraClearFlags.SolidColor;
        photoCamera.backgroundColor = new Color(0, 0, 0, 0); // ͸������
        photoCamera.targetTexture = tempRenderTexture;
        photoCamera.transform.position = photoCamera.transform.position + new Vector3(0, 0.2f, 0);
    }

    // ȷ������Ŀ¼����
    private void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    // Ϊ������������
    private void CaptureAllObjects()
    {
        foreach (var child in directChildren)
        {
            CaptureSingleObject(child);
        }
    }

    // Ϊ������������
    private void CaptureSingleObject(Transform target)
    {
        // �ƶ������Ŀ��λ��
        PositionCameraForTarget(target);

        // ������������
        HideOtherObjects(target);

        // ��Ⱦ������ͼ��
        photoCamera.Render();
        RenderTexture.active = tempRenderTexture;
        captureTexture.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        captureTexture.Apply();

        // ����ͼ��
        SaveTextureAsPNG(captureTexture, target.name);

        // ��ʾ��������
        ShowAllObjects();
    }

    // �ƶ�������ʺ�����Ŀ���λ��
    private void PositionCameraForTarget(Transform target)
    {
        if (photoCamera == null) return;

        // �������λ�ã���Ŀ��ķ�����һ�����봦
        Vector3 cameraPosition = target.position - arrangementAxis * cameraDistance + cameraOffset;
        photoCamera.transform.position = cameraPosition;

        // ���������Ŀ��
        photoCamera.transform.LookAt(target.position + cameraOffset);
        target.transform.LookAt(photoCamera.transform);
    }

    // ������������
    private void HideOtherObjects(Transform targetToShow)
    {
        foreach (var child in directChildren)
        {
            child.gameObject.SetActive(child == targetToShow);
        }
    }

    // ��ʾ��������
    private void ShowAllObjects()
    {
        foreach (var child in directChildren)
        {
            child.gameObject.SetActive(true);
        }
    }

    // ��������ΪPNG
    private void SaveTextureAsPNG(Texture2D texture, string objectName)
    {
        byte[] bytes = texture.EncodeToPNG();
        string fileName = $"{objectName}.png";
        string fullPath = Path.Combine(savePath, fileName);

        File.WriteAllBytes(fullPath, bytes);

        // �ڱ༭����ˢ����Դ����
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private void OnDestroy()
    {
        // ������Դ
        if (tempRenderTexture != null)
            Destroy(tempRenderTexture);

        if (captureTexture != null)
            Destroy(captureTexture);
    }
}
