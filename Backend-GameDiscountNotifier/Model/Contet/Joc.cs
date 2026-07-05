using System.Text.Json.Serialization;

namespace Backend_GameDiscountNotifier.Model.Contet
{
    public class Joc
    {
        public int IdJoc { get; set; }
        public string Title { get; set; }
        public int IdSeller { get; set; }
        public string Tipus { get; set; }
        [JsonIgnore]
        public SellerJoc Seller { get; set; }
        [JsonIgnore]
        public List<JocEnPlataforma> JocEnPlataformes { get; set; } = new();
        public override string ToString()
        {
            return SerialitzadorAmbConfig.SerializerAmbOpcioDeModel(this);
        }
    }
}
