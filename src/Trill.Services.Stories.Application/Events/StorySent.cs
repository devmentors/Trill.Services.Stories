using System;
using System.Collections.Generic;
using Convey.CQRS.Events;

namespace Trill.Services.Stories.Application.Events
{
    public class StorySent : IEvent
    {
        public long StoryId { get; }
        public AuthorModel Author { get; }
        public string Title { get; }
        public IEnumerable<string> Tags { get; }
        public DateTime CreatedAt { get; }
        public VisibilityModel Visibility { get; }

        public StorySent(long storyId, AuthorModel author, string title, IEnumerable<string> tags, DateTime createdAt,
            VisibilityModel visibility)
        {
            StoryId = storyId;
            Author = author;
            Title = title;
            Tags = tags;
            CreatedAt = createdAt;
            Visibility = visibility;
        }

        public class AuthorModel
        {
            public Guid Id { get; }
            public string Name { get; }

            public AuthorModel(Guid id, string name)
            {
                Id = id;
                Name = name;
            }
        }
        
        public class VisibilityModel
        {
            public DateTime From { get; }
            public DateTime To { get; }
            public bool Highlighted { get; }

            public VisibilityModel(DateTime from, DateTime to, bool highlighted)
            {
                From = from;
                To = to;
                Highlighted = highlighted;
            }
        }
    }
}