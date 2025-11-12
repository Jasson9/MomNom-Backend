using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using MomNom_Backend;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Request;
using MomNom_Backend.Model.Response;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MomNom_Backend.Model;

namespace MomNom_Backend.Controllers
{
    [Route("reset-password")]
    [ApiController]
    public class ConfirmResetPassword : ControllerBase
    {
        private readonly MomNomContext _context;

        public ConfirmResetPassword(MomNomContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<string> confirm([FromQuery] ConfirmResetPasswordRequest req)
        {
            try
            {
                var reset = _context.TrPasswordResets.Where((e)=> e.Id == req.resetId && e.Status == "P").FirstOrDefault();

                if (reset == null)
                {
                    return "Invalid reset link.";
                }

                DateTime now = DateTime.Now;
                TimeSpan difference = (TimeSpan)(now - reset.CreatedAt);

                if (difference.Minutes >= 30)
                {
                    return "Reset Link Expired";
                }

                reset.Status = "C";
                var user =  _context.MsUsers.Where(e=>e.Email.Equals(reset.Email)).FirstOrDefault();

                if (user == null)
                {
                    return "User Email not found.";
                }

                user.PasswordHash = reset.PasswordHash;

                await _context.SaveChangesAsync();

                var sessions = await _context.TrUserSessions.Where(e => e.UserId == user.UserId).ExecuteDeleteAsync();

                return "Password Has Been Reset!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Unexpected internal server error occured";
            }

        }

    }
}
