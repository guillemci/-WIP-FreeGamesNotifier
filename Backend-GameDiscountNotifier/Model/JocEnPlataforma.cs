using System.Text.Json;

namespace Backend_GameDiscountNotifier.Model
{
    public class JocEnPlataforma
    {
        public int IdJoc { get; set; }
        public Joc Joc { get; set; }
        public int IdPlataforma { get; set; }
        public Plataforma Plataforma { get; set; }
        public int IdJocPlatataforma { get; set; }
        public string Desc { get; set; }
        public string Enllaç { get; set; }
        public decimal PreuOriginal { get; set; }
        public string ImatgeLink { get; set; }
        public string DadesJson { get; set; }
        public override string ToString()
        {
            return SerialitzadorAmbConfig.SerializerAmbOpcioDeModel(this);
        }
    }
}