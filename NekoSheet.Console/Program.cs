using System;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace NekoSheet
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                //输入路径
                new Option<string>("--path",
                ()=>{ return Directory.GetCurrentDirectory(); }, //默认值
                "xlsx file path or directory path"),

                //输出路径
                new Option<string>("--output", "Output path."),

                //Json 格式
                new Option<bool>("--indented", ()=> false, "得到带有格式的那种Json"),

                new Option<char>("--commentChar", ()=>'@' , "文件名注释符号，在自动生成文件名时，如果该项不为空，则忽略表格文件中注释符号及其后面的内容.\n如\"dict_props@道具定义表.xlsx\"自动生成的json名为\"dict_props.json\""),
                new Option<string>("--rootNode", ()=>"data" , "生成的Json文件是个数组嘛，这个数组的\"key\"或者叫\"property\"的名字"),

            };

            rootCommand.Handler = CommandHandler.Create<string,string,bool,char,string>((path,output, indented, commentChar,rootNode) =>
            {
                var filePaths = GetSheetFileNames(path);
                foreach(var file in filePaths)
                {
                    if(!File.Exists(file))
                    {
                        Console.WriteLine("File not found:" + file);
                        return;
                    }

                    var nekosheet = new NekoSheet(file);
                    nekosheet.JsonRootNodeName = rootNode;
                    var output_path = GetOutputName(output, file, commentChar);
                    nekosheet.SaveJson(0, output_path, indented);

                    Console.WriteLine("Write Json File：" + output_path);
                }


                Console.WriteLine("\nFinish...");
            });


            return rootCommand.InvokeAsync(args).Result;
        }


        static string[] GetSheetFileNames(string path)
        {
            if (Directory.Exists(path))
            {
                //给的文件是个目录
                var files = Directory.GetFiles(path, "*.xlsx", SearchOption.TopDirectoryOnly);
                return files;
            }
            else
            {
                return new string[] { path };
            }
        }

        /// <summary>
        /// 获取输出路径
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="inputPath"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static string GetOutputName(string outputPath,string filePath, char commentChar)
        {
            if (Directory.Exists(outputPath))
            {
                //给定的路径是个文件夹,也就是说要自动生成文件名
                string file_name = Path.GetFileNameWithoutExtension(filePath);
                int _comment = file_name.IndexOf(commentChar);
                if(_comment > 0)
                {
                    file_name = file_name.Substring(0, _comment);
                }
                return Path.Combine(outputPath, file_name + ".json");
            }
            else
                return outputPath;
        }


    }



}
