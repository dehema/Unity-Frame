using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

public class ScriptGenerator
{
    string jsonDir;
    string scriptDir;
    public ScriptGenerator(string _jsonDir, string _scriptDir)
    {
        jsonDir = _jsonDir;
        scriptDir = _scriptDir;
    }

    /// <summary>
    /// 为指定目录中的所有JSON文件生成C#映射类
    /// </summary>
    public void GenerateScriptsFromJsonFiles()
    {
        // 确保目录存在
        if (!Directory.Exists(jsonDir))
        {
            Debug.LogError($"JSON directory does not exist: {jsonDir}");
            return;
        }

        if (!Directory.Exists(scriptDir))
        {
            Directory.CreateDirectory(scriptDir);
            Debug.Log($"Created script directory: {scriptDir}");
        }

        // 获取所有JSON文件
        string[] jsonFiles = Directory.GetFiles(jsonDir, "*.json", SearchOption.AllDirectories);
        Debug.Log($"Found {jsonFiles.Length} JSON files to process");

        foreach (string jsonFile in jsonFiles)
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonFile);
                jsonContent = jsonContent
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("  ", "");
                string fileName = Path.GetFileNameWithoutExtension(jsonFile);
                string className = CapitalizeFirstLetter(fileName);

                // 解析JSON以确定结构
                JToken jsonData = JToken.Parse(jsonContent);

                // 生成类代码
                string classCode = GenerateClassFromJson(className, jsonData);

                // 写入文件
                string outputPath = Path.Combine(scriptDir, $"{className}Config.cs");
                File.WriteAllText(outputPath, classCode);

                Debug.Log($"Generated class {className} at {outputPath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error processing {jsonFile}: {ex.Message}");
            }
        }

        Debug.Log("脚本生成成功");
    }

    /// <summary>
    /// 从JSON结构生成C#类
    /// </summary>
    private string GenerateClassFromJson(string className, JToken jsonData)
    {
        StringBuilder sb = new StringBuilder();

        // 添加头部
        sb
          .AppendLine("using System.Collections.Generic;")
          .AppendLine()
          .AppendLine($"public partial class {className}Config : ConfigBase")
          .AppendLine("{");

        JObject jObject = jsonData as JObject;

        foreach (var property in jObject.Properties())
        {
            string propertyName = property.Name;
            string propertyNamePascal = propertyName;

            // 处理数组类型的属性
            if (property.Value is JArray arrayValue && arrayValue.Count > 0)
            {
                // 为数组元素创建一个嵌套类
                string itemClassName = CapitalizeFirstLetter(propertyNamePascal);
                if (itemClassName.EndsWith("s"))
                {
                    itemClassName = itemClassName.Substring(0, itemClassName.Length - 1);
                }
                else
                {
                    itemClassName += "ItemConfig";
                }

                // 添加数组属性
                string keyType = GetCSharpType((arrayValue[0] as JObject).Properties().ToArray().First().Value);
                sb
                 .AppendLine($"    public Dictionary<{keyType}, {itemClassName}> {propertyNamePascal} = new Dictionary<{keyType}, {itemClassName}>();")
                  ;
            }
            sb.AppendLine();
        }

        sb.AppendLine("    public override void Init()");
        sb.AppendLine("    {");
        sb.AppendLine("    ");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();

        foreach (var property in jObject.Properties())
        {
            string propertyName = property.Name;
            string propertyNamePascal = propertyName;

            // 处理数组类型的属性
            if (property.Value is JArray arrayValue && arrayValue.Count > 0)
            {
                // 为数组元素创建一个嵌套类
                string itemClassName = CapitalizeFirstLetter(propertyNamePascal);
                if (itemClassName.EndsWith("s"))
                {
                    itemClassName = itemClassName.Substring(0, itemClassName.Length - 1);
                }
                else
                {
                    itemClassName += "ItemConfig";
                }

                // 如果数组中有对象，创建嵌套类
                if (arrayValue.Count > 0 && arrayValue[0] is JObject firstItem)
                {
                    sb
                      .AppendLine($"public class {itemClassName}")
                      .AppendLine("{")
                      .Append(GeneratePropertiesFromJObject(firstItem, 4))
                      .AppendLine("}");
                }
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// 从JObject生成属性
    /// </summary>
    private string GeneratePropertiesFromJObject(JObject jObject, int indentLevel)
    {
        StringBuilder sb = new StringBuilder();
        string indent = new string(' ', indentLevel);

        foreach (var property in jObject.Properties())
        {
            string propertyName = property.Name;
            string propertyType = GetCSharpType(property.Value);
            string propertyNamePascal = propertyName;

            sb
              .AppendLine($"{indent}public {propertyType} {propertyNamePascal};")
              ;
        }

        return sb.ToString();
    }

    /// <summary>
    /// 从JSON值确定C#类型
    /// </summary>
    private string GetCSharpType(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Integer:
                return "int";
            case JTokenType.Float:
                return "float";
            case JTokenType.String:
                return "string";
            case JTokenType.Boolean:
                return "bool";
            case JTokenType.Date:
                return "DateTime";
            case JTokenType.Array:
                var array = (JArray)token;
                if (array.Count == 0)
                    return "List<object>";

                string elementType = GetCSharpType(array[0]);
                if (elementType.Contains("<") || elementType.Contains("["))
                    return $"List<object>";

                return $"List<{elementType}>";
            case JTokenType.Object:
                // 对于对象，我们将使用属性名作为类名
                // 检查父节点是否为JProperty
                if (token.Parent is JProperty parentProperty)
                {
                    return parentProperty.Name;
                }
                // 如果没有父属性（例如根对象），则返回通用对象类型
                return "object";
            case JTokenType.Null:
                return "object";
            default:
                return "object";
        }
    }

    //首字母大写
    public string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        char firstChar = char.ToUpper(input[0]);
        if (input.Length == 1)
            return firstChar.ToString();

        return firstChar + input.Substring(1);
    }

}
