using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update;
using AppGroup.Contabilidade.Application.UseCases.ContaContabil.Update.Handlers;

namespace AppGroup.Contabilidade.UnitTests.UseCases.ContaContabil.Update.Handlers;

public class ChecaNivelCodigoHandlerTests
{
    [Theory]
    [InlineData("123", 1)]
    [InlineData("123.456", 2)]
    [InlineData("123.456.789", 3)]
    [InlineData("1.2.3.4", 4)]
    public async Task DeveDefinirNivelCorretamente_ComBaseNoCodigo(string codigo, int nivelEsperado)
    {
        // Arrange
        var handler = new ChecaNivelCodigoHandler();
        var request = new EditarContaContabilRequest { Codigo = codigo };

        // Act
        await handler.Process(request);

        // Assert
        Assert.Equal(nivelEsperado, request.Nivel);
        Assert.False(request.HasError);
    }

    [Fact]
    public async Task DeveDefinirErro_SeCodigoNaoInformado()
    {
        // Arrange
        var handler = new ChecaNivelCodigoHandler();
        var request = new EditarContaContabilRequest { Codigo = null! };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => handler.Process(request));
        Assert.Equal("Codigo deve ser informado", ex.Message);
    }
}
