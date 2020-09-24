using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using COLID.AppDataService.Common.Utilities;
using Microsoft.AspNetCore.Mvc.Formatters;

public class TextPlainInputFormatter : InputFormatter
{
    private const string ContentType = MediaTypeNames.Text.Plain;

    public TextPlainInputFormatter()
    {
        SupportedMediaTypes.Add(ContentType);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var request = context.HttpContext.Request;
        using (var reader = new StreamReader(request.Body))
        {
            var content = await reader.ReadToEndAsync();
            return await InputFormatterResult.SuccessAsync(content);
        }
    }

    public override bool CanRead(InputFormatterContext context)
    {
        Guard.IsNotNull(context.HttpContext.Request.ContentType);
        var contentType = context.HttpContext.Request.ContentType;
        return contentType.StartsWith(ContentType);
    }
}
