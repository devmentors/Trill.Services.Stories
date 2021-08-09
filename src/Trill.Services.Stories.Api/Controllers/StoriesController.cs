using System;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Microsoft.AspNetCore.Mvc;
using Trill.Services.Stories.Application.Commands;
using Trill.Services.Stories.Application.DTO;
using Trill.Services.Stories.Application.Queries;
using Trill.Services.Stories.Application.Services;

namespace Trill.Services.Stories.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IStoryRequestStorage _storyRequestStorage;

        public StoriesController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher,
            IStoryRequestStorage storyRequestStorage)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _storyRequestStorage = storyRequestStorage;
        }
        
        [HttpGet]
        public async Task<ActionResult<PagedDto<StoryDto>>> Get([FromQuery] BrowseStories query)
            => Ok(await _queryDispatcher.QueryAsync(query));
        
        [HttpGet("{storyId:long}")]
        public async Task<ActionResult<StoryDetailsDto>> Get(long storyId, [FromQuery] GetStory query)
        {
            query.StoryId = storyId;
            var story = await _queryDispatcher.QueryAsync(query);
            if (story is null)
            {
                return NotFound();
            }

            return Ok(story);
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