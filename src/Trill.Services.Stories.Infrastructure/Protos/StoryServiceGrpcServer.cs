using System;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Grpc.Core;
using Trill.Services.Ads;
using Trill.Services.Stories.Application.Commands;
using Trill.Services.Stories.Application.Services;

namespace Trill.Services.Stories.Infrastructure.Protos
{
    public class StoryServiceGrpcServer : StoryService.StoryServiceBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IStoryRequestStorage _storyRequestStorage;

        public StoryServiceGrpcServer(ICommandDispatcher commandDispatcher, IStoryRequestStorage storyRequestStorage)
        {
            _commandDispatcher = commandDispatcher;
            _storyRequestStorage = storyRequestStorage;
        }

        public override async Task<SendStoryResponse> SendStory(SendStoryCommand request, ServerCallContext context)
        {
            var command = new SendStory(default, Guid.Parse(request.UserId), request.Title,
                request.Text, request.Tags, DateTime.Parse(request.VisibleFrom), DateTime.Parse(request.VisibleTo),
                request.Highlighted);
            
            await _commandDispatcher.SendAsync(command);
            
            return new SendStoryResponse
            {
                Id = _storyRequestStorage.GetStoryId(command.Id)
            };
        }
    }
}