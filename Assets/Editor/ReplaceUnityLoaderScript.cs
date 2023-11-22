using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class ReplaceUnityLoaderScript : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.WebGL)
        {
            string pathToBuiltProject = report.summary.outputPath;
            string customLoaderPath = "Assets/WebGLTemplates/InterstellarEllaTemplate/TemplateData/UnityLoader.js"; // Update this path
            string targetLoaderPath = Path.Combine(pathToBuiltProject, "Build/UnityLoader.js");

            ReplaceUnityLoader(customLoaderPath, targetLoaderPath);
        }
    }

    private void ReplaceUnityLoader(string sourceFilePath, string targetFilePath)
    {
        if (File.Exists(targetFilePath))
        {
            File.Delete(targetFilePath);
        }
        File.Copy(sourceFilePath, targetFilePath);
    }
}
