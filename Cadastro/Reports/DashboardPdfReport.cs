using System.Collections.Generic;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Cadastro.ViewModels;
using QuestPDF.Helpers;
using System.Linq;
using System.Globalization; // Para CultureInfo

namespace Cadastro.Reports
{
    public class DashboardPdfReport
    {
        private readonly List<DashboardReportViewModel> _data;

        public DashboardPdfReport(List<DashboardReportViewModel> data)
        {
            _data = data;
        }

        public byte[] Generate()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            using var stream = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    
                    page.Header()
                        .Text("Dashboard de Produtos")
                        .SemiBold().FontSize(20);
                    
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        // Cabeçalho (AGORA CORRIGIDO: Usando .Background() para aplicar a cor de fundo à célula)
                        table.Header(header =>
                        {
                            // APLICANDO BACKGROUND DIRETAMENTE À CÉLULA
                            header.Cell().Padding(5).Background(Colors.Grey.Lighten3).Text("Produto").Bold().FontSize(10);
                            header.Cell().Padding(5).Background(Colors.Grey.Lighten3).Text("Cliente").Bold().FontSize(10);
                            header.Cell().Padding(5).Background(Colors.Grey.Lighten3).Text("Valor").Bold().FontSize(10);
                            header.Cell().Padding(5).Background(Colors.Grey.Lighten3).Text("Disponível").Bold().FontSize(10);
                        });

                        // Linhas
                        if (_data != null && _data.Any())
                        {
                             foreach (var item in _data)
                            {
                                // APLICANDO BORDAS AO CONTÊINER DA CÉLULA (Opcional, mas melhora a visualização)
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Produto);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Cliente);
                                
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                    .Text(item.Valor.ToString("C2", new CultureInfo("pt-BR"))).AlignRight(); 
                                    
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                    .Text(item.Disponivel ? "Sim" : "Não").AlignCenter();
                            }
                        }
                        else
                        {
                            table.Cell().ColumnSpan(4).Padding(10).Text("Nenhum dado de produto encontrado para o relatório.").Italic().FontColor(Colors.Grey.Medium);
                        }
                    });
                    
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf(stream);

            // Rebobinar o stream após a escrita
            stream.Position = 0; 

            return stream.ToArray();
        }
    }
}