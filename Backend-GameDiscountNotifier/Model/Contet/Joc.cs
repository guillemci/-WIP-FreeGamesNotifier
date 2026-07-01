namespace Backend_GameDiscountNotifier.Model.Contet
{
    public class Joc
    {
        public int IdJoc { get; set; }
        public string Title { get; set; }
        public int IdSeller { get; set; }
        public string Tipus { get; set; }
        public SellerJoc Seller { get; set; }
        public List<JocEnPlataforma> JocEnPlataformes { get; set; } = new();
        public override string ToString()
        {
            return SerialitzadorAmbConfig.SerializerAmbOpcioDeModel(this);
        }
    }
}
