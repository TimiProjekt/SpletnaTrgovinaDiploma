using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using SpletnaTrgovinaDiploma.Data.Services;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public class XmlImportUtil
    {
        private readonly IItemsService itemsService;

        public XmlImportUtil(IItemsService itemsService)
        {
            this.itemsService = itemsService;
        }

        public async Task<int> TryImportItemDetails(XmlDocument doc, bool isUpdateExisting)
        {
            var productCatalog = doc?
                .DocumentElement?
                .SelectSingleNode("/ProductCatalog");

            return await TryImportListOfProducts(productCatalog, isUpdateExisting);
        }

        public async Task<int> TryImportAvailability(XmlDocument doc, bool isUpdateExisting)
        {
            var prices = doc?
                .DocumentElement?
                .SelectSingleNode("/CONTENT/PRICES");

            return await TryImportListOfProducts(prices, isUpdateExisting);
        }

        async Task<int> TryImportListOfProducts(XmlNode productList, bool isUpdateExisting)
        {
            if (productList == null)
                return 0;

            var newItems = new List<NewItemViewModel>();
            foreach (XmlNode product in productList.ChildNodes)
            {
                if (product != null)
                {
                    var newItemViewModel = TryReadingAttributesFrom(product);
                    newItems.Add(newItemViewModel);
                }
            }

            if (isUpdateExisting)
                return await itemsService.UpdateItemsNonNullValuesAsync(newItems);

            var allItems = await itemsService.GetAllAsync();
            var nonExistentNewItems = newItems
                .Where(item => allItems.All(i => i.ProductCode != item.ProductCode))
                .ToList();

            await itemsService.AddNewItemsAsync(nonExistentNewItems);

            return nonExistentNewItems.Count;

        }

        NewItemViewModel TryReadingAttributesFrom(XmlNode dataNode)
        {
            var newItem = new NewItemViewModel()
            {
                ItemDescriptions = new List<ItemDescription>()
            };

            foreach (XmlNode productAttribute in dataNode.ChildNodes)
            {
                TryItemListAttributeNames(newItem, productAttribute);
                TryPricesAvailabilityAttributeNames(newItem, productAttribute);
            }

            return newItem;
        }

        void TryItemListAttributeNames(NewItemViewModel newItem, XmlNode productAttribute)
        {
            // Based on itemList.xml
            if (productAttribute.Name == "ProductType")
                newItem.ShortDescription = productAttribute.InnerText;

            if (productAttribute.Name == "ProductCode")
                newItem.ProductCode = productAttribute.InnerText;

            if (productAttribute.Name == "ProductDescription")
                newItem.Name = productAttribute.InnerText;

            if (productAttribute.Name == "Image")
                newItem.ImageUrl = productAttribute.InnerText;

            if (productAttribute.Name == "Vendor")
                newItem.BrandNames.Add(productAttribute.InnerText);

            TryImportingAttributeList(newItem, productAttribute);
        }

        static void TryImportingAttributeList(NewItemViewModel newItem, XmlNode productAttribute)
        {
            if (productAttribute.Name == "AttrList")
            {
                foreach (XmlNode attribute in productAttribute.ChildNodes)
                {
                    var itemDescription = new ItemDescription()
                    {
                        Name = attribute.Attributes?[0].InnerText ?? "",
                        Description = attribute.Attributes?[1].InnerText ?? ""
                    };

                    newItem.ItemDescriptions.Add(itemDescription);
                }
            }
        }

        static void TryPricesAvailabilityAttributeNames(NewItemViewModel newItem, XmlNode productAttribute)
        {
            // Based on PricesAvail.xml
            if (productAttribute.Name == "WIC")
                newItem.ProductCode = productAttribute.InnerText;

            if (productAttribute.Name == "DESCRIPTION")
                newItem.Description = productAttribute.InnerText;

            if (productAttribute.Name == "RETAIL_PRICE")
            {
                if (decimal.TryParse(productAttribute.InnerText, out var decimalValue))
                    newItem.Price = decimalValue;
            }

            if (productAttribute.Name == "AVAIL")
            {
                if (int.TryParse(productAttribute.InnerText, out var intValue))
                    newItem.Availability = intValue;
            }
        }
    }
}