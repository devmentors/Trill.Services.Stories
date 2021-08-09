using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Convey.Persistence.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Trill.Services.Stories.Application.DTO;
using Trill.Services.Stories.Application.Queries;
using Trill.Services.Stories.Application.Services;
using Trill.Services.Stories.Infrastructure.Mongo.Documents;

namespace Trill.Services.Stories.Infrastructure.Mongo.Queries.Handlers
{
    public class BrowseStoriesHandler : IQueryHandler<BrowseStories, PagedDto<StoryDto>>
    {
        private readonly IMongoDatabase _database;
        private readonly IClock _clock;

        public BrowseStoriesHandler(IMongoDatabase database, IClock clock)
        {
            _database = database;
            _clock = clock;
        }

        public async Task<PagedDto<StoryDto>> HandleAsync(BrowseStories query)
        {
            var now = (query.Now.HasValue ? query.Now.Value : _clock.Current()).ToUnixTimeMilliseconds();
            var documents = _database.GetCollection<StoryDocument>("stories")
                .AsQueryable()
                .Where(x => x.From <= now && x.To >= now);

            var input = query.Query;
            if (!string.IsNullOrWhiteSpace(input))
            {
                documents = documents.Where(x =>
                    x.Title.Contains(input) || x.Author.Name.Contains(input) || x.Tags.Contains(input));
            }

            var result = await documents.OrderByDescending(x => x.CreatedAt).PaginateAsync(query);
            var storyIds = result.Items.Select(x => x.Id);

            var rates = await _database.GetCollection<StoryRatingDocument>("ratings")
                .AsQueryable()
                .Where(x => storyIds.Contains(x.StoryId))
                .ToListAsync();

            return new PagedDto<StoryDto>
            {
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                ResultsPerPage = result.ResultsPerPage,
                TotalResults = result.TotalResults,
                Items = result.Items.Select(x => x.ToDto(rates))
            };
        }
    }
}