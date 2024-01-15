namespace Data.Options
{
    public class WebDriverOptions
    {
        public const string WebDriver = "WebDriver";

        public bool Headless { get; set; }

        public double ImplicitWaitInSeconds { get; set; }
    }
}
