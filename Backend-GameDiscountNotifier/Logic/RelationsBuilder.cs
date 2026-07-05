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
            ////relacio joc-seller
            joc.Seller = sellerJoc;
            joc.IdSeller = sellerJoc.IdSeller;
            sellerJoc.Jocs.Add(joc);

            //relacio joc-jocenplataforma-plataforma
            jocEnPlataforma.Joc = joc;
            jocEnPlataforma.IdJoc = joc.IdSeller;
            jocEnPlataforma.Plataforma = plataforma;
            jocEnPlataforma.IdJocPlatataforma = plataforma.Id;
            joc.JocEnPlataformes.Add(jocEnPlataforma);
            plataforma.JocsPlataforma.Add(jocEnPlataforma);

            //relacio oferta-jocenplataforma
            oferta.JocPlatataforma = jocEnPlataforma;
            oferta.IdJocPlatataforma = jocEnPlataforma.IdJocPlatataforma;
            jocEnPlataforma.Ofertas.Add(oferta);
        }
    }
}
