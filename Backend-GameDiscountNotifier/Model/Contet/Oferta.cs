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
        public string DadesJsonOferta { get; set; }
        public bool EstaActiva { get; set; }
        public bool EstaPlanificada { get; set; }
        //public decimal DescompteCalculat
        //{
        //    get
        //    {
        //        if (esGratis)
        //            return 0;
        //        else if (JocPlatataforma.PreuOriginal > 0)
        //            return Math.Round((JocPlatataforma.PreuOriginal * Descompte) / 100, 2);
        //        else
        //            return 0;
        //    }
        //}
        //public bool OfertaEnPeu
        //{
        //    get
        //    {
        //        DateTime ara = DateTime.Now;

        //        if (ara >= DataIniciOferta && ara <= DataFiOferta)
        //            return true;
        //        else
        //            return false;
        //    }
        //}
        public override string ToString()
        {
            return SerialitzadorAmbConfig.SerializerAmbOpcioDeModel(this);
        }
    }
}
