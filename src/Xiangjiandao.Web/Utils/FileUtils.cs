using System.Reflection;
using System.Text.RegularExpressions;
using NetCorePal.Extensions.Primitives;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Xiangjiandao.Web.Utils;

/// <summary>
/// 文本工具类
/// </summary>
public class FileUtils
{

    /// <summary>
    /// 文件转字符串
    /// </summary>
    public static string FileToStr(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new KnownException("filePath is not exits");
        }

        // 获取当前程序集
        Assembly assembly = Assembly.GetExecutingAssembly();
        // 通过资源名称打开资源文件流
        using (Stream? stream = assembly.GetManifestResourceStream(filePath))
        {
            if (stream != null)
            {
                // 使用 StreamReader 读取资源文件内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContent = reader.ReadToEnd();
                    return fileContent;
                }
            }
            else
            {
                Console.WriteLine("无法找到资源文件");
                throw new KnownException("无法找到资源文件");
            }
        }
    }

    /// <summary>
    /// 文件转成流
    /// </summary>
    public static Stream FileToStream(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new KnownException("filePath is not exits");
        }

        // 获取当前程序集
        Assembly assembly = Assembly.GetExecutingAssembly();
        // 通过资源名称打开资源文件流
        using (Stream? stream = assembly.GetManifestResourceStream(filePath))
        {
            if (stream != null)
            {
                return stream;
            }
            else
            {
                Console.WriteLine("无法找到资源文件");
                throw new KnownException("无法找到资源文件");
            }
        }
    }

    /// <summary>
    /// 流转用户信息列表数据结构
    /// </summary>
    public static List<string> StreamUserPhoneOrEmail(Stream stream)
    {
        List<string> userPhoneOrEmailList = new List<string>();

        try
        {
            using (var workbook = new XSSFWorkbook(stream))
            {
                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet == null || sheet.LastRowNum < 1)
                {
                    throw new KnownException("上传的Excel文件为空或格式不正确");
                }
                    
                for (int row = 1; row <= sheet.LastRowNum; row++) // 注意：从第二行开始，忽略标题行
                {
                    IRow excelRow = sheet.GetRow(row);

                    if (excelRow == null)
                    {
                        continue;
                    }

                    if (excelRow.GetCell(0) == null)
                    {
                        continue;
                    }

                    var data = excelRow.GetCell(0).ToString()!;
                    if (string.IsNullOrWhiteSpace(data))
                    {
                        continue;
                    }
                    
                    if (!IsPhoneNumber(data) &&  !IsEmail(data))
                    {
                        throw new KnownException("第" + (row + 1) + "行的数据不是有效的手机号或邮箱");
                    }
                    
                    userPhoneOrEmailList.Add(data);
                    
                }
            }
            
            return userPhoneOrEmailList;
        } 
        catch (KnownException ex)
        {
            throw;
        } catch (Exception ex)
        {
            throw new KnownException("请上传正确格式的文件");
        } 
    }
    
    public static Stream GetStream(string path, string fileId)
    {
        var pattern = fileId + "-*";
        var filePath = Directory.GetFiles(path, pattern).FirstOrDefault();
        if (string.IsNullOrEmpty(filePath))
        {
            throw new KnownException("file is not exits");
        }
        return File.OpenRead(filePath);
    }
    private static bool IsPhoneNumber(string number)
    {
        // 简单的中国大陆手机号匹配（11位数字，以13/14/15/17/18/19开头）
        string pattern = @"^1[3-9]\d{9}$";
        return Regex.IsMatch(number, pattern);
    }

    private static bool IsEmail(string email)
    {
        // 常规邮箱格式验证
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }
}
