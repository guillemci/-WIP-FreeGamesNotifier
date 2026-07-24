using Backend_GameDiscountNotifier.Data;
using Backend_GameDiscountNotifier.Logic;
using Backend_GameDiscountNotifier.Model.Contet;
using Swashbuckle.AspNetCore.SwaggerGen;
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

                Dictionary<(string Id, DateTimeOffset DataInici, DateTimeOffset DataFi), Oferta> 
                    ofertasFiltrades = context.Ofertas
                    .Where(e => e.EstaActiva)
                    .ToDictionary(e => (e.IdExtretOferta, e.DataIniciOferta, e.DataFiOferta));

                foreach (var oferta in ofertasFiltrades)
                    oferta.Value.EstaActiva = false;

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

        public async Task logica(Dictionary<(string Id, DateTimeOffset DataInici, DateTimeOffset DataFi), Oferta> ofertasFiltrades, JsonElement valor, string id)
        {
            var ofertes = AjuntarLlistas(valor)
                .SelectMany(grup => grup.GetProperty("promotionalOffers").EnumerateArray());

            foreach (var oferta in ofertes)
            {
                var datafi = DateTimeOffset.Parse(oferta.GetProperty("endDate").ToString());
                var datainici = DateTimeOffset.Parse(oferta.GetProperty("startDate").ToString());
                var clau = (id, datainici, datafi);

                if (ofertasFiltrades.TryGetValue(clau, out var ofertaBD))
                    ofertasFiltrades[(id, datainici, datafi)].EstaActiva = true;
            }
        }

        public static List<JsonElement> AjuntarLlistas(JsonElement valor)
        {
            var promotions = valor.GetProperty("promotions").GetProperty("promotionalOffers").EnumerateArray().ToList();
            promotions.AddRange(valor.GetProperty("promotions").GetProperty("upcomingPromotionalOffers").EnumerateArray().ToList());

            return promotions;
        }
    }
}
