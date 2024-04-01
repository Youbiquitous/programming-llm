using Microsoft.SemanticKernel.Services;

namespace Prenoto.Common
{
    public class Settings
    {
        public Settings()
        {
            
        }
        public AIService AIService { get; set; }
    }

    public class AIService
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public Models Models { get; set; }
    }

    public class Models
    {
        public string Completion { get; set; }
    }
}
