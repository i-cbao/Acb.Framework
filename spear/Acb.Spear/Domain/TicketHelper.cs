using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Timing;
using Acb.WebApi;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Acb.Spear.Business.Domain.Repositories;
using Acb.Spear.Contracts.Dtos;

namespace Acb.Spear.Domain
{
    /// <summary> 项目凭证 </summary>
    public class SpearTicket : ClientTicket
    {
        public Guid Id { get; set; }
        public string Nick { get; set; }

        public string Avatar { get; set; }
        /// <summary> 项目编码 </summary>
        public string Code { get; set; }
    }

    public static class TicketHelper
    {
        private const string ProjectCodeKey = "project";
        private const string ProjectCacheKey = "_req_project";
        private const string TicketCacheKey = "_req_ticket";

        /// <summary> 验证令牌 </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static SpearTicket GetTicket(this HttpContext context, string scheme = "acb")
        {
            try
            {
                if (context.Items.TryGetValue(TicketCacheKey, out var t) && t != null)
                    return t as SpearTicket;
                if (!context.Request.Headers.TryGetValue("Authorization", out var authorize) ||
                    string.IsNullOrWhiteSpace(authorize))
                    return null;
                var arr = authorize.ToString()?.Split(' ');
                if (arr == null || arr.Length != 2 || arr[0] != scheme)
                    return null;
                var ticket = arr[1];
                var client = ticket.Client<SpearTicket>();
                if (client.ExpiredTime.HasValue && client.ExpiredTime.Value < Clock.Now)
                    return null;
                context.Items.TryAdd(TicketCacheKey, client);
                return client;
            }
            catch
            {
                return null;
            }
        }

        public static string GetProjectCode(this HttpContext context)
        {
            var ticket = context.GetTicket();
            string code;
            if (ticket != null)
            {
                code = ticket.Code;
            }
            else
            {
                code = ProjectCodeKey.QueryOrForm(string.Empty);
                if (!string.IsNullOrWhiteSpace(code))
                    return code;
                if (context.Request.Headers.TryGetValue(ProjectCodeKey, out var dcode))
                    code = dcode;
            }

            return code;
        }

        public static ProjectDto GetProject(this HttpContext context)
        {
            if (context.Items.TryGetValue(ProjectCacheKey, out var project) && project != null)
                return project as ProjectDto;
            var code = context.GetProjectCode();
            if (string.IsNullOrWhiteSpace(code))
                return null;
            project = CurrentIocManager.Resolve<ProjectRepository>().QueryByCodeAsync(code).GetAwaiter().GetResult();
            context.Items.TryAdd(ProjectCacheKey, project);
            return (ProjectDto)project;
        }
    }
}
