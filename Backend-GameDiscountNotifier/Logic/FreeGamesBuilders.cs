using Backend_GameDiscountNotifier.Model.Contet;
using System.Text.Json;

namespace Backend_GameDiscountNotifier.Logic
{
    public class FreeGamesBuilders
    {
        //metodes que tindran logica de forma dividida de BackgroundServiceAdquireFreeGamesJSON
        public static void SellerBuilder(SellerJoc sellerJocTemp, JsonElement valor) { }
        public static void JocBuilder(Joc jocTemp, JsonElement valor) { }
        public static void JocEnPlataformaBuilder(JocEnPlataforma jocEnPlataformaTemp, JsonElement valor) { }
        public static void OfertaBuilder(Oferta ofertaTemp, JsonElement valor) { }

    }
}
