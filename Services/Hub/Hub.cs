using System;
using System.Linq;
using System.Threading.Tasks;
using BDLocal;
using HelperFuncoes;
using jogadasUsuarioModel;
using JogoFuncoes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using UsuariosParametros;

namespace wsJogo{
    public class JogoHub : Hub {

        private readonly IJogo _local;
        private readonly LocalContext _banco;
        private readonly IHelper _help;
        public JogoHub(IJogo local, LocalContext banco, IHelper help){
            _local=local;
            _banco=banco;
            _help=help;
        }
        public async override Task OnConnectedAsync()
        {
            try{
                var httpContext = Context.GetHttpContext();
                var tokenValue = httpContext.Request.Query["access_token"].ToString();
                var usuarionome = _help.retornaUsuario(tokenValue);

                var usuario = _banco.usuario.Where(d=>d.nome==usuarionome && d.ativo==true).FirstOrDefault();
                if(usuario!=null){
                    usuario.ativo=false;
                    _banco.SaveChanges();
                }
                    await base.OnConnectedAsync();
                    UsuarioModel novousaurio = new UsuarioModel();
                    novousaurio.ativo=true;
                    novousaurio.nome=usuarionome;
                    novousaurio.datacriacao=DateTime.Now;
                    novousaurio.connectionId=Context.ConnectionId;
                    _banco.usuario.Add(novousaurio);
                    _banco.SaveChanges();
                    var listausuarios = _banco.usuario.Where(d=>d.ativo==true).ToList();
                    await Clients.All.SendAsync("listausuarios", listausuarios);
                
            }catch(Exception e){
                throw new System.InvalidOperationException("Erro");
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // do the logging here
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                //Remove Logged User
                var user =_banco.usuario.Where(d=>d.nome==Context.ConnectionId).FirstOrDefault();
                user.ativo=false;
                _banco.SaveChanges();

                var listausuarios = _banco.usuario.Where(d=>d.ativo==true).ToList();
                //Update Client
                await Clients.All.SendAsync("listausuarios", listausuarios);
            }        
        }

        public async Task CriarSala(string msg)
        {
            try{
                var salas = _local.listaSalasAbertas();
                await Clients.All.SendAsync("CriarSala", salas);
            }catch(Exception e){
                throw new System.InvalidOperationException("Erro");
            }
        }
        
        public async Task Message(string message)
        {
            await Clients.Others.SendAsync("Message", message);
        }

        public async Task EntrarSala(string message)
        {
            var httpContext = Context.GetHttpContext();
            var tokenValue = httpContext.Request.Query["access_token"].ToString();
            var usuarionome = _help.retornaUsuario(tokenValue);
            var usuarioCriou =  _banco.sala.Where(d=>d.idsala==message && d.jogador1==usuarionome && d.ativa==true).FirstOrDefault();
            var usuarios = _banco.sala.Where(d=>d.idsala==message && d.ativa==true).FirstOrDefault();

            if(usuarioCriou==null){
                if(usuarios.jogador2==null){
                    usuarios.jogador2=usuarionome;
                    _banco.SaveChanges();
                    await Clients.All.SendAsync("UsuariosSala", usuarios);
                }else{
                    throw new System.InvalidOperationException("Sala Cheia");
                }
            }else{
                await Clients.All.SendAsync("UsuariosSala", usuarios);
            }

            var salas = _local.listaSalasAbertas();
            await Clients.All.SendAsync("CriarSala", salas);
            
        }
        public async Task SairSala(string message)
        {
            var httpContext = Context.GetHttpContext();
            var tokenValue = httpContext.Request.Query["access_token"].ToString();
            var usuarionome = _help.retornaUsuario(tokenValue);
            
            var usuarios = _banco.sala.Where(d=>d.idsala==message && d.jogador1==usuarionome && d.ativa==true).FirstOrDefault();
            if(usuarios!=null){
                usuarios.jogador1="";
                _banco.SaveChanges();
                await Clients.Others.SendAsync("UsuariosSala", usuarios);
            }else{
                var usuarios2 = _banco.sala.Where(d=>d.idsala==message && d.jogador2==usuarionome && d.ativa==true).FirstOrDefault();    
                usuarios2.jogador2="";
                _banco.SaveChanges();
                await Clients.Others.SendAsync("UsuariosSala", usuarios2);
            }

            var salas = _local.listaSalasAbertas();
            await Clients.All.SendAsync("CriarSala", salas);
            
        }


        public async Task fazerJogada(string sala, int jogada)
        {
            var httpContext = Context.GetHttpContext();
            var tokenValue = httpContext.Request.Query["access_token"].ToString();
            var usuarionome = _help.retornaUsuario(tokenValue);
            jogadasModel _jogada = new jogadasModel();
            _jogada.nomejogador=usuarionome;
            
            var dadossala = _banco.sala.Where(d=>d.nomesala==sala && d.ativa==true).FirstOrDefault();
            
            if(dadossala.jogador1==usuarionome){
                _jogada.simbolo="X";
            }else{
                _jogada.simbolo="O";
            }
                _jogada.datacriacao=DateTime.Now;
                _jogada.nomesala=sala;
                _jogada.posicao=jogada;
                _banco.jogada.Add(_jogada);
                _banco.SaveChanges();
            //string strfromUserId = (ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserID).FirstOrDefault()).ToString();

        }


    }
}