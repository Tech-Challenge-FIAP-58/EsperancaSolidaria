using System.Text.RegularExpressions;
using MongoDB.Driver;
using UserService.Domain.Exceptions;

namespace UserService.Infra.Mongo
{
    public static partial class MongoWriteExceptionExtensions
    {
        public static bool IsDuplicateKey(this MongoWriteException exception) =>
            exception.WriteError?.Category == ServerErrorCategory.DuplicateKey;

        public static DuplicateEntityException ToDuplicateEntityException(this MongoWriteException exception) =>
            new(ExtractPropertyName(exception.WriteError?.Message), exception);

        public static string? ExtractPropertyName(string? writeErrorMessage)
        {
            if (string.IsNullOrEmpty(writeErrorMessage))
                return null;

            var match = IndexNameRegex().Match(writeErrorMessage);

            return match.Success
                ? match.Groups["property"].Value
                : null;
        }

        [GeneratedRegex(@"index:\s*(?<property>.+?)_-?1\b")]
        private static partial Regex IndexNameRegex();
    }
}
