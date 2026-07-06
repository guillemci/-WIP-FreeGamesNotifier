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
        private readonly IServiceScopeFactory scopeFactory;
        public BackgroundServiceAdquireFreeGamesJSON(HttpClient httpClient , IServiceScopeFactory scopeFactory)
        {

            this.httpClient = httpClient;
            this.scopeFactory = scopeFactory;
            this.Url = "https://store-site-backend-static-ipv4.ak.epicgames.com/freeGamesPromotions";
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string PLATAFORMA = "Epic_Games";

            using IServiceScope scope = scopeFactory.CreateScope();
            MariaDbContext context = scope.ServiceProvider.GetRequiredService<MariaDbContext>();
            //aixo ja estara creat a la bdd
            Plataforma? plataformaEpic = context.Plataformes.FirstOrDefault(e => e.NomPlataforma == "Epic_Games");
            Console.WriteLine(plataformaEpic);

            while (!stoppingToken.IsCancellationRequested)
            {
                var resposta = await httpClient.GetStringAsync(Url);
                JsonDocument document = JsonDocument.Parse(resposta);

                var contingut = document.RootElement
                    .GetProperty("data")
                    .GetProperty("Catalog")
                    .GetProperty("searchStore")
                    .GetProperty("elements");

                //la majoria del codi sera refactoritzat en metodes mes petits per una millor llegibilitat
                foreach (var valor in contingut.EnumerateArray())
                {
                    if
                    (
                        valor.GetProperty("promotions").ValueKind != JsonValueKind.Null &&
                        context.Ofertas.All(e => e.IdExtretOferta != valor.GetProperty("id").ToString())
                    )
                    {
                        SellerJoc? sellerJocTemp = context.SellersJocs.FirstOrDefault(e => e.NomSeller == valor.GetProperty("seller").GetProperty("name").ToString());
                        Joc? jocTemp = context.Jocs.FirstOrDefault(e => e.Title == valor.GetProperty("title").ToString());
                        JocEnPlataforma? jocEnPlataformaTemp = context.JocsEnPlataformes
                            .FirstOrDefault(e =>
                                e.Joc != null &&
                                e.Plataforma != null &&
                                e.Joc.Title == valor.GetProperty("title").ToString() &&
                                e.Plataforma.NomPlataforma == PLATAFORMA);

                        if (sellerJocTemp is null)
                        {
                            sellerJocTemp = EGFreeGamesBuilders.SellerBuilder(valor);
                            context.SellersJocs.Add(sellerJocTemp);
                        }

                        if (jocTemp is null)
                        {
                            jocTemp = EGFreeGamesBuilders.JocBuilder(valor);
                            context.Jocs.Add(jocTemp);
                        }

                        if (jocEnPlataformaTemp is null)
                        {
                            jocEnPlataformaTemp = EGFreeGamesBuilders.JocEnPlataformaBuilder(valor);
                            context.JocsEnPlataformes.Add(jocEnPlataformaTemp);
                        }

                        Oferta ofertaTemp = EGFreeGamesBuilders.OfertaBuilder(valor);
                        context.Ofertas.Add(ofertaTemp);

                        RelationsBuilder.RelationBuilder(jocTemp, jocEnPlataformaTemp, ofertaTemp, plataformaEpic, sellerJocTemp);

                        await context.SaveChangesAsync();
                    }
                }

                foreach (var element in context.Jocs)
                    Console.WriteLine(element);

                await Task.Delay(TimeSpan.FromSeconds(50), stoppingToken);
            }
        }
    }
}
