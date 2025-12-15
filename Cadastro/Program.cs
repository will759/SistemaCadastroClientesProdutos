using Cadastro.Infrastructure.Data.Common;
using Cadastro.Infrastructure.ExtensionMethods;
using Cadastro.Infrastructure.Web;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// As linhas using OfficeOpenXml e a configuração da licença foram removidas. (CONFIRMADO)

var builder = WebApplication.CreateBuilder(args);

// Configuração de Cultura para Português-Brasil (pt-BR) - MANTIDA
var defaultCulture = new CultureInfo("pt-BR");
var cultureInfo = new List<CultureInfo> { defaultCulture };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = cultureInfo;
    options.SupportedUICultures = cultureInfo;
});

// Adição de Repositórios e Serviços (Extension Methods)
builder.Services.AddRepositories().AddServices();

// Adição do AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapping));

// Configuração do DbContext (Usando In Memory Database)
builder.Services.AddDbContext<RegisterContext>(options =>
    options.UseInMemoryDatabase("Register"));

// Adição de Controllers e Views
builder.Services.AddControllersWithViews(options => 
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});


var app = builder.Build();

// Configuração do Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();