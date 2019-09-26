
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Acb.WebApi.Filters
{
    ///// <summary> Swagger 上传文件过滤器 </summary>
    //public class SwaggerFileUploadFilter : IOperationFilter
    //{
    //    /// <summary>
    //    /// 应用过滤器
    //    /// </summary>
    //    /// <param name="operation"></param>
    //    /// <param name="context"></param>
    //    public void Apply(Operation operation, OperationFilterContext context)
    //    {
    //        #region 文件上传处理

    //        if (!context.ApiDescription.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
    //            !context.ApiDescription.HttpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase))
    //        {
    //            return;
    //        }

    //        var parameters = context.ApiDescription.ActionDescriptor.Parameters;
    //        var fileParameters = parameters.Where(n => n.ParameterType == typeof(IFormFile)).ToList();

    //        if (fileParameters.Count < 0)
    //        {
    //            return;
    //        }

    //        operation.Consumes.Add("multipart/form-data");
    //        operation.Parameters ??= new List<IParameter>();
    //        var fileParams = new[] { "ContentType", "ContentDisposition", "Headers", "Length", "Name", "FileName" };
    //        for (var i = 0; i < operation.Parameters.Count; i++)
    //        {
    //            if (fileParams.Contains(operation.Parameters[i].Name))
    //            {
    //                operation.Parameters.RemoveAt(i);
    //                i--;
    //            }
    //        }

    //        //operation.Parameters.Clear();
    //        foreach (var fileParameter in fileParameters)
    //        {
    //            var parameter = parameters.Single(n => n.Name == fileParameter.Name);
    //            operation.Parameters.Add(new NonBodyParameter
    //            {
    //                Name = parameter.Name,
    //                In = "formData",
    //                Description = parameter.Name,
    //                Required = true,
    //                Type = "file"
    //            });
    //        }
    //        #endregion
    //    }
    //}
}
