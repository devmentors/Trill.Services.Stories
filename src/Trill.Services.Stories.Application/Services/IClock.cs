using System;

namespace Trill.Services.Stories.Application.Services
{
    public interface IClock
    {
        DateTime Current();
    }
}