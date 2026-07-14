namespace Backend_GameDiscountNotifier.Model.DTO
{
    public class CaratulaOferta
    {
        public string IdOferta { get; set; } = "";
        public decimal Preu { get; set; }
        public bool EsGratis { get; set; }
        public string Link { get; set; } = "";
        public string NomSeller { get; set; } = "";
        public string NomPlataforma { get; set; } = "";
        public decimal DescompteTotal { get; set; }
    }
}
