using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AdvancedJsonSerializer
{
    /// <summary>
    /// 将格式化的JSON字符串转换为美化的格式
    /// </summary>
    public static string PrettyPrintJson(string json)
    {
        var sb = new StringBuilder();
        int indentLevel = 0;
        bool inString = false;
        bool escapeNext = false;

        foreach (char c in json)
        {
            if (escapeNext)
            {
                sb.Append(c);
                escapeNext = false;
                continue;
            }

            if (c == '\\' && inString)
            {
                sb.Append(c);
                escapeNext = true;
                continue;
            }

            if (c == '"' && !escapeNext)
            {
                inString = !inString;
                sb.Append(c);
                continue;
            }

            if (inString)
            {
                sb.Append(c);
                continue;
            }

            switch (c)
            {
                case '{':
                case '[':
                    sb.Append(c);
                    indentLevel++;
                    sb.Append('\n');
                    sb.Append(new string('\t', indentLevel));
                    break;
                case '}':
                case ']':
                    indentLevel--;
                    sb.Append('\n');
                    sb.Append(new string('\t', indentLevel));
                    sb.Append(c);
                    break;
                case ',':
                    sb.Append(c);
                    sb.Append('\n');
                    sb.Append(new string('\t', indentLevel));
                    break;
                case ':':
                    sb.Append(c);
                    sb.Append(' ');
                    break;
                default:
                    if (!char.IsWhiteSpace(c))
                    {
                        sb.Append(c);
                    }
                    break;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 压缩JSON，移除所有空白字符
    /// </summary>
    public static string CompressJson(string json)
    {
        var sb = new StringBuilder();
        bool inString = false;
        bool escapeNext = false;

        foreach (char c in json)
        {
            if (escapeNext)
            {
                sb.Append(c);
                escapeNext = false;
                continue;
            }

            if (c == '\\' && inString)
            {
                sb.Append(c);
                escapeNext = true;
                continue;
            }

            if (c == '"')
            {
                inString = !inString;
                sb.Append(c);
                continue;
            }

            if (inString || !char.IsWhiteSpace(c))
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 验证JSON格式是否有效
    /// </summary>
    public static bool ValidateJson(string json)
    {
        if (string.IsNullOrEmpty(json))
            return false;

        json = json.Trim();

        if ((json.StartsWith("{") && json.EndsWith("}")) ||
            (json.StartsWith("[") && json.EndsWith("]")))
        {
            try
            {
                JsonUtility.FromJson<Dictionary<string, object>>(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// 合并两个JSON对象
    /// </summary>
    public static string MergeJson(string json1, string json2)
    {
        try
        {
            // 简单实现：将两个JSON字符串合并
            string merged = json1.TrimEnd('}') + "," + json2.TrimStart('{');
            return merged;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to merge JSON: " + e.Message);
            return json1;
        }
    }

    /// <summary>
    /// 从JSON中提取特定的值
    /// </summary>
    public static string ExtractValue(string json, string key)
    {
        try
        {
            // 简单的键值查找
            string searchPattern = "\"" + key + "\":";
            int startIndex = json.IndexOf(searchPattern);

            if (startIndex == -1)
                return null;

            startIndex += searchPattern.Length;
            int endIndex = json.IndexOf(',', startIndex);

            if (endIndex == -1)
                endIndex = json.IndexOf('}', startIndex);

            if (endIndex == -1)
                endIndex = json.Length;

            return json.Substring(startIndex, endIndex - startIndex).Trim();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to extract value: " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// 将JSON转换为CSV格式（用于简单的扁平JSON）
    /// </summary>
    public static string JsonToCsv(string json)
    {
        try
        {
            var lines = new List<string>();
            
            // 简单实现：提取所有键值对
            json = json.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "");
            var pairs = json.Split(',');

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    lines.Add(keyValue[0].Trim().Trim('"') + "," + keyValue[1].Trim().Trim('"'));
                }
            }

            return string.Join("\n", lines);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to convert JSON to CSV: " + e.Message);
            return "";
        }
    }
}

/// <summary>
/// JSON差异比较工具
/// </summary>
public class JsonDiffTool
{
    public class DiffResult
    {
        public string key;
        public string oldValue;
        public string newValue;
        public DiffType type;
    }

    public enum DiffType
    {
        Added,
        Removed,
        Modified
    }

    public static List<DiffResult> ComparJson(string json1, string json2)
    {
        var results = new List<DiffResult>();
        
        try
        {
            // 简单实现：比较两个JSON字符串的内容
            var pairs1 = ExtractKeyValuePairs(json1);
            var pairs2 = ExtractKeyValuePairs(json2);

            // 找出被移除和修改的项
            foreach (var pair in pairs1)
            {
                if (!pairs2.ContainsKey(pair.Key))
                {
                    results.Add(new DiffResult
                    {
                        key = pair.Key,
                        oldValue = pair.Value,
                        type = DiffType.Removed
                    });
                }
                else if (pairs2[pair.Key] != pair.Value)
                {
                    results.Add(new DiffResult
                    {
                        key = pair.Key,
                        oldValue = pair.Value,
                        newValue = pairs2[pair.Key],
                        type = DiffType.Modified
                    });
                }
            }

            // 找出新增的项
            foreach (var pair in pairs2)
            {
                if (!pairs1.ContainsKey(pair.Key))
                {
                    results.Add(new DiffResult
                    {
                        key = pair.Key,
                        newValue = pair.Value,
                        type = DiffType.Added
                    });
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to compare JSON: " + e.Message);
        }

        return results;
    }

    private static Dictionary<string, string> ExtractKeyValuePairs(string json)
    {
        var pairs = new Dictionary<string, string>();

        // 移除括号和空白
        json = json.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Trim();
        
        var elements = json.Split(',');

        foreach (var element in elements)
        {
            var keyValue = element.Split(':');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim().Trim('"');
                string value = keyValue[1].Trim().Trim('"');
                pairs[key] = value;
            }
        }

        return pairs;
    }
}
