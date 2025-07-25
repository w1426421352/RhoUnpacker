// 文件名: RhoUnpacker/Program.cs

using System;
using System.IO;
using System.Collections.Generic;
using KartLibrary.File; // 引用我们核心库
using KartLibrary.Consts;

namespace RhoUnpacker
{
    static class Program
    {
        static void Main(string[] args)
        {
            // 1. 解析命令行参数
            if (args.Length != 2)
            {
                Console.WriteLine("Error: Invalid arguments.");
                Console.WriteLine("Usage: RhoUnpacker.exe <inputFile.rho> <outputDirectory>");
                return; // 参数数量不正确则退出
            }

            string inputFile = args[0];
            string outputDir = args[1];

            Console.WriteLine($"Input file: {inputFile}");
            Console.WriteLine($"Output directory: {outputDir}");

            // 2. 检查输入文件是否存在
            if (!System.IO.File.Exists(inputFile))
            {
                Console.WriteLine($"Error: Input file not found at '{inputFile}'");
                return;
            }

            // 3. 执行解包逻辑
            try
            {
                // 创建输出目录（如果不存在）
                Directory.CreateDirectory(outputDir);

                // 使用KartLibrary中的逻辑来解包。
                // 默认使用国服(CN)的CountryCode，因为它不影响解包逻辑本身。
                PackFolderManager packManager = new PackFolderManager();
                packManager.OpenSingleFile(CountryCode.CN, inputFile);

                // 递归地将所有文件写入磁盘
                ExtractFolder(packManager.GetRootFolder(), outputDir);

                Console.WriteLine("Unpacking completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during unpacking: {ex.Message}");
                Console.WriteLine(ex.StackTrace); // 打印详细错误以供调试
            }
        }

        // 递归提取文件的辅助函数
        private static void ExtractFolder(PackFolderInfo folder, string currentPath)
        {
            // 在当前路径下创建这个文件夹
            string newPath = Path.Combine(currentPath, folder.FolderName);
            Directory.CreateDirectory(newPath);

            // 提取该文件夹下的所有文件
            foreach (var fileInfo in folder.GetFilesInfo())
            {
                string filePath = Path.Combine(newPath, fileInfo.FileName);
                try
                {
                    byte[] data = fileInfo.GetData();
                    if (data != null)
                    {
                        System.IO.File.WriteAllBytes(filePath, data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to extract file: {fileInfo.FullName}. Error: {ex.Message}");
                }
            }

            // 递归处理所有子文件夹
            foreach (var subFolder in folder.GetFoldersInfo())
            {
                ExtractFolder(subFolder, newPath);
            }
        }
    }
}