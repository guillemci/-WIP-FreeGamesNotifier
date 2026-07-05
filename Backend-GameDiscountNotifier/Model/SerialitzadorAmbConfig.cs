using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend_GameDiscountNotifier.Model
{
    public class SerialitzadorAmbConfig
    {
        public static string SerializerAmbOpcioDeModel<T>(T contingut)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                MaxDepth = 1
            };

            return JsonSerializer.Serialize(contingut, options);
        }
    }
}