﻿using MessageApp103.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace MessageApp103.Hubs
{
    public class ChatHub:Hub
    { 
        
        private readonly UserManager<AppUser> _userManager;
        public ChatHub( UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
       
            _userManager = userManager;
           
        }
        public async Task SendMessage(string user, string message)
        {
             await Clients.All.SendAsync("RecieveMessage",user,message,DateTime.Now.ToString("dddd, dd MMMM yyyy"));
        }

        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                user.ConnectionId=Context.ConnectionId;
               var result = _userManager.UpdateAsync(user).Result;
                Clients.All.SendAsync("Connect", user.Id);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                user.ConnectionId=null;
                Clients.All.SendAsync("DisConnect", user.Id);
                var result = _userManager.UpdateAsync(user).Result;
            }
            return base.OnDisconnectedAsync(exception);
        }



    }
}
