using System.Collections.Generic;
using System.IO;
using System.Text;
using Cadastro.ViewModels;

namespace Cadastro.Reports
{
    public class DashboardExcelReport
    {
        private readonly List<DashboardReportViewModel> _data;

        public DashboardExcelReport(List<DashboardReportViewModel> data)
        {
            _data = data;
        }

        public byte[] Generate()
        {
            var sb = new StringBuilder();

            // Cabeçalho
            sb.AppendLine("Produto;Cliente;Valor;Disponível");

            // Dados
            foreach (var item in _data)
            {
                sb.AppendLine($"{item.Produto};{item.Cliente};{item.Valor.ToString("F2").Replace('.', ',')};{(item.Disponivel ? "Sim" : "Não")}");
            }

            // Converte para byte[]
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
