﻿using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Delete;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Generator;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Get;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.GetById;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AppGroup.Contabilidade.WebApi.Controllers.V1;

[Route("api/v1/[controller]")]
public class ContaContabilController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContaContabilController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CriarContaContabilResponse>> Post([FromBody] CriarContaContabilRequest request)
    {
        var result = await _mediator.Send(request);

        return request.HasError
                ? BadRequest(result.Data)
                : CreatedAtAction(nameof(Post), result.Data);
    }

    [HttpGet("GerarSugestaoConta")]
    public async Task<ActionResult<CriarSugestaoResponse>> GerarSugestaoConta([FromQuery] Guid? idPai = null)
    {
        var request = new CriarSugestaoRequest { IdPai = idPai };

        var result = await _mediator.Send(request);

        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<ActionResult<ListarContaContabilResponse>> ListarContas()
    {
        var request = new ListarContaContabilRequest();

        var result = await _mediator.Send(request);

        return request.HasError
                ? BadRequest(result.Data)
                : result;
    }

    [HttpGet("{idConta}")]
    public async Task<ActionResult<ListarPorIdContaContabilResponse>> BuscarContaPorId([FromRoute] Guid idConta)
    {
        var request = new ListarPorIdContaContabilRequest { IdConta = idConta };

        var result = await _mediator.Send(request);

        return request.HasError
                ? BadRequest(result.Data)
                : Ok(result.Data);
    }

    [HttpPut]
    public async Task<ActionResult<EditarContaContabilResponse>> Editar([FromBody] EditarContaContabilRequest request)
    {
        var result = await _mediator.Send(request);

        return request.HasError
                ? BadRequest(result.Data)
                : Ok(result.Data);
    }

    [HttpDelete]
    public async Task<ActionResult<DeletarContaContabilResponse>> Delete([FromBody] DeletarContaContabilRequest request)
    {
        var result = await _mediator.Send(request);

        return request.HasError
                ? BadRequest(result.Data)
                : Ok(result.Data);
    }
}
