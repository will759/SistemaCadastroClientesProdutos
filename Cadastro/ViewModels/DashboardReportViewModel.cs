using System;

namespace Cadastro.ViewModels
{
    public class DashboardReportViewModel
    {
        public string Produto { get; set; }
        public string Cliente { get; set; }
        public decimal Valor { get; set; }
        public bool Disponivel { get; set; }
        public DateTime DataGeracao { get; set; }
    }
}
