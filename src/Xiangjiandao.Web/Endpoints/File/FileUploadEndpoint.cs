using FastEndpoints;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.File;

/// <summary>
/// 上传文件
/// </summary>
public class FileUploadEndpoint(
    ILogger<FileUploadEndpoint> logger) : Endpoint<FileUploadRequest, ResponseData<FileUploadSuccessVo>>
{
    
    public override void Configure()
    {
        Post("/api/v1/file/upload");
        AllowAnonymous();
        Description(x=>x.WithTags("File"));
        AllowFormData();
    }
    
    public override async Task HandleAsync(FileUploadRequest request, CancellationToken cancellationToken)
    {
        var file = request.FileUploadForm.File;
        var fileType = request.FileUploadForm.FileType;
        // 检查文件内容
        if (file == null || file.Length == 0)
        {
            logger.LogError("File is Empty");
            throw new KnownException("文件不能为空");
        }

        // 根据文件信息生成文件名以及相应的目录
        var fileId = Guid.NewGuid().ToString("N");
        var fileName = fileId + "-" + file.FileName;

        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var storagePath = fileType switch
        {
            FileType.Picture => Path.Combine(basePath, "Data/Picture/"),
            FileType.File => Path.Combine(basePath, "Data/File/"),
            _ => throw new KnownException("不支持的文件类型")
        };
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }

        var filePath = Path.Combine(storagePath, fileName);

        // 保存文件
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);
        var result = new FileUploadSuccessVo
        {
            FileId = fileType.GetCode() + "-" + fileName,
        };

        await SendAsync(result.AsSuccessResponseData(), cancellation: cancellationToken);
    }
}

public class FileUploadRequest
{
    /// <summary>
    /// 文件
    /// </summary>
    [FromForm]
    public FileUploadForm FileUploadForm { get; set; } = null!;
    
}

public class FileUploadForm
{
    
    /// <summary>
    /// 文件
    /// </summary>
    [FormField]
    public IFormFile File { get; set; } = null!;
    
    /// <summary>
    /// 文件类型
    /// </summary>
    [FormField]
    public FileType FileType { get; set; } = FileType.Picture; // 默认是图片类型
}