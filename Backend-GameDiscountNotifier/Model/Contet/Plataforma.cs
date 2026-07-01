namespace Backend_GameDiscountNotifier.Model.Contet
{
    public class Plataforma
    {
        public int Id { get; set; }
        public string NomPlataforma { get; set; }
        public List<JocEnPlataforma> JocsPlataforma { get; set; } = new();
        public override string ToString()
        {
            return SerialitzadorAmbConfig.SerializerAmbOpcioDeModel(this);
        }
    }
}
