using System;
using Trill.Services.Stories.Application.Services;

namespace Trill.Services.Stories.Infrastructure.Services
{
    internal class IdGenerator : IIdGenerator
    {
        private readonly IdGen.IdGenerator _generator;

        public IdGenerator()
        {
            var generatorId = 0;
            var generatorIdEnv = Environment.GetEnvironmentVariable("GENERATOR_ID");
            if (!string.IsNullOrWhiteSpace(generatorIdEnv))
            {
                int.TryParse(generatorIdEnv, out generatorId);
            }
            
            _generator = new IdGen.IdGenerator(generatorId);
        }

        public long Generate() => _generator.CreateId();
    }
}