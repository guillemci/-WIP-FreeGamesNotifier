namespace Backend_GameDiscountNotifier.Model.Contet
{
    public class SellerJoc
    {
        public int IdSeller { get; set; }
        public string NomSeller { get; set; }
        public List<Joc> Jocs { get; set; } = new();
        public override string ToString()
        {
            return SerialitzadorAmbConfig.SerializerAmbOpcioDeModel(this);
        }
    }
}
