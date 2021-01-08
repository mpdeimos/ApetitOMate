using System;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using FluentAssertions;
using NUnit.Framework;

namespace ApetitOMate.Core.Action
{
    public class UnfulfilledOrderRetrieverTest
    {
        [Test]
        [Ignore("Sadly the unfulfilled orders have been deleted")]
        public async Task Test()
        {
            var retriever = new UnfulfilledOrderRetriever(new ApetitoApiFactory(Config.Instance.ApetitoConfig).Build());
            Order[] guests = await retriever.Get(DateTime.Parse("2019-03-01"), DateTime.Parse("2019-03-01"));
            guests.Should().HaveCount(1);
        }
    }
}