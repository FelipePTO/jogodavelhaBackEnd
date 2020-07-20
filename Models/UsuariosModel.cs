using System;
using System.ComponentModel.DataAnnotations;

namespace UsuariosParametros{
    public class UsuarioModel{
        [Key]
        public int id {get;set;}
        public string nome {get;set;}
        public string connectionId {get;set;}
        public DateTime datacriacao {get;set;}
        public bool ativo {get;set;}
    }
}