using UserService.Domain.Exceptions;
using UserService.Infra.Mongo;

namespace UserService.Test
{
    public class MongoWriteExceptionExtensionsTests
    {
        [Theory]
        [InlineData(
            "E11000 duplicate key error collection: esperanca.users index: Email_1 dup key: { Email: \"a@b.c\" }",
            "Email")]
        [InlineData(
            "E11000 duplicate key error collection: esperanca.users index: Cpf_1 dup key: { Cpf: \"52998224725\" }",
            "Cpf")]
        [InlineData(
            "E11000 duplicate key error collection: esperanca.users index: Cpf_-1 dup key: { Cpf: \"529\" }",
            "Cpf")]
        public void ExtractPropertyName_DuplicateKeyMessage_ReturnsIndexedProperty(string message, string expected)
            => Assert.Equal(expected, MongoWriteExceptionExtensions.ExtractPropertyName(message));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("E11000 duplicate key error collection: esperanca.users index: _id_ dup key: { _id: 1 }")]
        [InlineData("erro sem formato reconhecível")]
        public void ExtractPropertyName_Unparseable_ReturnsNull(string? message)
            => Assert.Null(MongoWriteExceptionExtensions.ExtractPropertyName(message));

        [Fact]
        public void DuplicateEntityException_WithProperty_NamesItInMessage()
        {
            var ex = new DuplicateEntityException("Email");

            Assert.Equal("Email", ex.PropertyName);
            Assert.Contains("Email", ex.Message);
        }

        [Fact]
        public void DuplicateEntityException_WithoutProperty_FallsBackToGenericMessage()
        {
            var ex = new DuplicateEntityException(propertyName: null);

            Assert.Null(ex.PropertyName);
            Assert.Equal("Já existe um registro com os dados informados.", ex.Message);
        }
    }
}
