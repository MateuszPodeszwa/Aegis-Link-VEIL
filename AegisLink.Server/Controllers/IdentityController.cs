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
        public Task<IActionResult> Deregister([FromBody] UserKeyReg request)
        {
            // Disabled until a secure deregistration authorization flow exists.
            // The current AegisId + PublicKey check is not sufficient because the
            // public key is retrievable via the lookup endpoint and does not prove
            // possession of a secret or private key.
            IActionResult result = StatusCode(501, new
            {
                error = "Deregistration is temporarily unavailable until a secure authorization flow is implemented"
            });

            return Task.FromResult(result);
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
