using Convey.CQRS.Events;

namespace Trill.Services.Stories.Application.Events
{
    public class StoryActionRejected : IRejectedEvent
    {
        public string Reason { get; }
        public string Code { get; }

        public StoryActionRejected(string reason, string code)
        {
            Reason = reason;
            Code = code;
        }
    }
}