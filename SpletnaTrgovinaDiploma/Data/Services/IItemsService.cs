using SpletnaTrgovinaDiploma.Data.Base;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;
using System.Threading.Tasks;

namespace SpletnaTrgovinaDiploma.Data.Services
{
    public interface IItemsService : IEntityBaseRepository<Item>
    {
        Task<Item> GetItemByIdAsync(int id);
        Task<NewItemDropdownsViewModel> GetNewItemDropdownsValues();
        Task AddNewItemAsync(NewItemViewModel data);
        Task UpdateItemAsync(NewItemViewModel data);
    }
}
