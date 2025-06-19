using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorBuildScript
{
    [MenuItem("Build/Android")]
    public static void BuildAndroid()
    {
        // ��ȡ�����в���
        string[] args = System.Environment.GetCommandLineArgs();
        string outputPath = ParseCommandLineArgument(args, "-outputPath");

        // ���δָ�����·����ʹ��Ĭ��ֵ
        if (string.IsNullOrEmpty(outputPath))
        {
            outputPath = "Builds/Android/MyGame.apk";
            Debug.Log($"ʹ��Ĭ�����·��: {outputPath}");
        }
        else
        {
            // ȷ��Ŀ¼����
            string directory = System.IO.Path.GetDirectoryName(outputPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            Debug.Log($"ʹ��������ָ�������·��: {outputPath}");
        }

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = GetEnabledScenes();
        options.locationPathName = outputPath;
        options.target = BuildTarget.Android;
        options.options = BuildOptions.None;

        BuildPipeline.BuildPlayer(options);
    }

    private static string[] GetEnabledScenes()
    {
        return System.Array.FindAll(
            EditorBuildSettings.scenes,
            scene => scene.enabled
        ).Select(scene => scene.path).ToArray();
    }

    // ���������в����ĸ�������
    private static string ParseCommandLineArgument(string[] args, string paramName)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == paramName && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
