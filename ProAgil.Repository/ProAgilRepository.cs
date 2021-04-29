using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly DataContext _context;
        public ProAgilRepository(DataContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        // Geral
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        // Evento
        public async Task<Evento[]> GetAllEventoAsync(bool incluiPalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(o => o.Lotes)
                .Include(o => o.RedesSociais);
            if(incluiPalestrantes)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }
            query = query
                .AsNoTracking()
                .OrderByDescending(o => o.DataEvento);

            return await query.ToArrayAsync();   
        }

        public async Task<Evento> GetAllEventoAsyncById(int eventoId, bool incluiPalestrantes)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(o => o.Lotes)
                .Include(o => o.RedesSociais);
            if(incluiPalestrantes)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }
            query = query
                .AsNoTracking()
                .OrderByDescending(o => o.DataEvento)
                .Where(o => o.Id == eventoId);

            return await query.FirstOrDefaultAsync();   
        }

        public async Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool incluiPalestrantes)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(o => o.Lotes)
                .Include(o => o.RedesSociais);
            if(incluiPalestrantes)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }
            query = query
                .AsNoTracking()
                .OrderByDescending(o => o.DataEvento)
                .Where(o => o.Tema.ToLower().Contains(tema.ToLower()));

            return await query.ToArrayAsync();   
        }

        // Palestrante
        public async Task<Palestrante[]> GetAllPalestrantesAsyncByName(string nome, bool incluiEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(o => o.RedesSociais);
            if(incluiEventos)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }
            query = query
                .AsNoTracking()
                .Where(o => o.Nome.ToLower().Contains(nome.ToLower()));

            return await query.ToArrayAsync();   
        }

        public async Task<Palestrante> GetPalestranteAsync(int palestranteId, bool incluiEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(o => o.RedesSociais);
            if(incluiEventos)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }
            query = query
                .AsNoTracking()
                .OrderBy(o => o.Nome)
                .Where(o => o.Id == palestranteId);

            return await query.FirstOrDefaultAsync();   
        }
    }
}