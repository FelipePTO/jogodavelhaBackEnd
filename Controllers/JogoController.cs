using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using HelperFuncoes;
using JogoFuncoes;
using Microsoft.AspNetCore.Mvc;
using SiteParametros;

namespace back.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class JogoController : ControllerBase
    {
        private readonly IJogo _jogo;
        private readonly IHelper _help;
        public JogoController(IJogo jogo, IHelper help){
            _jogo=jogo;
            _help=help;
        }

        [HttpGet("loginUsuario/{usuario}")]
        public ActionResult loginUsuario(string usuario)
        {
            try{
                //string token = Request.Headers["Authorization"];
                //var usuario = _help.retornaUsuario(token);
                var resultado = _jogo.loginUsuario(usuario);
                if(resultado.status=="OK")
                    return StatusCode(200,resultado);                
                else
                    return StatusCode(422,resultado);                
            }catch(Exception e){
                return StatusCode(422, e.Message);
            }
        }

        [HttpPost("CriarSala")]
        public ActionResult CriarSala([FromBody] InfosSalaModel sala)
        {
            try{
                string token = Request.Headers["Authorization"];
                var usuario = _help.retornaUsuario(token);
                var resultado = _jogo.CriarSala(sala.nomesala, usuario);
                if(resultado.status=="OK")
                    return StatusCode(200,resultado);                
                else
                    return StatusCode(422,resultado);                
            }catch(Exception e){
                return StatusCode(422, e.Message);
            }
        }

        [HttpPut("SairSala")]
        public ActionResult SairSala([FromBody] InfosSalaModel sala)
        {
            try{
                string token = Request.Headers["Authorization"];
                var usuario = _help.retornaUsuario(token);
                var resultado = _jogo.SairSala(sala.nomesala, usuario);
                if(resultado.status=="OK")
                    return StatusCode(200,resultado);                
                else
                    return StatusCode(422,resultado);                
            }catch(Exception e){
                return StatusCode(422, e.Message);
            }
        }

        [HttpGet("ListarSalasAbertas")]
        public ActionResult ListarSalasAbertas()
        {
            try{
                string token = Request.Headers["Authorization"];
                var usuario = _help.retornaUsuario(token);
                var resultado = _jogo.listaSalasAbertas();
                return StatusCode(200,resultado);                
            }catch(Exception e){
                return StatusCode(422, e.Message);
            }
        }

        [HttpGet("EntrarSala")]
        public ActionResult EntrarSala([FromBody] InfosSalaModel sala)
        {
            try{
                string token = Request.Headers["Authorization"];
                var usuario = _help.retornaUsuario(token);
                var resultado = _jogo.EntrarSala(sala.nomesala, usuario);
                if(resultado.status=="OK")
                    return StatusCode(200,resultado);                
                else
                    return StatusCode(422,resultado);                
            }catch(Exception e){
                return StatusCode(422, e.Message);
            }
        }

    }
}
