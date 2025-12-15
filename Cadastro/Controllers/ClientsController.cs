using Cadastro.Interfaces;
using Cadastro.ViewModels;
using Cadastro.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Cadastro.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientViewModelService _clientViewModelService;

        public ClientsController(IClientViewModelService clientViewModelService)
        {
            _clientViewModelService = clientViewModelService;
        }

        // GET: Clients
        public ActionResult Index(string search)
        {
            var list = _clientViewModelService.GetAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                list = list.Where(c =>
                    c.Name.ToLower().Contains(search) ||
                    c.LastName.ToLower().Contains(search) ||
                    c.Email.ToLower().Contains(search)
                );
            }

            return View(list);
        }

        // GET: Clients/Details/5
        public ActionResult Details(int id)
        {
            var viewModel = _clientViewModelService.Get(id);
            return View(viewModel);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            _clientViewModelService.Insert(viewModel);
            TempData["Success"] = "Cliente cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int id)
        {
            var viewModel = _clientViewModelService.Get(id);
            return View(viewModel);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ClientViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            _clientViewModelService.Update(viewModel);
            TempData["Success"] = "Cliente atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int id)
        {
            var viewModel = _clientViewModelService.Get(id);
            return View(viewModel);
        }

        // POST: Clients/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            _clientViewModelService.Delete(id);
            TempData["Success"] = "Cliente removido com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EXPORTAÇÕES
        // =========================

        // GET: Clients/ExportExcel
        public IActionResult ExportExcel()
        {
            var clients = _clientViewModelService.GetAll().ToList();

            var report = new ClientsExcelReport(clients);
            var file = report.Generate();

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Clientes.xlsx"
            );
        }

        // GET: Clients/ExportPdf
        public IActionResult ExportPdf()
        {
            var clients = _clientViewModelService.GetAll().ToList();

            var report = new ClientsPdfReport(clients);
            var file = report.Generate();

            return File(
                file,
                "application/pdf",
                "Clientes.pdf"
            );
        }
    }
}
