using Cadastro.Domain.Entities;
using System.Collections.Generic;

namespace Cadastro.ViewModels
{
    public class HomeViewModel
    {
        public int TotalClientes { get; set; }
        public int TotalProdutos { get; set; }

        public List<Client> UltimosClientes { get; set; }
        public List<Product> UltimosProdutos { get; set; }
    }
}
