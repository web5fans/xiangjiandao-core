using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.Enums;

namespace Xiangjiandao.Web.Endpoints.File;

/// <summary>
/// 下载文件
/// </summary>
public class FileDownloadEndpoint(
    ILogger<FileDownloadEndpoint> logger) : Endpoint<FileDownloadRequest, IActionResult>
{
    public override void Configure()
    {
        Get("/api/v1/file/download");
        AllowAnonymous();
        Description(x => x.WithTags("File"));
        ResponseCache(7 * 24 * 60 * 60, ResponseCacheLocation.Any, false, null,
            ["fileId", "fileType"]); //cache for 60 seconds
    }

    public override async Task HandleAsync(FileDownloadRequest request, CancellationToken cancellationToken)
    {
        var fileId = request.FileId;
        var fileType = request.FileType;
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var storagePath = fileType switch
        {
            FileType.Picture => Path.Combine(basePath, "Data/Picture/"),
            FileType.File => Path.Combine(basePath, "Data/File/"),
            _ => throw new KnownException("不支持的文件类型")
        };

        if (!Directory.Exists(storagePath))
        {
            return;
        }

        var pattern = fileId + "-*";
        var filePath = Directory.GetFiles(storagePath, pattern).FirstOrDefault();
        if (filePath == null)
        {
            return;
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);

        if (request.AutoDownload)
        {
            await SendBytesAsync(
                bytes: fileBytes,
                fileName: Path.GetFileName(filePath),
                cancellation: cancellationToken
            );
        }
        else
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var contentType);
            await SendBytesAsync(
                bytes: fileBytes,
                contentType: contentType ?? "application/octet-stream",
                cancellation: cancellationToken
            );
        }
    }
}

/// <summary>
/// FileDownloadRequest
/// </summary>
public record FileDownloadRequest
{
    /// <summary>
    /// 文件Id
    /// </summary>
    [BindFrom("fileId")]
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// 文件类型
    /// </summary>
    [BindFrom("fileType")]
    public FileType FileType { get; set; } = FileType.File;

    /// <summary>
    /// 是否自动下载
    /// </summary>
    [BindFrom("autoDownload")]
    public bool AutoDownload { get; set; } = true;
}