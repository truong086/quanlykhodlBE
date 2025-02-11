using FirebaseAdmin.Messaging;
using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using Twilio.TwiML.Messaging;
using Notification = FirebaseAdmin.Messaging.Notification;
using Message = FirebaseAdmin.Messaging.Message;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

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

                _dBContext.usertokenapps.Add(data);
                _dBContext.SaveChanges();

                return await Task.FromResult(PayLoad<UserTokenAppDTO>.Successfully(userTokenAppDTO));
            }catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<UserTokenAppDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> DeleteData()
        {
            try
            {
                var data = _dBContext.usertokenapps.ToList();

                _dBContext.usertokenapps.RemoveRange(data);
                _dBContext.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
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

                _dBContext.usertokenapps.Add(tokenNew);
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
                //Cách 1
                var data = _dBContext.usertokenapps.ToList();
                if (data.Count <= 0)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                /*foreach (var item in data.Select(x => x.Token).Chunk(500)) // Gửi cho 500 thiết bị 1 lần
                {

                    var messageSend = new MulticastMessage
                    {
                        Tokens = item,
                        Notification = new Notification()
                        {
                            Title = "💫🕳💫🕳🕳💯 Thông báo có Plan mới",
                            Body = "Có plan từ Admin vừa tạo"
                        }
                    };

                    var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(messageSend);
                    Console.WriteLine($"Data: {response} Success");
                }*/

                var checkList = new List<string>();
                foreach (var item in data)
                {
                    if (!checkList.Contains(item.Token))
                    {
                        try
                        {
                            var messageSend = new Message
                            {
                                Token = item.Token,
                                Notification = new Notification()
                                {
                                    Title = "💫🕳💫🕳🕳💯 New Plan Notification",
                                    Body = "There is a plan from Admin just created"
                                }
                            };
                            await FirebaseMessaging.DefaultInstance.SendAsync(messageSend);

                            
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        checkList.Add(item.Token);
                    }
                    
                }




                // Cách 2
                //var message = new Message()
                //{
                //    Topic = "allDevices",  // Gửi đến tất cả thiết bị đăng ký topic này "allDevices"
                //    Notification = new Notification()
                //    {
                //        Title = "❤❤❤💘💘",
                //        Body = "💫💨💨💨☮🕳"
                //    }
                //};

                //await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }
    }
}
