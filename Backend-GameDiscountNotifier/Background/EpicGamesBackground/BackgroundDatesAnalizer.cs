using Backend_GameDiscountNotifier.Data;
using Backend_GameDiscountNotifier.Logic;
using Backend_GameDiscountNotifier.Model.Contet;
using System.Text.Json;

namespace Backend_GameDiscountNotifier.Background.EpicGamesBackground
{
    public class BackgroundDatesAnalizer : BackgroundService
    {
        private string Url;
        private readonly IServiceScopeFactory ScopeFactory;
        private readonly HttpClient httpClient;
        public BackgroundDatesAnalizer(HttpClient httpClient, IServiceScopeFactory serviceScopeFactory)
        {
            this.ScopeFactory = serviceScopeFactory;
            this.httpClient = httpClient;
            this.Url = "https://store-site-backend-static-ipv4.ak.epicgames.com/freeGamesPromotions";
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using IServiceScope service = ScopeFactory.CreateScope();
                using MariaDbContext context = service.ServiceProvider.GetRequiredService<MariaDbContext>();

                var resposta = await httpClient.GetStringAsync(Url);
                JsonDocument document = JsonDocument.Parse(resposta);

                var contingut = document.RootElement
                    .GetProperty("data")
                    .GetProperty("Catalog")
                    .GetProperty("searchStore")
                    .GetProperty("elements");

                IEnumerable<Oferta> ofertasFiltrades = context.Ofertas.Where(e => e.EstaActiva).ToList();

                foreach (var valor in contingut.EnumerateArray())
                {    
                    if (valor.GetProperty("promotions").ValueKind != JsonValueKind.Null)
                    {
                        var id = valor.GetProperty("id").ToString();
                        await logica(ofertasFiltrades, valor, id);
                    }
                }

                await context.SaveChangesAsync();
                await Task.Delay(TimeSpan.FromSeconds(28800), stoppingToken);
            }
        }

        //podria intentar fer moltes d'aquestes comprovacions amb un hashset, ara mateix tinc el cap reventat, nose ni el codi que escric
        //veig la possibilitat d'ajuntar abans els 2 tipos de promocions en una sola llista amb addrange
        //tant promotionalOffers com upcomingPromotionalOffers
        //pero potser hi ha alguna manera millor de fer-ho, de moment o fare aixi, vui acabar aquest projecte
        public async Task logica(IEnumerable<Oferta> ofertasFiltrades, JsonElement valor, string id)
        {
            var promotions = AjuntarLlistas(valor);

            foreach (var grups in promotions)
            {
                var ofertasJson = grups.GetProperty("promotionalOffers").EnumerateArray().GetEnumerator();
                RecorrerOfertas(ofertasFiltrades, ofertasJson, id);
            }
        }

        public static List<JsonElement> AjuntarLlistas(JsonElement valor)
        {
            var promotions = valor.GetProperty("promotions").GetProperty("promotionalOffers").EnumerateArray().ToList();
            promotions.AddRange(valor.GetProperty("promotions").GetProperty("upcomingPromotionalOffers").EnumerateArray().ToList());

            return promotions;
        }

        public static void RecorrerOfertas(IEnumerable<Oferta> ofertasFiltrades, IEnumerator<JsonElement> ofertasJson, string id)
        {

            foreach (var oferta in ofertasFiltrades)
            {
                bool trovat = true;

                while (ofertasJson.MoveNext() && trovat)
                {
                    if (TrovatEnElJsonEpic(oferta, ofertasJson, id))
                    {
                        trovat = false;
                        oferta.EstaActiva = trovat;
                    }
                }

                //segons ChatGpt IEnumerator<JsonElement>, no dona suport a Reset, potser Pasar IEnumerable i conseguir el Enumerator en bucle funciona
                //o usar foreach amb break... NO OK
                ofertasJson.Reset();
            }
        }

        //potser convertir en LAMBDA/DELEGATE
        public static bool TrovatEnElJsonEpic(Oferta oferta, IEnumerator<JsonElement> ofertasJson, string id)
        {
            return 
                oferta.DataFiOferta == DateTimeOffset.Parse(ofertasJson.Current.GetProperty("endDate").ToString()) && 
                oferta.DataIniciOferta == DateTimeOffset.Parse(ofertasJson.Current.GetProperty("startDate").ToString()) && 
                id == oferta.IdExtretOferta;
        }
    }
}
