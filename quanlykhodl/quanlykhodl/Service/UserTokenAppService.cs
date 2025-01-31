using FirebaseAdmin.Messaging;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using Twilio.TwiML.Messaging;
using Notification = FirebaseAdmin.Messaging.Notification;
using Message = FirebaseAdmin.Messaging.Message;

namespace quanlykhodl.Service
{
    public class UserTokenAppService : IUserTokenAppService
    {
        private readonly DBContext _dBContext;
        public UserTokenAppService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public async Task<PayLoad<UserTokenAppDTO>> AddToken(UserTokenAppDTO userTokenAppDTO)
        {
            try
            {
                var data = new UserTokenApp
                {
                    Token = userTokenAppDTO.token
                };

                _dBContext.userTokenApps.Add(data);
                _dBContext.SaveChanges();

                return await Task.FromResult(PayLoad<UserTokenAppDTO>.Successfully(userTokenAppDTO));
            }catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<UserTokenAppDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<UserTokenAppDTO>> RegisterTopic(UserTokenAppDTO userTokenAppDTO)
        {
            try
            {
                if(userTokenAppDTO == null || userTokenAppDTO.token == "")
                    return await Task.FromResult(PayLoad<UserTokenAppDTO>.CreatedFail(Status.DATANULL));

                var tokenNew = new UserTokenApp
                {
                    Token = userTokenAppDTO.token
                };

                var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(new List<string>
                {userTokenAppDTO.token }, "allDevices");

                _dBContext.userTokenApps.Add(tokenNew);
                _dBContext.SaveChanges();

                return await Task.FromResult(PayLoad<UserTokenAppDTO>.Successfully(userTokenAppDTO));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<UserTokenAppDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> SendNotify()
        {
            try
            {
                /* Cách 1
                var data = _dBContext.userTokenApps.Select(x => x.Token).ToList();
                if(data.Count <= 0)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                foreach(var item in data.Chunk(500)) // Gửi cho 500 thiết bị 1 lần
                {
                    var messageSend = new MulticastMessage
                    {
                        Tokens = data,
                        Notification = new Notification()
                        {
                            Title = "💫🕳💫🕳🕳💯 Thông báo có Plan mới",
                            Body = "Có plan từ Admin vừa tạo"
                        }
                    };

                    var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(messageSend);
                    Console.WriteLine($"Data: {response} Success");
                }*/

                // Cách 2
                var message = new Message()
                {
                    Topic = "allDevices",  // Gửi đến tất cả thiết bị đăng ký topic này "allDevices"
                    Notification = new Notification()
                    {
                        Title = "❤❤❤💘💘",
                        Body = "💫💨💨💨☮🕳"
                    }
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }
    }
}
