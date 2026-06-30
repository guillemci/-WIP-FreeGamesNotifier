
using Backend_GameDiscountNotifier.Data;
using Backend_GameDiscountNotifier.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Backend_GameDiscountNotifier
{
    public class BackgroundServiceAdquireJSON : BackgroundService
    {
        private string Url;
        private readonly HttpClient httpClient;
        //private readonly MariaDbContext mariaDbContext;
        public BackgroundServiceAdquireJSON(HttpClient httpClient /*, MariaDbContext mariaDbContext*/)
        {
            
            this.httpClient = httpClient;
            //this.mariaDbContext = mariaDbContext;
            this.Url = "https://store-site-backend-static-ipv4.ak.epicgames.com/freeGamesPromotions";
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Joc> jocs = new();
            List<SellerJoc> autorjocs = new();
            List<JocEnPlataforma> jocsEnPlataforma = new();
            List<Oferta> ofertas = new();

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var resposta = await httpClient.GetStringAsync(Url);
                    JsonDocument document = JsonDocument.Parse(resposta);

                    var contingut = document.RootElement.GetProperty("data").GetProperty("Catalog").GetProperty("searchStore").GetProperty("elements");

                    int IdSellerProva = 1;
                    int IdJocProva = 1;
                    int IdJEPProva = 1;
                    int Oferta = 1;
                    const string PLATAFORMA = "EpicGames";
                    string elementEnllaç;
                    string enllaç;

                    List<Plataforma> plataformas = new();


                    //aixo ja estara creat a la bdd
                    plataformas.Add(new Plataforma
                    {
                        Id = 1,
                        NomPlataforma = "Epic Games"
                    });

                    foreach (var valor in contingut.EnumerateArray())
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

                        //jocenplataforma (epic games)
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
                        text = text.Replace("$", "").Trim();
                        jocEnPlataformaTemp.PreuOriginal = Convert.ToDecimal(text);

                        //relacions joc-seller
                        jocTemp.IdSeller = sellerJocTemp.IdSeller;
                        sellerJocTemp.Jocs.Add(jocTemp);

                        //relacio joc-jocenplataforma

                        //relacio plataforma-jocenplataforma

                        IdSellerProva++;
                        IdJocProva++;
                        IdJEPProva++;
                        Oferta++;

                        Console.WriteLine(jocEnPlataformaTemp.ToString());
                    }



                    await Task.Delay(TimeSpan.FromSeconds(8), stoppingToken);
                }
            }
            catch
            {

            }
        }
    }
}
