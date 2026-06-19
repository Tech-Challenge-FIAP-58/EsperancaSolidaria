namespace CampaignService.WebApi.Settings
{
	public static class RetrySettings
	{
		public static int MaxRetryAttempts { get; set; }
		public static int DelayBetweenRetriesInSeconds { get; set; }
	}
}
