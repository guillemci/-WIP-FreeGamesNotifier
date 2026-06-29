namespace Backend_GameDiscountNotifier.Model
{
    public class SellerJoc
    {
        public int IdSeller { get; set; }
        public string NomSeller { get; set; }
        public List<Joc> Jocs { get; set; } = new();
    }
}
