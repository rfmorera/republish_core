using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Republish.Data;
using Republish.Data.Repositories;
using Republish.Data.RepositoriesInterfaces;
using Republish.Models;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.DTOs;

namespace Services.Impls
{
    public class AdministratorsService : IAdministratorsService
    {
        //private readonly RepublishContext _context;
        //private readonly IStoredProcedureService _storedProcedureService;
        //private readonly CustomUserManager _userManager;
        //private readonly IForgotPasswordService _forgotPasswordService;
        //public AdministratorsService(RepublishContext context, IStoredProcedureService storedProcedureService, IForgotPasswordService forgotPasswordService, CustomUserManager userManager)
        //{
        //    _context = context;
        //    _storedProcedureService = storedProcedureService;
        //    _userManager = userManager;
        //    _forgotPasswordService = forgotPasswordService;
        //}

        //public async Task<bool> Add(AdministratorUserDTO administrator, int FirmId, string callbackUrl)
        //{
        //    RepublishUser RepublishUser = new RepublishUser()
        //    {
        //        FirmId = FirmId,
        //        FirstName = administrator.Name,
        //        UserType = "admin",
        //        Email = administrator.Email
        //    };

        //    _context.RepublishUsers.Add(RepublishUser);
        //    int userid = RepublishUser.Id;

        //    UserLogin userLogin = new UserLogin()
        //    {
        //        FirmId = FirmId,
        //        UserId = userid,
        //        Enable = true,
        //        FirstTimeLogin = true,
        //        PermissionGroup = "CMS",
        //        IsPswdEncrypted = true,
        //        UserName = administrator.Username,
        //        NormalizedUserName = administrator.Username.ToUpper(),
        //        TwoFactorEnabled = false,
        //        EmailConfirmed = true,
                
        //    };
        //    IdentityResult res = await _userManager.CreateAsync(userLogin);

        //    await _userManager.AddToRoleAsync(userLogin, "admin");

        //    await _forgotPasswordService.SendResetPasswordEmail(userLogin, callbackUrl, true);

        //    return true;
        //}

        //public async Task Delete(int UserId)
        //{
        //    var id = new SqlParameter("@UserId", UserId);
        //    await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.superuser_administrators_delete @UserId", id);
        //}
        
        //public async Task<IEnumerable<AdministratorUserDTO>> GetAll()
        //{
        //    IEnumerable<AdministratorUserDTO> list = await (from user in _context.RepublishUsers
        //                                                    join login in _context.Users on user.Id equals login.UserId
        //                                                    join lawFirm in _context.LawFirms on user.FirmId equals lawFirm.Id
        //                                                    where user.UserType == "admin"
        //                                                    select new AdministratorUserDTO()
        //                                                    {
        //                                                        UserId = user.Id,
        //                                                        Name = user.FirstName,
        //                                                        Username = login.UserName,
        //                                                        Email = user.Email,
        //                                                    }
        //                                              ).ToListAsync();
        //    return list;
        //}
    }
}
