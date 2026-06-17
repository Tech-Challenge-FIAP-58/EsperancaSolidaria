namespace UserService.Domain.Models
{
    public class EntityBase
    {
        public Guid Guid { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
