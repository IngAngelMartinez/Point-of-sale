using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Asistencia> Asistencia { get; set; }

        DbSet<CatalogoAsistencia> CatalogoAsistencia { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
