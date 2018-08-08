using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Acb.WebApi.Test.Hubs
{
    public class MessageHub : Hub
    {
        /// <summary> 发送弹幕方法 </summary>
        public const string SendMethod = "send";

        /// <summary>  删除弹幕 </summary>
        public const string DeleteMethod = "delete";

        /// <summary> 签到方法 </summary>
        public const string SignMethod = "signin";

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public Task SignIn(string str)
        {
            return Clients.All.SendAsync(SignMethod, str);
        }

        public Task SendBarrage(string str)
        {
            return Clients.All.SendAsync(SendMethod, str);
        }
    }
}
