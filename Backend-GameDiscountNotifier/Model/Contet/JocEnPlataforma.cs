using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend_GameDiscountNotifier.Model.Contet
{
    public class JocEnPlataforma
    {
        public int IdJocPlatataforma { get; set; }
        public int IdJoc { get; set; }
        public Joc Joc { get; set; }
        public int IdPlataforma { get; set; }
        public Plataforma Plataforma { get; set; }
        public string Desc { get; set; }
        public string Enllaç { get; set; }
        public decimal PreuOriginal { get; set; }
        public string ImatgeLink { get; set; }
        public List<Oferta> Ofertas { get; set; } = new();
        public override string ToString()
        {
            return SerialitzadorAmbConfig.SerializerAmbOpcioDeModel(this);
        }
    }
}