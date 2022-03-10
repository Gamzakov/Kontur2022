using System.Collections.Generic;

namespace Morphology.Tests
{
    internal static class SentenceMorpherTestData
    {
        public static List<string> Empty => new();

        public static List<string> EmptyLines => new()
        {
            "",
            "",
            "",
            ""
        };

        public static List<string> Default => new()
        {
            "1",
            "СЛОВО sp1,tag1,tag2,tag3,tag4",
            "СЛОВО1 sp1,tag1,tag2,tag3",
            "СЛОВО2 sp1,tag1,tag2,tag5.1 tag5.2",
            "СЛОВО3 sp1,tag1,tag3",
            "",
            "2",
            "ДРУГОЕСЛОВО sp2,tag1,tag3",
            "ДРУГОЕСЛОВО1 sp2,tag2,tag3,tag5.1 tag5.2",
            "ДРУГОЕСЛОВО2 sp2,tag3,tag4,tag5.1 tag5.2",
            "ДРУГОЕСЛОВО3 sp2,tag1,tag2",
            "ДРУГОЕСЛОВО sp2,tag2,tag3",
            "",
            "3",
            "WORD sp2,tag1, tag4",
            "WORD1 sp2,tag2, tag3",
            "WORD2 sp2,tag4,tag5.0 tag5.2, tag6",
            "WORD3 sp2,tag4,tag5.1 tag5.2, tag6"
        };

        public static List<string> WithMultipleSameNormalForms => new()
        {
            "1",
            "СЛОВО tag1,tag2,tag3,tag4",
            "",
            "2",
            "СЛОВО tag1,tag2,tag3,tag4",
            "ДРУГОЕСЛОВО1 tag2,tag3,tag5.1 tag5.2"
        };
    }
}
