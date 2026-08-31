using Capri.Sgr.Domain.ValueObjects;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.ValueObjects;

public class CnpjTests
{
    [TestCase("04.252.011/0001-10", "04252011000110")]
    [TestCase("11.222.333/0001-81", "11222333000181")]
    public void ValidCnpjIsNormalized(string input, string expected) => new Cnpj(input).Value.ShouldBe(expected);

    [TestCase("04.252.011/0001-11")]
    [TestCase("00.000.000/0000-00")]
    [TestCase("04.252.011/0001")]
    public void InvalidCnpjIsRejected(string input) => Should.Throw<ArgumentException>(() => new Cnpj(input));
}