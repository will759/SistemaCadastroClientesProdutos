using Cadastro.ViewModels;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;

namespace Cadastro.Reports
{
    public class ClientsExcelReport
    {
        private readonly List<ClientViewModel> _clients;

        public ClientsExcelReport(List<ClientViewModel> clients)
        {
            _clients = clients;
        }

        public byte[] Generate()
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Clientes");

            // Cabeçalho
            ws.Cell(1, 1).Value = "Código";
            ws.Cell(1, 2).Value = "Nome";
            ws.Cell(1, 3).Value = "Sobrenome";
            ws.Cell(1, 4).Value = "Email";
            ws.Cell(1, 5).Value = "Ativo";

            var row = 2;
            foreach (var c in _clients)
            {
                ws.Cell(row, 1).Value = c.Id;
                ws.Cell(row, 2).Value = c.Name;
                ws.Cell(row, 3).Value = c.LastName;
                ws.Cell(row, 4).Value = c.Email;
                ws.Cell(row, 5).Value = c.Ative ? "Sim" : "Não";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
