namespace Backend_GameDiscountNotifier.Data
{
    public class LaboratoriDb
    {
        public static async Task Insert(MariaDbContext context)
        {
            context.Plataformes.Add(new Model.Contet.Plataforma
            {
                NomPlataforma = "Epic_Games"
            });

            await context.SaveChangesAsync();
        }
    }
}
