namespace Backend_GameDiscountNotifier.Data
{
    public class LaboratoriDb
    {
        public static async Task Insert(MariaDbContext context)
        {
            //context.Plataformes.Add(new Model.Plataforma
            //{
            //    NomPlataforma = "Steam"
            //});

            await context.SaveChangesAsync();
        }
    }
}
