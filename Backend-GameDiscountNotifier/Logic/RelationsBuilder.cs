using Backend_GameDiscountNotifier.Model.Contet;

namespace Backend_GameDiscountNotifier.Logic
{
    public class RelationsBuilder
    {
        //he d'acabar de definir relacions, hi ha moltes que no cal que tinguin bidireccionalitat tant definida
        public static void RelationBuilder
        (
            Joc joc,
            JocEnPlataforma jocEnPlataforma,
            Oferta oferta,
            Plataforma plataforma,
            SellerJoc sellerJoc
        )
        {
            joc.Seller = sellerJoc;

            jocEnPlataforma.Joc = joc;
            jocEnPlataforma.Plataforma = plataforma;

            oferta.JocPlatataforma = jocEnPlataforma;
        }
    }
}
