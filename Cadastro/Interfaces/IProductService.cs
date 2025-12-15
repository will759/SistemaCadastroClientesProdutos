using Cadastro.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cadastro.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        
        Task<Product> GetByIdAsync(int id);
        
        Task AddAsync(Product product);
        
        Task UpdateAsync(Product product);
        
        Task DeleteAsync(int id);
        
        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
}