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
        // 获取命令行参数
        string[] args = System.Environment.GetCommandLineArgs();
        string outputPath = ParseCommandLineArgument(args, "-outputPath");

        // 如果未指定输出路径，使用默认值
        if (string.IsNullOrEmpty(outputPath))
        {
            outputPath = "Builds/Android/MyGame.apk";
            Debug.Log($"使用默认输出路径: {outputPath}");
        }
        else
        {
            // 确保目录存在
            string directory = System.IO.Path.GetDirectoryName(outputPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            Debug.Log($"使用命令行指定的输出路径: {outputPath}");
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

    // 解析命令行参数的辅助方法
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
