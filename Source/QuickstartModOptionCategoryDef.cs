using Verse;
using Verse.Noise;

namespace Quickstart
{
    public class QuickstartModOptionCategoryDef : Def
    {
        [NoTranslate]
        public string texPath;

        public bool isDev;

        public const string General = "General";
        public const string Scenario = "Scenario";
        public const string Priorities = "Priorities";
        public const string Health = "Health";
        public const string Assignments = "Assignments";
    }
}