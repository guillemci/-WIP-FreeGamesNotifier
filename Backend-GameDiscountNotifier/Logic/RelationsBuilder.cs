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
            //relacio joc-seller
            joc.Seller = sellerJoc;
            
            //relacio jocenplataforma-joc
            jocEnPlataforma.Joc = joc;

            //relacio jocenplataforma-plataforma
            jocEnPlataforma.Plataforma = plataforma;

            //relacio oferta-jocenplataforma
            oferta.JocPlatataforma = jocEnPlataforma;
        }
    }
}
