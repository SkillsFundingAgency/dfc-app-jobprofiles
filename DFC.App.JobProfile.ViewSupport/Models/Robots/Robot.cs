using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.JobProfile.Models.Robots
{
    [ExcludeFromCodeCoverage]
    public sealed class Robot
    {
        private readonly StringBuilder robotData;

        public Robot()
        {
            robotData = new StringBuilder();
        }

        public string Data => robotData.ToString();

        public void Add(string text)
        {
            robotData.AppendLine(text);
        }
    }
}
