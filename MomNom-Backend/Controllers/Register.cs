using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly MomNomContext _context;

        public RegisterController(MomNomContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<RegisterResponse>>> RegisterUser([FromBody] RegisterRequest msuser)
        {
            try
            {
                if (msuser.Password.Length < 6)
                {
                    throw new BadRequestException<RegisterResponse>("Password length must be at least 6 characters");
                }

                if (msuser.Username.Length < 4)
                {
                    throw new BadRequestException<RegisterResponse>("Username length must be at least 6 characters");
                }

                if (msuser.Username.IsContainingSymbol())
                {
                    throw new BadRequestException<RegisterResponse>("Username cannot contain symbol");
                }

                if (msuser.Email.IsValidEmail() == false)
                {
                    throw new BadRequestException<RegisterResponse>("Requested email is not a valid email");
                }

                if (msuser.PasswordConfirm != msuser.Password)
                {
                    throw new BadRequestException<RegisterResponse>("Confirm password must be same with password");
                }

                if (_context.MsUsers.Where((e => e.Email == msuser.Email)).Count() >= 1)
                {
                    throw new BadRequestException<RegisterResponse>("Same email already exist");
                }

                var user = _context.MsUsers.Add(new MsUser
                {
                    Email = msuser.Email,
                    Username = msuser.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(msuser.Password)
                });
                await _context.SaveChangesAsync();

                var sessionId = await Auth.generateSessionId(_context, user.Entity.UserId);

                return new BaseResponse<RegisterResponse>(new RegisterResponse
                {
                    sessionId = sessionId
                });
            }
            catch (BaseException<RegisterResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<RegisterResponse>("Unexpected internal server error occured").toResponseOutput();
            }

        }

    }
}
