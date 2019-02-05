using FluentAssertions;
using NUnit.Framework;

namespace ApetitOMate.Core.Model
{
    public class TableGuestTest
    {
        [Test]
        public void SimpleTest()
        {
            var guest = new TableGuest
            {
                ArticleDescription = "Thai-Gemüse-Curry"
            };

            guest.ArticleDescription.Should().Be("Thai-Gemüse-Curry");
        }
    }
}
