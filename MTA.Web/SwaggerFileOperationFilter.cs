using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class FileUploadOperation : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileUploadMime = "multipart/form-data";

        if (operation.RequestBody == null || !operation.RequestBody.Content.ContainsKey(fileUploadMime))
            return;

        var parameters = context.MethodInfo.GetParameters();

        var properties = new Dictionary<string, OpenApiSchema>();

        foreach (var param in parameters)
        {
            if (param.ParameterType == typeof(Microsoft.AspNetCore.Http.IFormFile))
            {
                properties.Add(param.Name, new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                });
            }
            else if (!param.ParameterType.IsPrimitive && param.ParameterType != typeof(string))
            {
                // Add properties of DTO
                foreach (var prop in param.ParameterType.GetProperties())
                {
                    var schema = context.SchemaGenerator.GenerateSchema(prop.PropertyType, context.SchemaRepository);
                    properties.Add(prop.Name, schema);
                }
            }
            else
            {
                // simple parameters (string, int, ...)
                properties.Add(param.Name, new OpenApiSchema { Type = param.ParameterType.Name.ToLower() });
            }
        }

        operation.RequestBody.Content[fileUploadMime].Schema = new OpenApiSchema
        {
            Type = "object",
            Properties = properties
        };
    }
}
