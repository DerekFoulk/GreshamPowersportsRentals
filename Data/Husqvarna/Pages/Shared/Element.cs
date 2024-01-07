using Data.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace Data.Husqvarna.Pages.Shared
{
    public abstract class Element<T> : BaseElement<T> where T : Element<T>
    {
        protected Element(ILogger logger, IWebDriver webDriver) : base(logger, webDriver)
        {
        }
    }
}
