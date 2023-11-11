using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class QualityLevelEnumGenerator
{
    [MenuItem("Tools/Generate Quality Level Enum")]
    public static void GenerateQualityLevelEnum()
    {
        string filePath = Path.Combine(Application.dataPath, "QualityLevelEnum.cs");
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("public enum QualityLevel");
        stringBuilder.AppendLine("{");

        foreach (var name in QualitySettings.names)
        {
            string validEnumName = name.Replace(" ", "_").Replace("-", "_");
            stringBuilder.AppendLine($"    {validEnumName},");
        }

        stringBuilder.AppendLine("}");

        File.WriteAllText(filePath, stringBuilder.ToString());
        AssetDatabase.Refresh();
    }
}
