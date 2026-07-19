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

                foreach (var valor in contingut.EnumerateArray())
                {
                    foreach (var valor2 in valor.GetProperty("promotions").EnumerateArray())
                    {
                        if (valor.GetProperty("promotions").ValueKind != JsonValueKind.Null)
                        {
                            DateTimeOffset datainicianalitzada = EGFreeGamesBuilders.LogicaData(valor, "startDate");
                            DateTimeOffset datafianalitzada = EGFreeGamesBuilders.LogicaData(valor, "endDate");
                            Oferta? oferta = context.Ofertas
                                .FirstOrDefault(e => e.IdExtretOferta == valor.GetProperty("id").ToString() &&
                                e.DataIniciOferta != datainicianalitzada ||
                                e.DataFiOferta != datafianalitzada &&
                                e.DataFiOferta >= DateTimeOffset.Now);

                            if (oferta != null)
                            {
                                oferta.DataIniciOferta = datainicianalitzada;
                                oferta.DataFiOferta = datafianalitzada;
                            }
                        }
                    }
                }

                await context.SaveChangesAsync();
                await Task.Delay(TimeSpan.FromSeconds(28800), stoppingToken);
            }
        }
    }
}
