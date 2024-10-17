using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PacificTest.Services;

namespace PacificTest.Controllers
{
    [ApiController]
    public class AvatarController : ControllerBase
    {
        private IAvatarService avatarService;

        public AvatarController(IAvatarService avatar)
        {
            avatarService = avatar;
        }

        [Route("/avatar")]
        public async Task<IActionResult> Get(string? UserIdentifier)
        {
            var url = await avatarService.GetAvatar(UserIdentifier);

            return Ok(new { id = UserIdentifier, url });
        }
    }
}
