using Backend_GameDiscountNotifier.Data;
using Backend_GameDiscountNotifier.Logic;
using Backend_GameDiscountNotifier.Model.Contet;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Backend_GameDiscountNotifier.BackGround
{
    public class BackgroundServiceAdquireFreeGamesJSON : BackgroundService
    {
        private string Url;
        private readonly HttpClient httpClient;
        //private readonly MariaDbContext mariaDbContext;
        public BackgroundServiceAdquireFreeGamesJSON(HttpClient httpClient /*, MariaDbContext mariaDbContext*/)
        {

            this.httpClient = httpClient;
            //this.mariaDbContext = mariaDbContext;
            this.Url = "https://store-site-backend-static-ipv4.ak.epicgames.com/freeGamesPromotions";
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string PLATAFORMA = "EpicGames";
            List<SellerJoc> sellerjocs = new();
            List<Joc> jocs = new();
            List<JocEnPlataforma> jocsEnPlataformas = new();
            List<Oferta> ofertas = new();
            string elementEnllaç;
            string enllaç;
            int IdSellerProva = 1;
            int IdJocProva = 1;
            int IdJEPProva = 1;
            int Oferta = 1;

            List<Plataforma> plataformas = new();

            //aixo ja estara creat a la bdd
            plataformas.Add(new Plataforma
            {
                Id = 1,
                NomPlataforma = "Epic Games"
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                var resposta = await httpClient.GetStringAsync(Url);
                JsonDocument document = JsonDocument.Parse(resposta);

                var contingut = document.RootElement.GetProperty("data").GetProperty("Catalog").GetProperty("searchStore").GetProperty("elements");

                //la majoria del codi sera refactoritzat en metodes mes petits per una millor llegibilitat
                foreach (var valor in contingut.EnumerateArray())
                {
                    if 
                    (
                        valor.GetProperty("promotions").ValueKind != JsonValueKind.Null || 
                        ofertas.Any(e => e.IdExtretOferta == valor.GetProperty("id").ToString())
                    )
                    {
                        SellerJoc sellerJocTemp = new();
                        Joc jocTemp = new();
                        JocEnPlataforma jocEnPlataformaTemp = new();
                        Oferta ofertaTemp = new();

                        //seller
                        sellerJocTemp.IdSeller = IdSellerProva;
                        FreeGamesBuilders.SellerBuilder(sellerJocTemp, valor);

                        //joc
                        jocTemp.IdJoc = IdJocProva;
                        FreeGamesBuilders.JocBuilder(jocTemp, valor);

                        //jocenplataforma
                        jocEnPlataformaTemp.IdJocPlatataforma = IdJEPProva;
                        FreeGamesBuilders.JocEnPlataformaBuilder(jocEnPlataformaTemp, valor);

                        //oferta
                        FreeGamesBuilders.OfertaBuilder(ofertaTemp, valor);

                        Console.WriteLine($"iteracio {IdSellerProva}");
                        Console.WriteLine(sellerJocTemp.ToString());
                        Console.WriteLine();
                        Console.WriteLine(jocTemp.ToString());
                        Console.WriteLine();
                        Console.WriteLine(jocEnPlataformaTemp.ToString());
                        Console.WriteLine();
                        Console.WriteLine(ofertaTemp.ToString());
                        Console.WriteLine();

                        IdSellerProva++;
                        IdJocProva++;
                        IdJEPProva++;
                        Oferta++;
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(1040), stoppingToken);
            }
        }
    }
}
