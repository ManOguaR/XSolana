using System.Collections.Generic;
using XSolana.Parsers.Models;

namespace XSolana.Conventions
{
    public class PdaDefinition
    {
        public List<PdaSeedDefinition> Seeds { get; set; } = [];
        public PdaProgramDefinition Program { get; set; }
    }
}