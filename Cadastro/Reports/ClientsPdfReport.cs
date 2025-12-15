using Cadastro.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace Cadastro.Reports
{
    public class ClientsPdfReport
    {
        private readonly List<ClientViewModel> _clients;

        public ClientsPdfReport(List<ClientViewModel> clients)
        {
            _clients = clients;
        }

        public byte[] Generate()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Relatório de Clientes")
                        .FontSize(20)
                        .Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Código").Bold();
                            header.Cell().Text("Nome").Bold();
                            header.Cell().Text("Sobrenome").Bold();
                            header.Cell().Text("Email").Bold();
                            header.Cell().Text("Ativo").Bold();
                        });

                        foreach (var c in _clients)
                        {
                            table.Cell().Text(c.Id.ToString());
                            table.Cell().Text(c.Name);
                            table.Cell().Text(c.LastName);
                            table.Cell().Text(c.Email);
                            table.Cell().Text(c.Ative ? "Sim" : "Não");
                        }
                    });
                });
            }).GeneratePdf();
        }
    }
}
