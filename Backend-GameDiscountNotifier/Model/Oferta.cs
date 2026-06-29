namespace Backend_GameDiscountNotifier.Model
{
    public class Oferta
    {
        public string IdExtretOferta { get; set; }
        public int IdPlataformaJoc { get; set; }
        public JocEnPlataforma plataformaJoc { get; set; }
        public int Descompte { get; set; } 
        public DateTime DataIniciOferta { get; set; }
        public DateTime DataFiOferta { get; set; }
        public bool esGratis { get; set; }
        public string DadesJsonOferta { get; set; }
        public decimal DescompteCalculat
        {
            get
            {
                if (esGratis)
                    return 0;
                else if (plataformaJoc.PreuOriginal > 0)
                    return Math.Round((plataformaJoc.PreuOriginal * Descompte) / 100, 2);
                else
                    return 0;
            }
        }
        public bool OfertaEnPeu
        {
            get
            {
                DateTime ara = DateTime.Now;

                if (ara >= DataIniciOferta && ara <= DataFiOferta)
                    return true;
                else
                    return false;
            }
        }
    }
}
