using System.Text.Json.Serialization;

namespace Backend_GameDiscountNotifier.Model.Contet
{
    public class Oferta
    {
        public int IdOferta { get; set; }
        public string IdExtretOferta { get; set; }
        public int IdJocPlatataforma { get; set; }
        [JsonIgnore]
        public JocEnPlataforma JocPlatataforma { get; set; }
        public int Descompte { get; set; } 
        public DateTimeOffset DataIniciOferta { get; set; }
        public DateTimeOffset DataFiOferta { get; set; }
        public bool esGratis { get; set; }
        public decimal PreuMomentOferta { get; set; }
        [JsonIgnore]
        public string DadesJsonOferta { get; set; }
        public bool EstaActiva { get; set; }
        public override string ToString()
        {
            return SerialitzadorAmbConfig.SerializerAmbOpcioDeModel(this);
        }
    }
}
