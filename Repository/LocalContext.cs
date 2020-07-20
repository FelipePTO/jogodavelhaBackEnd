using jogadasUsuarioModel;
using Microsoft.EntityFrameworkCore;
using SalaParametros;
using UsuariosParametros;

namespace BDLocal{
    public class LocalContext : DbContext {

        public LocalContext(DbContextOptions<LocalContext> opt) : base(opt) { }

        public DbSet<SalaModel> sala {get;set;}
        public DbSet<UsuarioModel> usuario {get;set;}
        public DbSet<jogadasModel> jogada {get;set;}
    }
}