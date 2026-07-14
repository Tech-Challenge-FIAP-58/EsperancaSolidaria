using UserService.Infra.Utils;

namespace UserService.Test
{
    public class BCryptPasswordHasherTests
    {
        private readonly BCryptPasswordHasher _hasher = new();

        [Fact]
        public void Hash_ProducesHashDifferentFromRaw()
        {
            var hash = _hasher.Hash("Senha@123");

            Assert.False(string.IsNullOrWhiteSpace(hash));
            Assert.NotEqual("Senha@123", hash);
        }

        [Fact]
        public void Verify_CorrectPassword_ReturnsTrue()
        {
            var hash = _hasher.Hash("Senha@123");
            Assert.True(_hasher.Verify("Senha@123", hash));
        }

        [Fact]
        public void Verify_WrongPassword_ReturnsFalse()
        {
            var hash = _hasher.Hash("Senha@123");
            Assert.False(_hasher.Verify("outra-senha", hash));
        }

        [Fact]
        public void Hash_SameInput_ProducesDifferentHashes_DueToSalt()
        {
            var h1 = _hasher.Hash("Senha@123");
            var h2 = _hasher.Hash("Senha@123");

            Assert.NotEqual(h1, h2);
            Assert.True(_hasher.Verify("Senha@123", h1));
            Assert.True(_hasher.Verify("Senha@123", h2));
        }
    }
}
