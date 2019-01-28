using Acb.Core.Dependency;
using Acb.Core.Extensions;
using Acb.Core.Timing;
using Acb.Spear.Contracts;
using Acb.Spear.Contracts.Dtos;
using Acb.Spear.Contracts.Enums;
using Acb.WebApi;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Acb.Spear.Domain
{
    /// <summary> 项目凭证 </summary>
    public class SpearTicket : ClientTicket
    {
        /// <summary> 帐号Id </summary>
        public Guid Id { get; set; }

        /// <summary> 帐号昵称 </summary>
        public string Nick { get; set; }

        /// <summary> 帐号头像 </summary>
        public string Avatar { get; set; }

        /// <summary> 项目编码 </summary>
        public Guid? ProjectId { get; set; }
        /// <summary> 权限 </summary>
        public byte Role { get; set; }
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

        /// <summary>
        /// 获取项目编码
        /// 1.参数
        /// 2.header
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetProjectCode(this HttpContext context)
        {
            var code = ProjectCodeKey.QueryOrForm(string.Empty);
            if (!string.IsNullOrWhiteSpace(code))
                return code;
            if (context.Request.Headers.TryGetValue(ProjectCodeKey, out var dcode))
                code = dcode;
            return code;
        }

        /// <summary> 根据编码获取项目 </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ProjectDto GetProjectByCode(this HttpContext context)
        {
            var code = context.GetProjectCode();
            if (string.IsNullOrWhiteSpace(code))
                return null;
            var contract = CurrentIocManager.Resolve<IProjectContract>();
            return contract.DetailByCodeAsync(code).SyncRun();
        }

        /// <summary> 根据编码获取项目 </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ProjectDto GetProjectByToken(this HttpContext context)
        {
            var ticket = context.GetTicket();
            if (ticket == null || !ticket.ProjectId.HasValue)
                return null;
            var contract = CurrentIocManager.Resolve<IProjectContract>();
            return contract.DetailAsync(ticket.ProjectId.Value).SyncRun();
        }

        /// <summary> 获取项目信息 </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ProjectDto GetProject(this HttpContext context)
        {
            if (context.Items.TryGetValue(ProjectCacheKey, out var cacheProject) && cacheProject != null)
                return cacheProject as ProjectDto;
            var project = context.GetProjectByToken();
            if (project == null)
            {
                project = context.GetProjectByCode();
                if (project == null || (project.Security & SecurityEnum.Get) > 0)
                    return null;
            }
            context.Items.TryAdd(ProjectCacheKey, project);
            return project;
        }

        /// <summary> 设置项目 </summary>
        /// <param name="context"></param>
        /// <param name="project"></param>
        public static void SetProject(this HttpContext context, ProjectDto project)
        {
            context.Items.TryAdd(ProjectCacheKey, project);
        }
    }
}
