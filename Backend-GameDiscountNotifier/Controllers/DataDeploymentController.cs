using Backend_GameDiscountNotifier.Data;
using Backend_GameDiscountNotifier.Model.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_GameDiscountNotifier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataDeploymentController : ControllerBase
    {
        private readonly IServiceScopeFactory scopeFactory;
        public DataDeploymentController(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<CaratulaOferta>> GetCaratula()
        {

            using IServiceScope scope = scopeFactory.CreateScope();
            using MariaDbContext context = scope.ServiceProvider.GetRequiredService<MariaDbContext>();
            DateTime ara = DateTime.Now;
            var resultat = await context
                .Ofertas
                //.Where(e => ara >= e.DataIniciOferta && ara <= e.DataFiOferta)
                .Select(e => new CaratulaOferta
                {
                    IdOferta = e.IdExtretOferta,
                    Preu = e.PreuMomentOferta,
                    EsGratis = e.esGratis,
                    Link = e.JocPlatataforma.Enllaç,
                    NomSeller = e.JocPlatataforma.Joc.Seller.NomSeller,
                    NomPlataforma = e.JocPlatataforma.Plataforma.NomPlataforma
                    //DescompteTotal = e.DescompteCalculat
                }).ToListAsync();
            return resultat;
        }
    }
}
