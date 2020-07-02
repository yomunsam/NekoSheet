using NekoSheet.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoSheet
{
    /// <summary>
    /// 字段信息
    /// </summary>
    internal class SheetFieldInfo
    {
        public int Index { get; set; }



        public string Name { get; set; }
        public FieldType Type { get; set; } = FieldType.String;
        public FieldType ArrayType { get; set; } = FieldType.String; //if is array.

        public bool Key { get; set; } = false; //主键字段

        public bool Required { get; set; } = false;

    }
}
