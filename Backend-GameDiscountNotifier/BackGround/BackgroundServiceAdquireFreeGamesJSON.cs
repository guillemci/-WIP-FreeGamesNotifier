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
            List<SellerJoc> sellersjocs = new();
            List<Joc> jocs = new();
            List<JocEnPlataforma> jocsEnPlataformas = new();
            List<Oferta> ofertas = new();
            string elementEnllaç;
            string enllaç;
            int IdSellerProva = 1;
            int IdJocProva = 1;
            int IdJEPProva = 1;
            int Oferta = 1;

            //aixo ja estara creat a la bdd
            List<Plataforma> plataformas = new();
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
                        SellerJoc? sellerJocTemp = sellersjocs.FirstOrDefault(e => e.NomSeller == valor.GetProperty("seller").GetProperty("name").ToString());
                        Joc? jocTemp = jocs.FirstOrDefault(e => e.Title == valor.GetProperty("title").ToString());
                        JocEnPlataforma? jocEnPlataformaTemp = jocsEnPlataformas
                            .FirstOrDefault(e => e.Joc.Title == valor
                            .GetProperty("seller")
                            .GetProperty("name")
                            .ToString() && 
                            e.Plataforma.NomPlataforma == PLATAFORMA);

                        if (sellerJocTemp is null)
                        {
                            sellerJocTemp = EGFreeGamesBuilders.SellerBuilder(valor);
                            sellerJocTemp.IdSeller = IdSellerProva;
                            sellersjocs.Add(sellerJocTemp);
                        }

                        if (jocTemp is null)
                        {
                            jocTemp = EGFreeGamesBuilders.JocBuilder(valor);
                            jocTemp.IdJoc = IdJocProva;
                            jocs.Add(jocTemp);
                        }

                        if (jocEnPlataformaTemp is null)
                        {
                            jocEnPlataformaTemp = EGFreeGamesBuilders.JocEnPlataformaBuilder(valor);
                            jocEnPlataformaTemp.IdJocPlatataforma = IdJEPProva;
                            jocsEnPlataformas.Add(jocEnPlataformaTemp);
                        }

                        Oferta ofertaTemp = EGFreeGamesBuilders.OfertaBuilder(valor);
                        ofertas.Add(ofertaTemp);

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
