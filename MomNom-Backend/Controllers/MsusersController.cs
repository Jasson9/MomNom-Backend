using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomNom_Backend;
using MomNom_Backend.Model.Db;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MsusersController : ControllerBase
    {
        private readonly MomNomContext _context;

        public MsusersController(MomNomContext context)
        {
            _context = context;
        }

        // GET: api/Msusers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MsUser>>> GetMsusers()
        {
            return await _context.MsUsers.ToListAsync();
        }

        // GET: api/Msusers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MsUser>> GetMsuser(int id)
        {
            var msuser = await _context.MsUsers.FindAsync(id);

            if (msuser == null)
            {
                return NotFound();
            }

            return msuser;
        }

        // PUT: api/Msusers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMsuser(int id, MsUser msuser)
        {
            if (id != msuser.UserId)
            {
                return BadRequest();
            }

            _context.Entry(msuser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MsuserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Msusers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MsUser>> PostMsuser(MsUser msuser)
        {
            _context.MsUsers.Add(msuser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMsuser", new { id = msuser.UserId }, msuser);
        }

        // DELETE: api/Msusers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMsuser(int id)
        {
            var msuser = await _context.MsUsers.FindAsync(id);
            if (msuser == null)
            {
                return NotFound();
            }

            _context.MsUsers.Remove(msuser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MsuserExists(int id)
        {
            return _context.MsUsers.Any(e => e.UserId == id);
        }
    }
}
