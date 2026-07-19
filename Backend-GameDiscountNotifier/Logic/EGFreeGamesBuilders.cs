using Backend_GameDiscountNotifier.Data;
using Backend_GameDiscountNotifier.Model.Contet;
using System.Text.Json;

namespace Backend_GameDiscountNotifier.Logic
{
    public class EGFreeGamesBuilders
    {
        //pasar els metodes voids perque tornin un objecte?
        public static SellerJoc SellerBuilder(JsonElement valor)
        {
            return new SellerJoc 
            { 
                NomSeller = valor.GetProperty("seller").GetProperty("name").ToString() 
            };
        }
        public static Joc JocBuilder(JsonElement valor)
        {
            return new Joc
            {
                Title = valor.GetProperty("title").ToString(),
                Tipus = valor.GetProperty("offerType").ToString()
            };
        }
        public static JocEnPlataforma JocEnPlataformaBuilder(JsonElement valor)
        {
            return new JocEnPlataforma
            {
                Desc = valor.GetProperty("description").ToString(),
                PreuOriginal = AconseguirPreu(valor),
                Enllaç = ExtreureEnllacTipusOferta(valor),
                ImatgeLink = valor.GetProperty("keyImages")
                .EnumerateArray()
                .First(e => e.GetProperty("type")
                .ToString() == "Thumbnail")
                .GetProperty("url")
                .ToString()
            };
        }

        public static Oferta OfertaBuilder(JsonElement fixaJoc, DateTimeOffset datainici, DateTimeOffset dataFi)
        {
            return new Oferta
            {
                IdExtretOferta = fixaJoc.GetProperty("id").ToString(),
                Descompte = 100,
                DataIniciOferta = datainici,
                DataFiOferta = dataFi,
                esGratis = true,
                PreuMomentOferta = AconseguirPreu(fixaJoc),
                DadesJsonOferta = fixaJoc.ToString()
            };
        }
        //public static List<Oferta> OfertaBuilder(JsonElement valor, string estatOferta, MariaDbContext context)
        //{
        //    List<Oferta> llista = new();

        //    var promotions = valor.GetProperty("promotions").GetProperty(estatOferta).GetProperty("promotionalOffers");
        //    var id = valor.GetProperty("id").ToString();

        //    foreach (var element in promotions.EnumerateArray())
        //        if (!context.Ofertas.Any(e => 
        //            e.DataIniciOferta != LogicaData(element, "startDate") && 
        //            e.DataFiOferta != LogicaData(element, "endDate") ||
        //            e.IdExtretOferta != id
        //        )) 

        //        llista.Add(new Oferta
        //        {
        //            IdExtretOferta = id,
        //            Descompte = 100,
        //            DataIniciOferta = LogicaData(element, "startDate"),
        //            DataFiOferta = LogicaData(element, "endDate"),
        //            esGratis = true,
        //            PreuMomentOferta = AconseguirPreu(valor),
        //            DadesJsonOferta = valor.ToString()
        //        });
        //    return llista;
        //}
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
            return DateTimeOffset.Parse(valor.GetProperty(tipusdedata).ToString());
        }

        //public static DateTimeOffset LogicaData(JsonElement valor, string tipusdedata)
        //{
            //var promotions = valor.GetProperty("promotions");

            //if (promotions.GetProperty("promotionalOffers").GetArrayLength() != 0)
            //    return DateTimeOffset.Parse(extreureData(valor, "promotionalOffers", "promotionalOffers", tipusdedata));
            //else if (promotions.GetProperty("upcomingPromotionalOffers").GetArrayLength() != 0)
            //    return DateTimeOffset.Parse(extreureData(valor, "upcomingPromotionalOffers", "promotionalOffers", tipusdedata));
            //else
            //    throw new Exception();
        }
        //public static string extreureData(JsonElement valor, string arrel1, string arrel2, string arrel3Definit)
        //{
        //    const string PROMOTIONS = "promotions";

        //    return valor
        //        .GetProperty(PROMOTIONS)
        //        .GetProperty(arrel1)[0]
        //        .GetProperty(arrel2)[0]
        //        .GetProperty(arrel3Definit)
        //        .ToString();
        //}
    }
}