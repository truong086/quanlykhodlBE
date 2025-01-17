﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Pkcs;
using quanlykhodl.Clouds;
using quanlykhodl.Common;
using quanlykhodl.EmailConfigs;
using quanlykhodl.FunctionAuto;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace quanlykhodl.Service
{
    public class AccountService : IAccountService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private Jwt _jwt;
        private readonly Cloud _cloud;
        private readonly SendEmais _emails;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        public AccountService(DBContext context, IMapper mapper, IOptionsMonitor<Jwt> jwt, IOptions<Cloud> cloud,
            SendEmais emails, IHttpContextAccessor httpContextAccessor, IRoleService roleService,
            IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _jwt = jwt.CurrentValue;
            _cloud = cloud.Value;
            _emails = emails;
            _httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
            _userService = userService;

        }
        public async Task<PayLoad<AccountDTO>> Add(AccountDTO accountDTO)
        {
            try
            {
                var checkData = _context.accounts.Where(x => x.email == accountDTO.email || 
                x.phone == accountDTO.phone || x.username == accountDTO.username && !x.Deleted).FirstOrDefault();

                var checkRole = _context.roles.Where(x => x.name.ToLower() == TokenViewModel.USER.ToLower() 
                && !x.Deleted).FirstOrDefault();
                if (checkData != null)
                    return await Task.FromResult(PayLoad<AccountDTO>.CreatedFail(Status.DATATONTAI));

                uploadCloud.CloudInaryIFromAccount(accountDTO.image, accountDTO.email, _cloud);
                var mapData = _mapper.Map<Account>(accountDTO);
                mapData.password = EncryptionHelper.CreatePasswordHash(accountDTO.password, _jwt.Key);
                mapData.Action = false;
                mapData.image = uploadCloud.Link;
                mapData.publicid = uploadCloud.publicId;
                mapData.Deleted = false;
                mapData.role = checkRole;
                mapData.role_id = checkRole.id;

                var descriptEmail = new SendEmail
                {
                    title = "Mã xác nhận tài khoản",
                    message = "Thông tin mã xác nhận",
                    iamge = uploadCloud.Link,
                    name = mapData.username,
                    active = geneAction()
                };

                
                _context.accounts.Add(mapData);

                var checkToken = _context.tokens.Where(x => x.account_id == mapData.id && !x.Deleted && x.Status == Status.CREATEPASSWORD).ToList();
                if(checkToken.Any() || checkToken != null)
                {
                    _context.tokens.RemoveRange(checkToken);
                    await _context.SaveChangesAsync();
                }

                var tokenOTP = new Token
                {
                    account_id = mapData.id,
                    account = mapData,
                    code = descriptEmail.active,
                    Status = Status.CREATEPASSWORD
                };

                _context.tokens.Add(tokenOTP);
                await _context.SaveChangesAsync();

                var tempalte = Status.TEMPLATEVIEW;

                var tempalateEmail = await _emails.RenderViewToStringAsync(tempalte, descriptEmail);
                await _emails.SendEmai(mapData.email, descriptEmail.title, tempalateEmail);

                // Khởi động Background Task để xử lý
                _ = Task.Run(() =>
                {
                    var work = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<VerificationTaskWorker>();
                    work.ProcessVerificationAsync(tokenOTP, Status.CREATEPASSWORD); // Chuyền dữ liệu vào hàm "VerificationTaskWorker" này
                });

                return await Task.FromResult(PayLoad<AccountDTO>.Successfully(accountDTO));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<AccountDTO>.CreatedFail(ex.Message));
            }
        }

        private string geneAction()
        {
            var random = new Random();
            string code = Status.RANDOMCODE;
            var geneCode = new string(Enumerable.Repeat(code, 6).Select(s => s[random.Next(s.Length)]).ToArray());
            return geneCode;
        }

        public async Task<PayLoad<string>> Delete(int id)
        {
            try { 
                var deleteAccount = _context.accounts.Where(x => x.id == id).FirstOrDefault();
                var checkToken = _context.tokens.Include(a => a.account).Where(x => x.account_id == deleteAccount.id).ToList();

                if (deleteAccount.image != null)
                    uploadCloud.DeleteAllImageAndFolder(deleteAccount.email, _cloud);
                _context.tokens.RemoveRange(checkToken);
                _context.accounts.Remove(deleteAccount);

                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try 
            {
                var data = _context.accounts.Where(x => !x.Deleted).ToList();

                if (!string.IsNullOrEmpty(name))
                    data = data.Where(x => x.username.Contains(name) || x.email.Contains(name)).ToList();

                var pageList = new PageList<object>(loadData(data), page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages

                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<AccountgetAll> loadData(List<Account> data)
        {
            var list = new List<AccountgetAll>();
            foreach(var item in data)
            {
                var mapData = _mapper.Map<AccountgetAll>(item);
                mapData.Id = item.id;
                mapData.roleName = checkRole(item.role_id.Value).name;

                list.Add(mapData);
            }

            return list;
        }

        public async Task<PayLoad<object>> FindOne(int id)
        {
            try
            {
                var checkAccount = _context.accounts.Include(r => r.role).Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    id = checkAccount.id,
                    username = checkAccount.username,
                    email = checkAccount.email,
                    phone = checkAccount.phone,
                    image = checkAccount.image,
                    action = checkAccount.Action,
                    role_name = checkRole(checkAccount.role_id.Value) == null ? Status.ROLENOTFOULD : checkRole(checkAccount.role_id.Value).name,
                    role_id = checkRole(checkAccount.role_id.Value) == null ? Status.ROLENOTFOULD : checkRole(checkAccount.role_id.Value).id.ToString()
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<ReturnLogin>> LoginPage(Login accountDTO)
        {
            try
            {
                var passwordEncoding = EncryptionHelper.CreatePasswordHash(accountDTO.password, _jwt.Key);
                var checkAccount = _context.accounts.Include(r => r.role).Where(x => (x.email == accountDTO.username || x.username == accountDTO.username) && x.password == passwordEncoding).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<ReturnLogin>.CreatedFail(Status.DATANULL));

                var checkRoleId = checkRole(checkAccount.role_id.Value);
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(Status.IDAUTHENTICATION, checkAccount.id.ToString()),
                    new Claim(ClaimTypes.Role, checkRoleId == null ? "Role không tồn tại" : checkRoleId.name),

                };

                return await Task.FromResult(PayLoad<ReturnLogin>.Successfully(new ReturnLogin
                {
                    id = checkAccount.id,
                    username = checkAccount.username,
                    Email = checkAccount.email,
                    Image = checkAccount.image,
                    role = checkRoleId == null ? "Role đã bị xóa hoặc chưa cài đặt" : checkRoleId.name,
                    Sdt = checkAccount.phone,
                    Token = GenerateToken(claims)
                }));
            }catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<ReturnLogin>.CreatedFail(ex.Message));
            }
        }

        private role checkRole(int id)
        {
            var roleId = _context.roles.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
            if (roleId == null)
                return null;

            return roleId;
        }
        private string GenerateToken(List<Claim>? claim)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creadentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_jwt.Issuer,
                _jwt.Issuer,
                expires: DateTime.Now.AddMinutes(12000),
                claims: claim,
                signingCredentials: creadentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<PayLoad<AccountUpdate>> Update(int id, AccountUpdate accountDTO)
        {
            try {
                //var user = _userService.name();
                //var chuyenDoi = Convert.ToInt32(user);
                var checkId = _context.accounts.Include(r => r.role).Where(x => x.id == id && !x.Deleted).FirstOrDefault();
                var checkRole = _context.roles.Where(x => x.id == accountDTO.role_id && !x.Deleted).FirstOrDefault();
                if (checkId == null || checkRole == null)
                    return await Task.FromResult(PayLoad<AccountUpdate>.CreatedFail(Status.DATANULL));

                if(accountDTO.image != null)
                {
                    uploadCloud.DeleteImageAllOnFoderCloud(checkId.email, _cloud);
                    uploadCloud.CloudInaryIFromAccount(accountDTO.image, checkId.email, _cloud);

                    checkId.image = uploadCloud.Link;
                    checkId.publicid = uploadCloud.publicId;
                }

                checkId.username = accountDTO.username;
                checkId.email = accountDTO.email;
                checkId.address = accountDTO.address;
                checkId.UpdatedAt = DateTimeOffset.UtcNow;
                checkId.role = checkRole;
                checkId.role_id = accountDTO.role_id;

                _context.accounts.Update(checkId);

                _context.SaveChanges();

                return await Task.FromResult(PayLoad<AccountUpdate>.Successfully(accountDTO));


            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<AccountUpdate>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> DeleteToken(Token token, string? status)
        {
            try
            {
                var checkToken = _context.tokens.Include(a => a.account)
                    .Where(x => x.account_id == token.account_id && x.Status == token.Status && !x.Deleted)
                    .ToList();

                if(checkToken != null)
                {
                    _context.tokens.RemoveRange(checkToken);

                    if (await _context.SaveChangesAsync() > 0)
                        return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
                }

                return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATAERROR));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> DeleteAccountNoAction()
        {
            try
            {
                var checkData = _context.accounts.Where(x => !x.Action && !x.Deleted).ToList();
                deleteAccountAndTokenNoAction(checkData);

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        private void deleteAccountAndTokenNoAction(List<Account> data)
        {
            if (data.Any())
            {

                foreach (var item in data)
                {
                    var checkTokenCreate = _context.tokens.Where(x => x.account_id == item.id).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                    if (checkTokenCreate != null)
                    {
                        var dateChenhLenh = checkDateChenhLech(checkTokenCreate.CreatedAt);
                        if (dateChenhLenh >= 1)
                        {
                            var deleteData = _context.tokens.Include(a => a.account).Where(x => x.account_id == item.id).ToList();
                            _context.tokens.RemoveRange(deleteData);
                        }
                    }

                    var checkDateAccount = checkDateChenhLech(item.CreatedAt);
                    if (checkDateAccount >= 1)
                    {
                        uploadCloud.DeleteAllImageAndFolder(item.email, _cloud);
                        _context.accounts.Remove(item);

                    }

                    _context.SaveChanges();
                }
            }
        }

        private int checkDateChenhLech(DateTimeOffset data)
        {
            var dateNow = DateTimeOffset.UtcNow;

            TimeSpan tinhChenhLech = dateNow.Subtract(data);

            int chuyenDoi = Math.Abs(tinhChenhLech.Hours);
            /*
                "Days": Ngày
                "Hours": Giờ
                "Minutes": Phút
                "Seconds": Giây
             */

            return chuyenDoi;


        }

        public async Task<PayLoad<string>> ReloadOTP(reLoadOtp data)
        {
            try
            {
                var checkEmail = _context.accounts.Where(x => x.email ==  data.email && !x.Deleted && data.type.ToLower() == Status.UPDATEPASSWORD.ToLower() ? x.Action : !x.Action).FirstOrDefault();
                if (checkEmail == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                var checkToken = _context.tokens.Include(a => a.account)
                    .Where(x => x.account_id == checkEmail.id && !x.Deleted)
                    .ToList();

                if (checkToken.Any() || checkToken != null)
                {
                    _context.tokens.RemoveRange(checkToken);
                    await _context.SaveChangesAsync();
                }

                var descriptEmail = new SendEmail
                {
                    title = Status.TITLEEMAIL,
                    message = Status.MESSAGEEMAIL,
                    iamge = checkEmail.image,
                    name = checkEmail.username,
                    active = geneAction()
                };
                var tokenOTP = new Token
                {
                    account_id = checkEmail.id,
                    account = checkEmail,
                    code = descriptEmail.active,
                    Status = data.type.ToLower() == Status.UPDATEPASSWORD.ToLower() ? Status.UPDATEPASSWORD : Status.CREATEPASSWORD
                };

                _context.tokens.Add(tokenOTP);
                await _context.SaveChangesAsync();

                var tempalte = Status.TEMPLATEVIEW;

                var tempalateEmail = await _emails.RenderViewToStringAsync(tempalte, descriptEmail);
                await _emails.SendEmai(checkEmail.email, descriptEmail.title, tempalateEmail);

                // Khởi động Background Task để xử lý
                _ = Task.Run(() =>
                {
                    var work = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<VerificationTaskWorker>();
                    work.ProcessVerificationAsync(tokenOTP, Status.CREATEPASSWORD); // Chuyền dữ liệu vào hàm "VerificationTaskWorker" này
                });

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> ForgotPassword(forgotPassword data)
        {
            try
            {
                var checkData = _context.accounts.Where(x => x.email == data.email && !x.Deleted && x.Action).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                if (checkData.password != EncryptionHelper.CreatePasswordHash(data.passwordOld, _jwt.Key))
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.PASSWORDOLDFAILD));

                checkData.password = EncryptionHelper.CreatePasswordHash(data.passwordNew, _jwt.Key);
                checkData.UpdatedAt = DateTimeOffset.UtcNow;

                _context.accounts.Update(checkData);

                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> Action(ActionAccount data)
        {
            try
            {
                var checkEmail = _context.accounts.Where(x => x.email == data.email && !x.Action && !x.Deleted).FirstOrDefault();
                if (checkEmail == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                var checkToken = checkTokenAccount(checkEmail.id, data.code, Status.CREATEPASSWORD);
                if (checkToken == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                var deleteToken = _context.tokens.Include(a => a.account).Where(x => x.account_id == checkEmail.id).ToList();
                checkEmail.Action = true;
                _context.accounts.Update(checkEmail);
                _context.tokens.RemoveRange(deleteToken);

                await _context.SaveChangesAsync();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> CheckCode(ActionAccount data)
        {
            try
            {
                var checkEmail = _context.accounts.Where(x => x.email == data.email && !x.Deleted && x.Action).FirstOrDefault();
                if(checkEmail == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                var checkToken = checkTokenAccount(checkEmail.id, data.code, Status.UPDATEPASSWORD);
                if (checkToken == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                var deleteToken = _context.tokens.Include(a => a.account).Where(x => x.account_id == checkEmail.id).ToList();
                _context.tokens.RemoveRange(deleteToken);

                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        private Token checkTokenAccount(int id_account, string code, string status)
        {
            var checkToken = _context.tokens.Include(a => a.account).Where(x => x.account_id == id_account && x.code == code && x.Status == status)
                    .OrderByDescending(x => x.CreatedAt).FirstOrDefault();

            return checkToken == null ? null : checkToken;
        }

        public async Task<PayLoad<object>> FindAllToken()
        {
            var data = _context.tokens.ToList();

            return await Task.FromResult(PayLoad<object>.Successfully(data));
        }

        public async Task<PayLoad<string>> updatePasswords(updatatePasswordAccount data)
        {
            try
            {
                var checkEmail = _context.accounts.Where(x => x.email == data.email).FirstOrDefault();
                if (checkEmail == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                checkEmail.password = EncryptionHelper.CreatePasswordHash(data.passwordNew, _jwt.Key);

                _context.accounts.Update(checkEmail);

                _context.SaveChanges();

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
            }catch(Exception ex )
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }
    }
}
