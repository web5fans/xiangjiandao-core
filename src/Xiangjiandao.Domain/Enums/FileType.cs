using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.Resources;

namespace Xiangjiandao.Domain.Enums;

/// <summary>
/// 文件类型枚举 1-图片 2-文件
/// </summary>
public enum FileType
{
    /// <summary>
    /// 图片
    /// </summary>
    Picture = 1,
    
    /// <summary>
    /// 文件 PDF/TXT等
    /// </summary>
    File = 2,
}

public static class FileTypeExtension
{
    public static string GetDesc(this FileType fileType)
    {
        return fileType switch
        {
            FileType.Picture => "图片",
            FileType.File => "文件",
            _ => throw new KnownException("不支持的文件类型")
        };
    }
    
    public static int GetCode(this FileType fileType)
    {
        return fileType switch
        {
            FileType.Picture => 1,
            FileType.File => 2,
            _ => throw new KnownException("不支持的文件类型")
        };
    }
}
