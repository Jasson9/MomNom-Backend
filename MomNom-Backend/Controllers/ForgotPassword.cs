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
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPassword : ControllerBase
    {
        private readonly MomNomContext _context;

        public ForgotPassword(MomNomContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<ForgotPasswordResponse>>> forgotPassword([FromBody] ForgotPasswordRequest msuser)
        {
            try
            {
                var user = _context.MsUsers.Where((e)=> e.Email == msuser.Email).FirstOrDefault();

                if (user == null)
                {
                    throw new BadRequestException<ForgotPasswordResponse>("Email is invalid or not exists");
                }

                if (msuser.Password.Length < 6)
                {
                    throw new BadRequestException<ForgotPasswordResponse>("Password length must be at least 6 characters");
                }

                if (msuser.ConfirmPassword != msuser.Password)
                {
                    throw new BadRequestException<ForgotPasswordResponse>("Confirm password must be same with password");
                }

                var resetEntity = _context.TrPasswordResets.Add(
                    new TrPasswordReset
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = msuser.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(msuser.Password)
                    }
                    );

                await _context.SaveChangesAsync();

                var fromAddress = new MailAddress(AppSettings.Email.Username, "MomNom - No Reply");
                var toAddress = new MailAddress(user.Email, user.Username);
                string fromPassword = AppSettings.Email.Password;
                string Uri = $"{AppSettings.frontEndUri}/reset-password?resetId={resetEntity.Entity.Id}";
                string subject = "MomNom Password Reset Request";
                string body = $"Dear {user.Username},\n\nHere is the link to confirm your password reset request: {Uri}\nNote: Password reset request will only be valid for 30 minutes";

                var smtp = new SmtpClient
                {
                    Host = AppSettings.Email.SmtpServer,
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }

                return new BaseResponse<ForgotPasswordResponse>(new ForgotPasswordResponse
                {
                    message = "Password reset link has been sent to your email",
                });
            }
            catch (BaseException<ForgotPasswordResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<ForgotPasswordResponse>("Unexpected internal server error occured").toResponseOutput();
            }

        }

    }
}
