using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using BDLocal;
using configSistema;
using HelperFuncoes;
using Microsoft.IdentityModel.Tokens;
using SalaParametros;
using UsuariosParametros;

namespace JogoFuncoes{
    public class Jogos : IJogo
    {
        private readonly LocalContext _local;
        private readonly IHelper _help;
        public Jogos(LocalContext local, IHelper help){
            _local=local;
            _help=help;
        }
        public RetornoModel CriarSala(string nomesala, string usuario)
        {
            RetornoModel retorno = new RetornoModel();
            try{
                var idsala = _help.RandomString(6);
                SalaModel novasala = new SalaModel();
                novasala.datacriacao=DateTime.Now;
                novasala.donosala=usuario;
                novasala.jogador1=usuario;
                novasala.nomesala=nomesala;
                novasala.ativa=true;
                novasala.idsala=idsala;

                _local.sala.Add(novasala);
                _local.SaveChanges();
                retorno.status="OK";
                
                retorno.resultado=idsala;
                return retorno;
            }catch(Exception e){
                return retorno;
            }
        }

        public RetornoModel EntrarSala(string nomesala, string usuario)
        {
            RetornoModel retorno = new RetornoModel();
            try{
                var sala = _local.sala.Where(d=>d.nomesala==nomesala).FirstOrDefault();
                if(sala!=null){
                    if(sala.jogador2==null){
                        retorno.status="OK";
                        retorno.resultado="Entrou na sala";
                        return retorno;
                    }else{  
                        retorno.status="Erro";
                        retorno.resultado="Sala Cheia";
                        return retorno;
                    }
                }else{
                    retorno.status="Erro";
                    retorno.resultado="Sala Não existe";
                    return retorno;
                }
            }catch(Exception e){
                return retorno;
            }
        }

        public List<SalaModel> listaSalasAbertas()
        {
            List<SalaModel> retorno = new List<SalaModel>();
            try{
                var listasalas = _local.sala.Where(d=>d.ativa==true && d.jogador1!="" && d.jogador2==null).ToList();
                return listasalas;
            }catch(Exception e){
                return retorno;
            }
        }

        public RetornoModel loginUsuario(string usuario)
        {
            RetornoModel retorno = new RetornoModel();
            try{
                var _usuario = _local.usuario.Where(d=>d.nome==usuario && d.ativo==true).FirstOrDefault();
                if(_usuario!=null){
                    retorno.status="Erro";
                    retorno.resultado="Usuário já existe";
                    return retorno;
                }else{
                    UsuarioModel novousuario = new UsuarioModel();
                    novousuario.ativo=true;
                    novousuario.datacriacao=DateTime.Now;
                    novousuario.nome=usuario;
                    _local.usuario.Add(novousuario);
                    _local.SaveChanges();
                    var token = gerarToken(usuario);
                    retorno.status="OK";
                    retorno.resultado=token;
                    return retorno;
                }
            }catch(Exception e){
                return retorno;
            }
        }

        public RetornoModel SairSala(string nomesala, string usuario)
        {
            throw new System.NotImplementedException();
        }


        private string gerarToken(string usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("testeDTItesteDTItesteDTI"));
            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                    issuer: "testeDTI.com.br",
                    audience: "testeDTI.com.br",
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signInCred
                );
            token.Payload["usuario"] = usuario;

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

    }
}