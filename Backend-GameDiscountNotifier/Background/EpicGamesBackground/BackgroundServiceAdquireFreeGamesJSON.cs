using Backend_GameDiscountNotifier.Data;
using Backend_GameDiscountNotifier.Logic;
using Backend_GameDiscountNotifier.Model.Contet;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Backend_GameDiscountNotifier.Background.EpicGamesBackground
{
    public class BackgroundServiceAdquireFreeGamesJSON : BackgroundService
    {
        private string Url;
        private readonly HttpClient httpClient;
        private readonly IServiceScopeFactory scopeFactory;
        public BackgroundServiceAdquireFreeGamesJSON(HttpClient httpClient, IServiceScopeFactory scopeFactory)
        {
            this.httpClient = httpClient;
            this.scopeFactory = scopeFactory;
            this.Url = "https://store-site-backend-static-ipv4.ak.epicgames.com/freeGamesPromotions";
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<SellerJoc> sellers = new();
            List<Joc> jocs = new();
            List<JocEnPlataforma> jocEnPlataformas = new();
            List<Plataforma> plataformas = new();
            List<Oferta> ofertas = new();

            using IServiceScope scope = scopeFactory.CreateScope();
            using MariaDbContext context = scope.ServiceProvider.GetRequiredService<MariaDbContext>();
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
                    if (valor.GetProperty("promotions").ValueKind != JsonValueKind.Null)
                    {
                        var id = valor.GetProperty("id").ToString();
                        await logica(context, valor, plataformaEpic, "promotionalOffers", id);
                        await logica(context, valor, plataformaEpic, "upcomingPromotionalOffers", id);
                    }
                }

                //foreach (var element in ofertas)
                //{
                //    Console.WriteLine();
                //    Console.WriteLine(element);
                //    Console.WriteLine(element.JocPlatataforma.Joc);
                //}

                await Task.Delay(TimeSpan.FromSeconds(28800), stoppingToken);
            }
        }

        protected async Task logica(MariaDbContext context, JsonElement valor, Plataforma? plataformaEpic, string ofertaAnalitzar, string id)
        {
            const string PLATAFORMA = "Epic_Games";

            var promotions = valor.GetProperty("promotions").GetProperty(ofertaAnalitzar).EnumerateArray();

            foreach (var grups in promotions)
            {
                foreach (var element in grups.GetProperty("promotionalOffers").EnumerateArray())
                {
                    DateTimeOffset dataInici = EGFreeGamesBuilders.LogicaData(element, "startDate");
                    DateTimeOffset dataFi = EGFreeGamesBuilders.LogicaData(element, "endDate");
                    int discount = element.GetProperty("discountSetting").GetProperty("discountPercentage").GetInt32();

                    if (!context.Ofertas.Any(e =>
                            e.IdExtretOferta == id &&
                            e.DataIniciOferta == dataInici &&
                            e.DataFiOferta == dataFi
                    ))
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

                        Oferta ofertaTemp = EGFreeGamesBuilders.OfertaBuilder(valor, dataInici, dataFi, discount);
                        context.Ofertas.Add(ofertaTemp);

                        RelationsBuilder.RelationBuilder(jocTemp, jocEnPlataformaTemp, ofertaTemp, plataformaEpic, sellerJocTemp);


                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
