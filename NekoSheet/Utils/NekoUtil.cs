using System.Linq;
using System.Text.Json.Nodes;
using NekoSheet.Enums;

namespace NekoSheet.Utils;

public static class NekoUtil
{
    /// <summary>
    /// 取出导出文件的基本名称（不带后缀）
    /// 
    /// </summary>
    /// <returns></returns>
    public static string GetBaseFileName()
    {
        return "";
    }


    public static bool ParseFieldStr(string str, out string name, out FieldType type, out FieldType subType)
    {
        subType = FieldType.String;
        if (!str.Contains(":"))
        {
            type = FieldType.String;
            name = str;
            return true;
        }
        else
        {
            var str_arr = str.Split(":");
            if(str_arr.Length != 2)
            {
                name = default;
                type = default;
                return false;
            }

            name = str_arr[0].Trim();
            return TryGetTypeByString(str_arr[1].Trim(), out type, out subType);
        }
    }

    public static bool TryGetTypeByString(string typeName, out FieldType type, out FieldType subType)
    {
        subType = FieldType.String;
        string typeName_lower = typeName.ToLower();
        switch (typeName_lower)
        {
            case "string":
            case "s":
                type = FieldType.String;
                return true;
            case "number":
            case "n":
                type = FieldType.Number;
                return true;
            case "boolean":
            case "bool":
            case "b":
                type = FieldType.Boolean;
                return true;
            case "array":
                type = FieldType.Array;
                return true;
            case "array<s>":        //试了一下，这样就是写起来烦，但是效率比用正则加递归来得高
            case "array<string>":
                subType = FieldType.String;
                type = FieldType.Array;
                return true;
            case "array<n>":        //试了一下，这样就是写起来烦，但是效率比用正则加递归来得高
            case "array<number>":
                subType = FieldType.Number;
                type = FieldType.Array;
                return true;
            case "array<b>":        //试了一下，这样就是写起来烦，但是效率比用正则加递归来得高
            case "array<bool>":
            case "array<boolean>":
                subType = FieldType.Boolean;
                type = FieldType.Array;
                return true;
            default:
                type = default;
                return false;
        }
    }
    
    public static void ParseFieldDesc(string str, out bool key, out bool required)
    {
        key = false;
        required = false;
        if (str.Contains(','))
        {
            var str_arr = str.Split(',');
            key = IsKey(str_arr);
            required = IsRequired(str_arr);
        }
        else
        {
            var str_arr = new string[] { str };
            key = IsKey(str_arr);
            required = IsRequired(str_arr);
        }
    }

    private static bool IsKey(string[] strArr)
    {
        return strArr.Any(s => s.ToLower() == "key");
    }

    private static bool IsRequired(string[] strArr)
        => strArr.Any(s => s.ToLower() == "required");

    
    public static void AddDefaultValue(in JsonObject jsonObj, string propertyName, FieldType type, FieldType subType)
    {
        switch (type)
        {
            case FieldType.String:
                jsonObj[propertyName] = "";
                break;
            case FieldType.Number:
                jsonObj[propertyName] = 0;
                break;
            case FieldType.Boolean:
                jsonObj[propertyName] = false;
                break;
            case FieldType.Array:
                if (subType == FieldType.Number)
                    jsonObj[propertyName] = new JsonArray(0);
                else if (subType == FieldType.Boolean)
                    jsonObj[propertyName] = new JsonArray(false);
                else
                    jsonObj[propertyName] = new JsonArray("");
                break;
            default:
                jsonObj.Add(propertyName, "");
                break;
        }
    }


    public static void AddJsonProperty(in JsonObject jsonObj, string propertyName, string content, FieldType type , FieldType subType, char splitChar = ',')
    {
        switch (type)
        {
            case FieldType.Number:
                if (int.TryParse(content, out int i_value))
                    jsonObj[propertyName] = i_value;
                else if (double.TryParse(content, out double d_value))
                    jsonObj[propertyName] = d_value;
                else
                    jsonObj[propertyName] = 0;
                break;
            case FieldType.Boolean:
                if (bool.TryParse(content, out bool b_value))
                    jsonObj[propertyName] = b_value;
                else
                    jsonObj[propertyName] = false;
                break;
            case FieldType.Array:
                if (content.Contains(splitChar))
                {
                    JsonArray jarr = new JsonArray();
                    var str_arr = content.Split(splitChar);
                    foreach (var item in str_arr)
                    {
                        jarr.Add(GetValueOrDefault(item, subType));
                    }
                    jsonObj[propertyName] = jarr;
                }
                else
                {
                    if (subType == FieldType.String)
                        jsonObj[propertyName] = new JsonArray(content);
                    else if (subType == FieldType.Number)
                        jsonObj[propertyName] = new JsonArray((int)GetValueOrDefault(content, subType));
                    else if (subType == FieldType.Boolean)
                        jsonObj[propertyName] = new JsonArray((bool)GetValueOrDefault(content, subType));
                    else
                        jsonObj[propertyName] = new JsonArray((string)GetValueOrDefault(content, subType));
                }
                break;
            default:
                jsonObj[propertyName] = content;
                break;

        }
    }

    private static object GetValueOrDefault(string content, FieldType type)
    {
        switch (type)
        {
            default:
            case FieldType.String:
                return content;
            case FieldType.Number:
                if (int.TryParse(content, out int i_value))
                    return i_value;

                if (double.TryParse(content, out double d_value))
                    return d_value;
                else
                    return 0;
            case FieldType.Boolean:
                if (bool.TryParse(content, out bool b_value))
                    return b_value;
                else
                    return false;
        }
    }

}
