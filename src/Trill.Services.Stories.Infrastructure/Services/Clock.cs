using System;
using Trill.Services.Stories.Application.Services;

namespace Trill.Services.Stories.Infrastructure.Services
{
    internal class UtcClock : IClock
    {
        public DateTime Current()  => DateTime.UtcNow;
    }
}