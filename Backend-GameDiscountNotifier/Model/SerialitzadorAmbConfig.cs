using System.Text.Json;

namespace Backend_GameDiscountNotifier.Model
{
    public class SerialitzadorAmbConfig
    {
        public static string SerializerAmbOpcioDeModel<T>(T contingut)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(contingut, options);
        }
    }
}
