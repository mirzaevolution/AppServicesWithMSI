using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppServicesDemo.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.AspNetCore.Hosting;

namespace AppServicesDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hosting;
        private static string _secretIdentifier = "https://centralizekv-mgr.vault.azure.net/secrets/MyConfig/a299e2cd0795452abcdf3faccd250a1f";
        public HomeController(IConfiguration config, IHostingEnvironment hosting)
        {
            _config = config;
            _hosting = hosting;
        }
        public async Task<IActionResult> Index()
        {
            //ViewBag.AppName = _config["ApplicationName"];
            //ViewBag.Environment = _config["Stamp:Environment"];
            //ViewBag.Version = _config["Stamp:Version"];

            ViewBag.AppName = Environment.GetEnvironmentVariable("ApplicationName");
            ViewBag.Environment = Environment.GetEnvironmentVariable("Stamp:Environment");
            ViewBag.Version = Environment.GetEnvironmentVariable("Stamp:Version");
            ViewBag.HostingEnvironment = _hosting.EnvironmentName;
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            if (_hosting.IsDevelopment())
            {
                azureServiceTokenProvider = new AzureServiceTokenProvider(
                        connectionString: "RunAs=App;AppId=ffde762d-e157-4993-af81-5c338702d7c9;TenantId=b4a0591f-30ee-4771-b588-886184518ec5;AppKey=.yEnc_181pinZ7zoOdLFM.poH_1ho.~G-q"
                    );
            }



            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            SecretBundle secretBundle = await keyVaultClient.GetSecretAsync(_secretIdentifier);
            if (secretBundle != null)
            {
                ViewBag.Secret = secretBundle.Value;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
