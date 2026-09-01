using Capri.Sgr.Domain.ValueObjects;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.ValueObjects;

public class CpfTests
{
    [TestCase("529.982.247-25", "52998224725")]
    [TestCase("111.444.777-35", "11144477735")]
    public void ValidCpfIsNormalized(string input, string expected) => new Cpf(input).Value.ShouldBe(expected);

    [TestCase("529.982.247-24")]
    [TestCase("111.111.111-11")]
    [TestCase("123")]
    public void InvalidCpfIsRejected(string input) => Should.Throw<ArgumentException>(() => new Cpf(input));
}
