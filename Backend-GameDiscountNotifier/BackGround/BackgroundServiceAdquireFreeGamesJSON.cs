using Backend_GameDiscountNotifier.Data;
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
                    if (valor.GetProperty("promotions").ValueKind != JsonValueKind.Null)
                    {
                        SellerJoc sellerJocTemp = new();
                        Joc jocTemp = new();
                        JocEnPlataforma jocEnPlataformaTemp = new();
                        Oferta ofertaTemp = new();
                        var tipusOferta = valor.GetProperty("offerType").ToString();

                        //seller
                        sellerJocTemp.IdSeller = IdSellerProva;
                        sellerJocTemp.NomSeller = valor.GetProperty("seller").GetProperty("name").ToString();

                        //joc
                        jocTemp.IdJoc = IdJocProva;
                        jocTemp.Title = valor.GetProperty("title").ToString();
                        jocTemp.Tipus = tipusOferta;

                        //jocenplataforma
                        jocEnPlataformaTemp.IdJocPlatataforma = IdJEPProva;
                        jocEnPlataformaTemp.Desc = valor.GetProperty("description").ToString();

                        //jocenplataforma enllaç
                        try
                        {
                            if (tipusOferta == "BASE_GAME")
                            {
                                //anar amb cuidado amb aquest [0], algo em diu que epic no te un array d'objectes alla, per no fer res
                                var fragmentOferta = valor.GetProperty("catalogNs").GetProperty("mappings")[0].GetProperty("pageSlug").ToString();
                                jocEnPlataformaTemp.Enllaç = $"https://store.epicgames.com/p/{fragmentOferta}";
                            }
                            else if (tipusOferta == "ADD_ON")
                            {
                                //anar amb cuidado amb aquest [0], algo em diu que epic no te un array d'objectes alla, per no fer res
                                var fragmentOferta = valor.GetProperty("offerMappings")[0].GetProperty("pageSlug").ToString();
                                jocEnPlataformaTemp.Enllaç = $"https://store.epicgames.com/p/{fragmentOferta}";
                            }
                            else if (tipusOferta == "BUNDLE")
                            {
                                //potser fer peticio amb un webclient per veure que realment urlslug sempre furuli?, de moment no ja que confio amb les meves estimacions.
                                var fragmentOferta = valor.GetProperty("urlSlug").ToString();
                                jocEnPlataformaTemp.Enllaç = $"https://store.epicgames.com/bundles/{fragmentOferta}";
                            }
                            else
                            {
                                throw new Exception();
                                //en comptes de tirar excepcio, podriam provar de quedar-nos amb
                                //per exemple urlslug i catalogns, i si despres de montar i fer peticions no va tirar excepcio...
                            }

                        }
                        catch
                        {
                            Console.WriteLine($"el tipus {tipusOferta} en la plataforma: X, no esta gestionat, si no es aixo, espavila");
                        }

                        var text = valor.GetProperty("price").GetProperty("totalPrice").GetProperty("fmtPrice").GetProperty("originalPrice").ToString();
                        var preu = Convert.ToDecimal(text.Replace("$", "").Trim());
                        jocEnPlataformaTemp.PreuOriginal = preu;

                        //jocEnPlataformaTemp.ImatgeLink
                        jocEnPlataformaTemp.ImatgeLink = valor.GetProperty("keyImages")
                            .EnumerateArray()
                            .First(e => e.GetProperty("type")
                            .ToString() == "Thumbnail")
                            .GetProperty("url")
                            .ToString();

                        //DadesJson
                        ofertaTemp.DadesJsonOferta = valor.ToString();

                        //oferta
                        ofertaTemp.IdExtretOferta = valor.GetProperty("id").ToString();
                        ofertaTemp.Descompte = 100;

                        var promotions = valor.GetProperty("promotions");

                        try
                        {
                            if (promotions.GetProperty("promotionalOffers").GetArrayLength() != 0)
                            {
                                ofertaTemp.DataIniciOferta = DateTimeOffset.Parse(promotions
                                    .GetProperty("promotionalOffers")[0]
                                    .GetProperty("promotionalOffers")[0]
                                    .GetProperty("startDate")
                                    .ToString());

                                ofertaTemp.DataFiOferta = DateTimeOffset.Parse(promotions
                                    .GetProperty("promotionalOffers")[0]
                                    .GetProperty("promotionalOffers")[0]
                                    .GetProperty("endDate")
                                    .ToString());
                            }
                            else if (promotions.GetProperty("upcomingPromotionalOffers").GetArrayLength() != 0)
                            {
                                ofertaTemp.DataIniciOferta = DateTimeOffset.Parse(promotions
                                    .GetProperty("upcomingPromotionalOffers")[0]
                                    .GetProperty("promotionalOffers")[0]
                                    .GetProperty("startDate")
                                    .ToString());

                                ofertaTemp.DataFiOferta = DateTimeOffset.Parse(promotions
                                    .GetProperty("upcomingPromotionalOffers")[0]
                                    .GetProperty("promotionalOffers")[0]
                                    .GetProperty("endDate")
                                    .ToString());
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            Console.WriteLine("VINGA A IMPLEMENTAR TRYX LETSGOO!!!");
                        }

                        ofertaTemp.esGratis = true;

                        ofertaTemp.PreuMomentOferta = preu;


                        ////relacions joc-seller
                        //jocTemp.IdSeller = sellerJocTemp.IdSeller;
                        //jocTemp.Seller = 
                        //sellerJocTemp.Jocs.Add(jocTemp);



                        ////relacio joc-jocenplataforma
                        //jocEnPlataformaTemp.IdJoc = jocTemp.IdJoc;

                        ////relacio plataforma-jocenplataforma

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
