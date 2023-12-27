using System.Collections.Generic;
using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Models;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface IItemsService : IEntityBaseRepository<Item>
    {
        Task<Item> GetItemByIdAsync(int id);
        Task AddNewItemAsync(NewItemViewModel dataItem);
        Task AddNewItemsAsync(List<NewItemViewModel> dataList);
        Task UpdateItemAsync(NewItemViewModel dataItem);
        Task<int> UpdateItemsNonNullValuesAsync(List<NewItemViewModel> dataList);
    }
}
