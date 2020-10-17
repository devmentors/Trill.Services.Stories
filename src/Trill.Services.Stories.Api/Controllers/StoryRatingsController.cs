using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.AspNetCore.Mvc;
using Trill.Services.Stories.Application.Commands;

namespace Trill.Services.Stories.Api.Controllers
{
    [ApiController]
    [Route("api/stories/{storyId}")]
    public class StoryRatingsController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public StoryRatingsController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost("rate")]
        public async Task<ActionResult> Post(RateStory command)
        {
            await _commandDispatcher.SendAsync(command);
            return NoContent();
        }
    }
}