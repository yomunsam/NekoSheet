using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Nekonya;
using NekoSheet.Utils;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace NekoSheet;

public class NekoSheet : System.IDisposable
{
    public char SplitChar { get; set; } = ',';
    public string? JsonRootNodeName { get; set; } = string.Empty;

    /// <summary>
    /// 在无法获取到数据时插入默认值
    /// </summary>
    public bool InsertDefaultValueIfNoData = true;

    private XSSFWorkbook m_workbook;
    private FileStream m_workbook_fileStream;

    public NekoSheet(string path)
    {
        m_workbook_fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        m_workbook = new XSSFWorkbook(m_workbook_fileStream);
    }


    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="output_paths">输出路径：如果指定了</param>
    //public string[] ConvertToJson()
    //{
        
    //}

    public void SaveJson(int sheetIndex,  string savePath, bool indented = false, Encoding encoding = null)
    {
        var json_str = this.CovertToJson(sheetIndex, indented);
        string path_dir = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(path_dir))
            Directory.CreateDirectory(path_dir);
        if (encoding != null)
            File.WriteAllText(savePath, json_str, encoding);
        else
            File.WriteAllText(savePath, json_str, Encoding.UTF8);
    }

    public string CovertToJson(int sheetIndex, bool Indented = false)
    {
        var sheet = m_workbook.GetSheetAt(sheetIndex);

        //获取fieldInfo
        var fieldInfos = GetFieldInfos(sheet);
        if (fieldInfos.Count < 1) return string.Empty;

        //遍历数据表
        var arrayObj = ReadSheetData(fieldInfos, sheet);
        JsonNode mainNode = this.JsonRootNodeName.IsNullOrEmpty() ? arrayObj : new JsonObject()
        {
            [this.JsonRootNodeName] = arrayObj
        };
        var jsonOpt = new JsonSerializerOptions {
            WriteIndented = Indented,
        };
        return mainNode.ToJsonString(jsonOpt);
    }


    private JsonArray ReadSheetData(IList<SheetFieldInfo> fields, ISheet sheet)
    {
        var mainArray = new JsonArray();
        //从第四行开始读
        for(var i = 4; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            if (row == null)
                break;

            var rowJObj = new JsonObject();

            bool break_flag = false;
            foreach(var field in fields)
            {
                var cell = row.GetCell(field.Index);
                if(cell == null)
                {
                    if (field.Key) //key空了，读到这里就停止
                    {
                        break_flag = true;
                        break;
                    }
                    else
                    {   
                        //非key的是数据空了，使用默认值或者跳过
                        if (InsertDefaultValueIfNoData)
                            NekoUtil.AddDefaultValue(in rowJObj, field.Name, field.Type, field.ArrayType);
                        else
                            continue;
                    }
                }
                else
                {
                    NekoUtil.AddJsonProperty(in rowJObj, field.Name, cell.ToString(), field.Type, field.ArrayType, SplitChar);
                }
            }

            if (break_flag)
                break;

            mainArray.Add(rowJObj);
        }

        return mainArray;
    }


    private List<SheetFieldInfo>? GetFieldInfos(ISheet sheet)
    {
        List<SheetFieldInfo> infos = [];

        var field_row = sheet.GetRow(2);
        var desc_row = sheet.GetRow(3); //辅助描述行

        if (field_row == null)
            return null;

        //遍历row
        for(var i = 0; i < field_row.Cells.Count; i++)
        {
            var cell = field_row.Cells[i];
            if (cell == null)
                continue;
            var cell_str = cell.ToString();
            if (cell_str.IsNullOrEmpty())
                continue;
            if(NekoUtil.ParseFieldStr(cell_str  ,out string name ,out var Type , out var subType))
            {
                var info = new SheetFieldInfo()
                {
                    Index = i,
                    Name = name,
                    Type = Type,
                    ArrayType = subType
                };

                //辅助描述行

                if (desc_row == null)
                {
                    infos.Add(info);
                    continue;
                }
                
                var d_cell = desc_row.GetCell(i);
                if(d_cell == null)
                {
                    infos.Add(info);
                    continue;
                }

                var d_cell_str = d_cell.ToString();
                NekoUtil.ParseFieldDesc(d_cell_str, out bool key, out bool required);
                info.Key = key;
                info.Required = required;
                infos.Add(info);
            }

            
        }

        //如果没有声明主键，则第一个为主键
        if(infos.Count > 0)
        {
            if(!infos.Any(f => f.Key))
            {
                infos.First().Key = true;
            }
        }

        return infos;
    }

    public void Dispose()
    {
        this.m_workbook_fileStream?.Dispose();
    }
}
