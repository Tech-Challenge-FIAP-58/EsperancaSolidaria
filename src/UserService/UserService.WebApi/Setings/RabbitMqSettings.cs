namespace UserService.WebApi.Setings
{
	public class RabbitMqSettings
	{
		public required string Host { get; set; }
		public required string VirtualHost { get; set; }
		public required ushort Port { get; set; }
		public required string Username { get; set; }
		public required string Password { get; set; }
	}
}
