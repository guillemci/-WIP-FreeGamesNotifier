using Backend_GameDiscountNotifier.Model.Contet;
using System.Text.Json;

namespace Backend_GameDiscountNotifier.Logic
{
    public class FreeGamesBuilders
    {
        //pasar els metodes voids perque tornin un objecte?
        public static void SellerBuilder(SellerJoc sellerJocTemp, JsonElement valor)
        {
            sellerJocTemp.NomSeller = valor.GetProperty("seller").GetProperty("name").ToString();
        }
        public static void JocBuilder(Joc jocTemp, JsonElement valor)
        {
            jocTemp.Title = valor.GetProperty("title").ToString();
            jocTemp.Tipus = valor.GetProperty("offerType").ToString();
        }
        public static void JocEnPlataformaBuilder(JocEnPlataforma jocEnPlataformaTemp, JsonElement valor)
        {
            jocEnPlataformaTemp.Desc = valor.GetProperty("description").ToString();
            jocEnPlataformaTemp.PreuOriginal = AconseguirPreu(valor);
            jocEnPlataformaTemp.Enllaç = ExtreureEnllacTipusOferta(valor);
            jocEnPlataformaTemp.ImatgeLink = valor.GetProperty("keyImages")
                .EnumerateArray()
                .First(e => e.GetProperty("type")
                .ToString() == "Thumbnail")
                .GetProperty("url")
                .ToString();
        }
        public static void OfertaBuilder(Oferta ofertaTemp, JsonElement valor)
        {
            ofertaTemp.IdExtretOferta = valor.GetProperty("id").ToString();
            ofertaTemp.Descompte = 100;
            ofertaTemp.DataIniciOferta = LogicaData(valor, "startDate");
            ofertaTemp.DataFiOferta = LogicaData(valor, "endDate");
            ofertaTemp.esGratis = true;
            ofertaTemp.PreuMomentOferta = AconseguirPreu(valor);
            ofertaTemp.DadesJsonOferta = valor.ToString();
        }
        public static decimal AconseguirPreu(JsonElement valor)
        {
            var text = valor.GetProperty("price")
                .GetProperty("totalPrice")
                .GetProperty("fmtPrice")
                .GetProperty("originalPrice")
                .ToString();

            return Convert.ToDecimal(text.Replace("$", "").Trim());
        }
        public static string ExtreureEnllacTipusOferta(JsonElement valor)
        {
            var tipusOferta = valor.GetProperty("offerType").ToString();

            if (tipusOferta == "BASE_GAME")
            {
                //anar amb cuidado amb aquest [0], algo em diu que epic no te un array d'objectes alla, per no fer res
                var fragmentOferta = valor.GetProperty("catalogNs")
                    .GetProperty("mappings")[0]
                    .GetProperty("pageSlug")
                    .ToString();

                return $"https://store.epicgames.com/p/{fragmentOferta}";
            }
            else if (tipusOferta == "ADD_ON")
            {
                //anar amb cuidado amb aquest [0], algo em diu que epic no te un array d'objectes alla, per no fer res
                var fragmentOferta = valor
                    .GetProperty("offerMappings")[0]
                    .GetProperty("pageSlug")
                    .ToString();

                return $"https://store.epicgames.com/p/{fragmentOferta}";
            }
            else if (tipusOferta == "BUNDLE")
            {
                //potser fer peticio amb un webclient per veure que realment urlslug sempre furuli?, de moment no ja que confio amb les meves estimacions.
                var fragmentOferta = valor.GetProperty("urlSlug").ToString();
                return $"https://store.epicgames.com/bundles/{fragmentOferta}";
            }
            else
            {
                throw new Exception();
                //en comptes de tirar excepcio, podriam provar de quedar-nos amb
                //per exemple urlslug i catalogns, i si despres de montar i fer peticions no va tirar excepcio...
            }
        }
        public static DateTimeOffset LogicaData(JsonElement valor, string tipusdedata)
        {
            var promotions = valor.GetProperty("promotions");

            if (promotions.GetProperty("promotionalOffers").GetArrayLength() != 0)
            {
                return DateTimeOffset.Parse(extreureData(valor, "promotionalOffers", "promotionalOffers", tipusdedata));
            }
            else if (promotions.GetProperty("upcomingPromotionalOffers").GetArrayLength() != 0)
            {
                return DateTimeOffset.Parse(extreureData(valor, "upcomingPromotionalOffers", "promotionalOffers", tipusdedata));
            }
            else
            {
                throw new Exception();
            }
        }
        public static string extreureData(JsonElement valor, string arrel1, string arrel2, string arrel3Definit)
        {
            const string PROMOTIONS = "promotions";

            return valor
                .GetProperty(PROMOTIONS)
                .GetProperty(arrel1)[0]
                .GetProperty(arrel2)[0]
                .GetProperty(arrel3Definit)
                .ToString();
        }
        //public static void EpicGameMapper
        //(
        //    SellerJoc sellerJocTemp, 
        //    Joc jocTemp, 
        //    JocEnPlataforma jocEnPlataformaTemp, 
        //    Oferta ofertaTemp, 
        //    JsonElement valor) {}
    }
}