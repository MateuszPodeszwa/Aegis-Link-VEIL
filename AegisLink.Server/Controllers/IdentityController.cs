using AegisLink.Server.Data;
using AegisLink.Server.Services;
using AegisLink.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AegisLink.Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly AegisLinkDbContext _dbContext;

        public IdentityController(AegisLinkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserKeyReg request)
        {
            if (!AegisIdService.Verify(request.AegisId, request.PublicKey))
                return BadRequest(new { error = "ID doesn't match public key" });

            var existing = await _dbContext.UserKeys.FindAsync(request.AegisId);

            if (existing is not null)
            {
                if (existing.PublicKey == request.PublicKey)
                    return Ok(new { aegisId = request.AegisId });

                return Conflict(new { error = "This ID is already taken" });
            }

            _dbContext.UserKeys.Add(new UserKey
            {
                AegisId = request.AegisId,
                PublicKey = request.PublicKey,
            });

            await _dbContext.SaveChangesAsync();

            return Ok(new { aegisId = request.AegisId });
            
        }

        [HttpDelete("deregister")]
        public async Task<IActionResult> Deregister([FromBody] UserKeyReg request)
        {
            // Re-run the same ownership check used at registration.
            // The caller must prove they hold the public key that hashes to the Aegis ID.
            if (!AegisIdService.Verify(request.AegisId, request.PublicKey))
                return BadRequest(new { error = "ID doesn't match public key" });

            var existing = await _dbContext.UserKeys.FindAsync(request.AegisId);

            if (existing is null)
                return NotFound(new { error = "ID not found" });

            if (existing.PublicKey != request.PublicKey)
                return Forbid();

            _dbContext.UserKeys.Remove(existing);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("lookup/{id}")]
        public async Task<IActionResult> Lookup(string id)
        {
            if (id.Length != 8)
                return BadRequest(new { error = "Invalid ID" });

            var entry = await _dbContext.UserKeys.AsNoTracking().FirstOrDefaultAsync(k => k.AegisId == id);

            if (entry is null)
                return NotFound(new { error = "ID not found" });

            return Ok(new UserKeyGetResult(entry.AegisId, entry.PublicKey));
        }
    }
}
