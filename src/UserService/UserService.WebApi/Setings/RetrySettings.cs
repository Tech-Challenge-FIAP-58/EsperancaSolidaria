namespace UserService.WebApi.Setings
{
	public static class RetrySettings
	{
		public static int MaxRetryAttempts { get; set; }
		public static int DelayBetweenRetriesInSeconds { get; set; }
	}
}
