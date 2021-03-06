using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApetitOMate.Core.Api.Apetito;
using ClosedXML.Excel;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ApetitOMate.Core.Api.Apetito
{
    public class ApetitoApiTest
    {
        private ApetitoApi api = new ApetitoApiFactory(Config.Instance.ApetitoConfig).Build();

        [Test]
        public async Task TestGetOrders()
        {
            Order[] guests = await this.api.GetOrders("2019-02-05");
            guests.Where(guest => guest.ArticleDescription == "Geschnittene Currywurst").Should().HaveCount(3);
        }


        [Test]
        public async Task TestIncompleteOrders()
        {
            Order[] guests = await this.api.GetOrders("2019-03-01");
            guests.Where(guest => !guest.IsOrderFulfilled)
                .Should().HaveCount(1)
                .And.Subject.First()
                    .Should().Match<Order>(guest => guest.OrderState == "4" && guest.OrderPositionState == "UnsuficcientStock");
        }


        [Test]
        public async Task TestGetTableGuests()
        {
            TableGuest[] guests = await this.api.GetTableGuests();
            guests.Where(guest => guest.EmailAddress.EndsWith("@cqse.eu")).Should().NotBeEmpty();
            foreach (TableGuest disabledGuest in guests.Where(guest => guest.IsLocked == true))
            {
                disabledGuest.IsLocked = false;
                TableGuest updated = await this.api.UpdateTableGuest(disabledGuest.Id, disabledGuest);
                updated.IsLocked.Should().BeFalse();
            }

        }

        [Test]
        public async Task TestGetTableGuestGroups()
        {
            TableGuestGroup[] groups = await this.api.GetTableGuestGroups();
            groups.Where(group => group.Id == 197).Should().HaveCount(1)
                .And.Subject.First().Should().Match<TableGuestGroup>(group => group.GroupName == "MAUS");
        }

        [Category("GDI")]
        [Test]
        public async Task TestDownloadTableGuestBillingDetail()
        {
            Stream download = await this.api.DownloadTableGuestBilling(new TableGuestBillingRequestOptions
            {
                TableGuestGroupIds = (await this.api.GetTableGuestGroups()).Select(group => group.Id).ToArray(),
                AllChecked = true,
                Kind = EKind.Detail
            }, "2019-03-13", "2019-03-13");


            using (var workbook = new XLWorkbook(download))
            {
                workbook.Worksheet(1).RowsUsed().Should().HaveCount(19);
            }
        }

        [Category("GDI")]
        [Test]
        public async Task TestDownloadTableGuestBillingGesamt()
        {
            Stream download = await this.api.DownloadTableGuestBilling(new TableGuestBillingRequestOptions
            {
                TableGuestGroupIds = (await this.api.GetTableGuestGroups()).Select(group => group.Id).ToArray(),
                AllChecked = true,
                Kind = EKind.Gesamt
            }, "2019-03-13", "2019-03-13");


            using (var workbook = new XLWorkbook(download))
            {
                workbook.Worksheet(1).RowsUsed().Should().HaveCount(18);
            }
        }

        [Test]
        public async Task TestGetStorageLocations()
        {
            StorageLocation[] locations = await this.api.GetStorageLocations();
            locations.Should().HaveCountGreaterThan(1).And.Subject.First().Should().Match<StorageLocation>(location => location.Id == 135);
        }

        [Test]
        public async Task TestGetInventory()
        {
            StorageLocation[] locations = await this.api.GetStorageLocations();

            Inventory[] inventoryAll = await this.api.GetInventory(locations.First(), true);
            inventoryAll.Should().Contain(inventory => inventory.StockQuantity == 0);

            Inventory[] inventoryInStock = await this.api.GetInventory(locations.First(), false);
            inventoryInStock.Should().NotContain(inventory => inventory.StockQuantity == 0);
        }

        [Test]
        public async Task TestGetInventoryOrders()
        {
            InventoryOrder[] orders = await this.api.GetInventoryOrders();
            orders.Should().Contain(order => order.GoodsReceipt != null && order.GoodsReceipt.IsBookedToStorageLocations);
        }
    }
}