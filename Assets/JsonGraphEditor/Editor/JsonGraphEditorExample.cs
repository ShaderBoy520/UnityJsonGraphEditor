using UnityEditor;
using UnityEngine;
using System.IO;

public class JsonGraphEditorExample
{
    [MenuItem("Tools/Json Graph Editor/Create Example JSON")]
    public static void CreateExampleJson()
    {
        string exampleJson = @"{
  ""player"": {
    ""name"": ""PlayerOne"",
    ""level"": 10,
    ""experience"": 5000,
    ""alive"": true
  },
  ""inventory"": [
    {
      ""id"": 1,
      ""name"": ""Sword"",
      ""quantity"": 1
    },
    {
      ""id"": 2,
      ""name"": ""Shield"",
      ""quantity"": 1
    }
  ],
  ""settings"": {
    ""volume"": 0.8,
    ""difficulty"": ""Normal"",
    ""fullscreen"": true
  }
}";

        string path = Path.Combine(Application.dataPath, "example.json");
        File.WriteAllText(path, exampleJson);
        
        EditorUtility.DisplayDialog("Success", "Example JSON created at:\n" + path, "OK");
        
        // 使用正确的资源路径格式
        string assetPath = "Assets/example.json";
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Json Graph Editor/Open Editor Window")]
    public static void OpenEditorWindow()
    {
        JsonGraphEditorWindow.OpenWindow();
    }
}
