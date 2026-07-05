using System.Text.Json.Serialization;

namespace Backend_GameDiscountNotifier.Model.Contet
{
    public class Plataforma
    {
        public int Id { get; set; }
        public string NomPlataforma { get; set; }
        [JsonIgnore]
        public List<JocEnPlataforma> JocsPlataforma { get; set; } = new();
        public override string ToString()
        {
            return SerialitzadorAmbConfig.SerializerAmbOpcioDeModel(this);
        }
    }
}
