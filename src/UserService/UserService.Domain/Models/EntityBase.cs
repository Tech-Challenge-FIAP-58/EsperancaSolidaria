namespace UserService.Domain.Models
{
    public class EntityBase
    {
        public Guid Guid { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
