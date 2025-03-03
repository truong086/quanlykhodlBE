﻿using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.Service;
using quanlykhodl.ViewModel;
using System.Runtime.InteropServices;
using Vonage.Users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace quanlykhodl.ChatHub
{
    public class NotificationHub : Hub
    {
        private readonly DBContext _dbcontext;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly Cloud _cloud;
        private KiemTraBase64 _kiemtrabase64;
        public NotificationHub(DBContext dbcontext, IUserService userService,
            IMapper mapper, IOptions<Cloud> cloud, KiemTraBase64 kiemtrabase64)
        {
            _mapper = mapper;
            _dbcontext = dbcontext;
            _userService = userService;
            _cloud = cloud.Value;
            _kiemtrabase64 = kiemtrabase64;
        }
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task sendMessageToUser(int receiverUserId, string message, string image)
        {
            var linkImage = string.Empty;
            var publicId = string.Empty;

            var checkUser = _dbcontext.onlineusersuser.Where(x => x.account_id == receiverUserId && x.isonline).FirstOrDefault();
            var checkUserSend = _dbcontext.accounts.Where(x => x.id == int.Parse(Context.UserIdentifier) && !x.deleted).FirstOrDefault();
            var checkUserReceiver = _dbcontext.accounts.Where(x => x.id == receiverUserId && !x.deleted).FirstOrDefault();

            if (image != null)
            {
                if (!_kiemtrabase64.kiemtra(image))
                {
                    uploadCloud.CloudInaryAccount(image, TokenViewModel.MESSAGE + Context.UserIdentifier + receiverUserId.ToString(), _cloud);
                }
                else
                {
                    var chuyenDoi = chuyenDoiIFromFileProduct(image, receiverUserId);
                    uploadCloud.CloudInaryIFromAccount(chuyenDoi, TokenViewModel.MESSAGE + Context.UserIdentifier + receiverUserId.ToString(), _cloud);
                }

                linkImage = uploadCloud.Link;
                publicId = uploadCloud.publicId;
            }

            if (checkUser != null)
            {
                if(!string.IsNullOrEmpty(linkImage) && !string.IsNullOrEmpty(publicId))
                    AddDataBase(int.Parse(Context.UserIdentifier), receiverUserId, message, true, linkImage, publicId);
                else
                    AddDataBase(int.Parse(Context.UserIdentifier), receiverUserId, message, true);

            }
            else
            {
                if (!string.IsNullOrEmpty(linkImage) && !string.IsNullOrEmpty(publicId))
                    AddDataBase(int.Parse(Context.UserIdentifier), receiverUserId, message, false, linkImage, publicId);
                else AddDataBase(int.Parse(Context.UserIdentifier), receiverUserId, message, false);
            }

            // Gửi cho người nhận
            await Clients.Client(checkUser.connectionid).SendAsync("ReceiveMessage", new
            {
                idUser2 = int.Parse(Context.UserIdentifier), // Id người gửi
                image_user2 = checkUserSend.image,
                name_user2 = checkUserSend.username,
                idUser1 = receiverUserId,
                image_user1 = checkUserReceiver.image,
                name_user1 = checkUserReceiver.username,
                message = message,
                CreateAt = DateTimeOffset.UtcNow,
                imagedata = image == null ? null : uploadCloud.Link
            });

            // Gửi lại cho bản thân để hiển thị
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", new
            {
                idUser2 = int.Parse(Context.UserIdentifier), // Id người gửi
                image_user2 = checkUserSend.image,
                idUser1 = receiverUserId,
                image_user1 = checkUserReceiver.image,
                name_user1 = checkUserReceiver.username,
                message = message,
                CreateAt = DateTime.UtcNow,
                imagedata = image == null ? null : uploadCloud.Link
            });
        }

        private IFormFile chuyenDoiIFromFileProduct(string data, int id)
        {
            var chuyenDoiStringBase64 = new ChuyenFile();
            var fileName = "Message" + id;
            return chuyenDoiStringBase64.chuyendoi(data, fileName);
        }
        private void AddDataBase(int sendId, int receiverId, string message, bool check, string? link = null, string? publicId = null)
        {
            var data = new Message
            {
                receiverid = receiverId,
                senderid = sendId,
                content = message,
                isread = check,
                image = link != null ? link : null,
                publicid = publicId != null ? publicId : null
            };

            _dbcontext.messages.Add(data);
            _dbcontext.SaveChanges();
        }

        // Hàm này nghĩa là khi chúng ta vừa Login xong có nghĩa là chúng ta chuẩn bị Connect thì sẽ gọi hàm này "OnConnectedAsync()" (Hàm này Client sẽ gọi)
        public override async Task OnConnectedAsync()
        {
            // Lấy thông tin từ query string
            //string name = Context.GetHttpContext()?.Request.Query["name"];
            //string avatarUrl = Context.GetHttpContext()?.Request.Query["avatarUrl"];

            //// Thêm người dùng vào danh sách
            //_onlineUserService.AddUser(Context.connectionid, 1, name ?? "Guest", avatarUrl ?? "/images/default-avatar.png");

            // Gửi danh sách người dùng mới cho tất cả client
            //await Clients.All.SendAsync("UpdateOnlineUsers", _onlineUserService.GetOnlineUsers());

            var userId = Context.UserIdentifier; // Lấy ra "Sub" trên mã JWT
            //var userId = _userService.name();
            if (userId != null) 
            {
                var checkAccount = _dbcontext.accounts.Where(x => x.id == int.Parse(userId) && !x.deleted).FirstOrDefault();
                if(checkAccount != null)
                {
                    var checkOnline = _dbcontext.onlineusersuser.Where(x => x.account_id == checkAccount.id).OrderByDescending(c => c.createdat).FirstOrDefault();
                    if(checkOnline == null || !checkOnline.isonline)
                    {
                        var userOnline = new OnlineUsers
                        {
                            account = checkAccount,
                            account_id = checkAccount.id,
                            connectionid = Context.ConnectionId,
                            isonline = true
                        };
                        _dbcontext.onlineusersuser.Add(userOnline);
                        _dbcontext.SaveChanges();

                    }
                    
                }
            }

            var dataOnline = _dbcontext.onlineusersuser.Where(x => x.isonline).ToList();


            await Clients.All.SendAsync("UserData", AccountOnline.GetAll(dataOnline, _mapper, _dbcontext));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Xóa người dùng khỏi danh sách
            //_onlineUserService.RemoveUser(Context.connectionid);

            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                var onlineUserData = _dbcontext.onlineusersuser.Where(x => x.account_id == int.Parse(userId) && x.isonline == true).ToList();
                if (onlineUserData.Any())
                {
                    foreach(var item in onlineUserData)
                    {
                        item.isonline = false;
                        item.updatedat = DateTimeOffset.UtcNow;

                        _dbcontext.onlineusersuser.Update(item);
                        _dbcontext.SaveChanges();
                    }
                }
            }
            var dataOnline = _dbcontext.onlineusersuser.Where(x => x.isonline).ToList();
            // Gửi danh sách người dùng mới cho tất cả client
            //await Clients.All.SendAsync("UpdateOnlineUsers", _onlineUserService.GetOnlineUsers());
            //await Clients.All.SendAsync("online", dataOnline);
            //await Clients.Caller.SendAsync("onError", "OnDisconnected:");

            await Clients.All.SendAsync("logoutData", AccountOnline.GetAll(dataOnline, _mapper, _dbcontext));
            await base.OnDisconnectedAsync(exception);
            
        }
    }
}