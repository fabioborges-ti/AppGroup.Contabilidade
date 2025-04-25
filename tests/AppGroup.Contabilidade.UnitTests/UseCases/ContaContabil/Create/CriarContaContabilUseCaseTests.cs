using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Create;
using AppGroup.Contabilidade.Domain.Enums;
using AppGroup.Contabilidade.Domain.Interfaces.Repositories;
using AppGroup.Contabilidade.Domain.Models.ContaContabil;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Create;

public class CriarContaContabilUseCaseTests
{
    private readonly Mock<IContaContabilRepository> _repositoryMock;
    private readonly Mock<ILogger<CriarContaContabilUseCase>> _loggerMock;
    private readonly CriarContaContabilUseCase _useCase;

    public CriarContaContabilUseCaseTests()
    {
        _loggerMock = new Mock<ILogger<CriarContaContabilUseCase>>();
        _repositoryMock = new Mock<IContaContabilRepository>();
        _useCase = new CriarContaContabilUseCase(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarCodigo_QuandoRequestValido()
    {
        // Arrange
        var request = new CriarContaContabilRequest
        {
            Codigo = "1",
            Nome = "Conta Pai",
            Tipo = TipoConta.Receitas,
            AceitaLancamentos = false
        };

        _repositoryMock
            .Setup(r => r.ExisteCodigo("1"))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.CriarContaContabil(It.IsAny<CriarContaContabilModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _useCase.Handle(request, default);

        // Assert
        Assert.False(request.HasError);
        Assert.Equal("1", response.Data);
    }

    [Fact]
    public async Task Handle_DeveRetornarErro_QuandoCodigoJaExiste()
    {
        // Arrange
        var request = new CriarContaContabilRequest
        {
            Codigo = "1",
            Nome = "Conta Pai",
            Tipo = TipoConta.Receitas,
            AceitaLancamentos = false
        };

        _repositoryMock
            .Setup(r => r.ExisteCodigo("1"))
            .ReturnsAsync(true);

        // Act
        var response = await _useCase.Handle(request, default);

        // Assert
        Assert.True(request.HasError);
        Assert.Equal("Código já cadastrado", response.Data);
    }
}