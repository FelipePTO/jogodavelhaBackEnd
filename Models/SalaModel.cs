using System;
using System.ComponentModel.DataAnnotations;

namespace SalaParametros{
    public class SalaModel{
        [Key]
        public int id {get;set;}
        public string donosala {get;set;}
        public string nomesala {get;set;}
        public string idsala {get;set;}
        public string jogador1 {get;set;}
        public string jogador2 {get;set;}
        public DateTime datacriacao{get;set;}
        public bool ativa {get;set;}
    }
}