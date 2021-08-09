using System;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.AspNetCore.Mvc;
using Trill.Services.Stories.Application.Commands;
using Trill.Services.Stories.Application.Services;

namespace Trill.Services.Stories.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IStoryRequestStorage _storyRequestStorage;

        public StoriesController(ICommandDispatcher commandDispatcher, IStoryRequestStorage storyRequestStorage)
        {
            _commandDispatcher = commandDispatcher;
            _storyRequestStorage = storyRequestStorage;
        }

        [HttpPost]
        public async Task<ActionResult> Post(SendStory command)
        {
            await _commandDispatcher.SendAsync(command);
            var storyId = _storyRequestStorage.GetStoryId(command.Id);
            return Created($"api/stories/{storyId}", null);
        }
    }
}