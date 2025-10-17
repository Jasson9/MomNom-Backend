using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Request;
using MomNom_Backend.Model.Exception;
using MomNom_Backend;
using System.Linq;

namespace MomNom_Backend.Controllers
{
    public class Auth
    {

        public async static Task<string> generateSessionId(MomNomContext context, int userId)
        {
            var sessionId = Guid.NewGuid().ToString();
            var session = context.TrUserSessions.Add(new TrUserSession
            {
                UserId = userId,
                SessionId = sessionId,
            });

            await context.SaveChangesAsync();
            return Utils.Base64Encode(sessionId);
        }

        public async static Task<MsUser> ValidateAuthToken(MomNomContext context, string authToken)
        {
            try
            {
                var sessionId = Utils.Base64Decode(authToken);
                List<TrUserSession> session = await context.TrUserSessions.Where((e)=>e.SessionId == sessionId && e.ExpiryDateTime >= DateTime.Now).ToListAsync();
                if (session.Count == 0)
                {
                    throw new UnauthorizedException<MsUser>("Invalid session");
                }

                List<MsUser> users = await context.MsUsers.ToListAsync();
                var user = session.Join(users, s=>s.UserId, u=>u.UserId, (s, u) => new MsUser { UserId = u.UserId, MsPlans = u.MsPlans, Username = u.Username, Email = u.Email }).FirstOrDefault();
                if(user == null) throw new UnauthorizedException<MsUser>("User cannot be found");
                return user;
            }
            catch (UnauthorizedException<MsUser>)
            {
                throw;
            }
            catch (Exception)
            {
                throw new UnauthorizedException<MsUser>("Invalid session");
            }
        }
    }
}
