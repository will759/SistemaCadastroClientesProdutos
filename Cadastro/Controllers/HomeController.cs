using Cadastro.Infrastructure.Data.Common;
using Cadastro.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using ClosedXML.Excel; // Usando ClosedXML
using Microsoft.AspNetCore.Http;
using Cadastro.Reports; // Para DashboardPdfReport
using System.Threading.Tasks; // Necessário para async/await
using System.Globalization; // Necessário para CultureInfo

namespace Cadastro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RegisterContext _context;

        public HomeController(ILogger<HomeController> logger, RegisterContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                TotalClientes = _context.Clients.Count(),
                TotalProdutos = _context.Products.Count(),
                UltimosClientes = _context.Clients
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .ToList(),
                UltimosProdutos = _context.Products
                    .Include(p => p.Client)
                    .OrderByDescending(p => p.Id)
                    .Take(5)
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ImportData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ImportMessage"] = "Nenhum arquivo selecionado ou arquivo vazio.";
                return RedirectToAction("Index");
            }

            try
            {
                // Verifica a extensão do arquivo
                if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || file.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    await ProcessExcelImport(file);
                    TempData["ImportMessage"] = "Importação do Excel concluída com sucesso!";
                }
                else if (file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ImportMessage"] = "Formato PDF: Importação não implementada.";
                }
                else
                {
                    TempData["ImportMessage"] = $"Formato não suportado: {file.FileName}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na importação.");
                TempData["ImportMessage"] = $"Erro na importação: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        private async Task ProcessExcelImport(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            
            // Certifica que o ClosedXML lê do início do stream
            stream.Position = 0; 

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();
            // Acessa a última linha usada de forma segura
            var lastRow = worksheet.LastRowUsed();
            int rowCount = lastRow != null ? lastRow.RowNumber() : 1; 
            int successfulImports = 0;

            // Começa na linha 2 (após o cabeçalho)
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    // Usamos .Cell(row, col).GetText() para obter o valor como string, 
                    // para manter a lógica de validação de string
                    var productName = worksheet.Cell(row, 1).GetText()?.Trim();
                    var valueRaw = worksheet.Cell(row, 2).GetText();
                    var isAvailableRaw = worksheet.Cell(row, 3).GetText();
                    var clientIdRaw = worksheet.Cell(row, 4).GetText();

                    // Validação e Conversão de Valor
                    if (!decimal.TryParse(valueRaw?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal productValue))
                    {
                        _logger.LogWarning($"Linha {row} ignorada: Valor '{valueRaw}' inválido.");
                        continue;
                    }

                    // Validação e Conversão de ClientId
                    if (!int.TryParse(clientIdRaw, out int clientId))
                    {
                        _logger.LogWarning($"Linha {row} ignorada: ClientId '{clientIdRaw}' inválido.");
                        continue;
                    }

                    // Conversão de Disponível
                    bool isAvailable = !string.IsNullOrWhiteSpace(isAvailableRaw) &&
                                       isAvailableRaw.ToLowerInvariant() != "0" &&
                                       isAvailableRaw.ToLowerInvariant() != "não" &&
                                       isAvailableRaw.ToLowerInvariant() != "false";

                    var newProduct = new Cadastro.Domain.Entities.Product
                    {
                        Name = productName,
                        Value = productValue,
                        IsAvailable = isAvailable,
                        ClientId = clientId
                    };

                    _context.Products.Add(newProduct);
                    successfulImports++;
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, $"Erro na linha {row} do Excel.");
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"{successfulImports} produtos importados com sucesso.");
        }

        public IActionResult ExportExcel()
        {
            var data = _context.Products
                .Include(p => p.Client)
                .Select(p => new DashboardReportViewModel
                {
                    Produto = p.Name,
                    Cliente = p.Client.Name ?? "-", // Simplificado com operador de coalescência
                    Valor = p.Value,
                    Disponivel = p.IsAvailable
                })
                .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Produtos");

            // CABEÇALHO
            worksheet.Cell(1, 1).Value = "Produto";
            worksheet.Cell(1, 2).Value = "Cliente";
            worksheet.Cell(1, 3).Value = "Valor";
            worksheet.Cell(1, 4).Value = "Disponível";
            
            // Estilo do Cabeçalho (Opcional: Negrito e Fundo Cinza Claro)
            worksheet.Range("A1:D1").Style.Font.Bold = true;
            worksheet.Range("A1:D1").Style.Fill.BackgroundColor = XLColor.LightGray;


            // DADOS
            for (int i = 0; i < data.Count; i++)
            {
                int row = i + 2;
                worksheet.Cell(row, 1).Value = data[i].Produto;
                worksheet.Cell(row, 2).Value = data[i].Cliente;
                
                // Aplicando a formatação do valor
                worksheet.Cell(row, 3).Value = data[i].Valor;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "R$ #,##0.00"; 
                
                // Conversão de Booleano para String para leitura
                worksheet.Cell(row, 4).Value = data[i].Disponivel ? "Sim" : "Não";
            }
            
            // Ajuste automático das colunas
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var bytes = stream.ToArray();

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "dashboard-produtos.xlsx");
        }

        public IActionResult ExportPdf()
        {
            var data = _context.Products
                .Include(p => p.Client)
                .Select(p => new DashboardReportViewModel
                {
                    Produto = p.Name,
                    Cliente = p.Client.Name ?? "-", // Simplificado com operador de coalescência
                    Valor = p.Value,
                    Disponivel = p.IsAvailable
                })
                .ToList();

            var report = new DashboardPdfReport(data);
            var file = report.Generate();

            return File(file,
                "application/pdf",
                "dashboard-produtos.pdf");
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}