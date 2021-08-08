using ApiCatalogoJogos.Exceptions;
using ApiCatalogoJogos.InputModel;
using ApiCatalogoJogos.Services;
using ApiCatalogoJogos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.Controllers.V1
{
    
    [Route("api/V1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {
        private readonly IJogosService _jogoService;

        public JogosController(IJogosService jogoService)
        {
            _jogoService = jogoService;
        }

        #region "Métodos"
        /// <summary>
        /// Buscar todos os jogos de forma paginada
        /// </summary>
        /// <param name="pagina"> Indica página que está sendo consultada (Mínimo (1))</param>
        /// <param name="quantidade">Indica a quantidade de Registros por página (Máximo 50)</param>
        /// <returns>Code 200 (Sucesso)</returns>
        /// <remarks>Code 204 (Caso não haja jogos em banco)</remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range (1, 50)] int quantidade = 1) 
        {
            var jogos = await _jogoService.Obter(pagina, quantidade);
            if (jogos.Count() == 0) 
            {
                return NoContent();
            }

            return Ok(jogos);
        }

        /// <summary>
        /// Buscar jogo pelo seu ID
        /// </summary>
        /// <param name="idJogo">Busca um jogo por vez</param>
        /// <returns>Code 200 (Sucesso)</returns>
        /// <remarks>Code 204 (Caso não haja o registro do jogo)</remarks>
        [HttpGet("{idJogo:guid}")]
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute]Guid idJogo)
        {
            var jogo = await _jogoService.Obter(idJogo);
            if (jogo == null) 
            {
                return NoContent();
            }

            return Ok(jogo);
        }
        /// <summary>
        /// Inserir um novo jogo
        /// </summary>
        /// <param name="jogoInputModel"> Passa como paramêtro o objeto jogo</param>       
        /// <returns>Code 200 (Sucesso)</returns>   
        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);

                return Ok(jogo);
            }
            catch (JogoJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com este nome para esta produtora!");
            }
            
        }
        /// <summary>
        /// Atualiza jogo existente
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser editado</param>
        /// <param name="jogoInputModel">Componetes do objeto a serem modificados</param>
        /// <returns>Code 200 (Sucesso)</returns>
        /// <remarks>Code 204 (Caso não haja o registro do jogo)</remarks>
        [HttpPut("{idJogo:guid}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, jogoInputModel);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo!");
            }
           
        }

        // Obs didatica: O patch atualiza apenas uma parte do recurso e não o recurso inteiro
        [HttpPatch("{idJogo:guid}/preco/{preco:decimal}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] decimal preco)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, preco);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo!");
            }
        }

        /// <summary>
        /// Deleta jogo
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser deletado</param>
        /// <returns>Code 200 (Sucesso)</returns>
        /// <remarks>Code 204 (Caso não haja o registro do jogo)</remarks>
        [HttpDelete]
        public async Task<ActionResult> ApagarJogo(Guid idJogo) 
        {
            try

            {
                await _jogoService.Remover(idJogo);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo!");
            }
        }
        #endregion
    }
}
