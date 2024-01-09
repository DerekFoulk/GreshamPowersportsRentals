using BlazorApp.Shared.Exceptions;
using BlazorApp.Shared.Extensions;
using Data.Exceptions;
using Data.Husqvarna.Pages.Shared;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Data.Extensions
{
    public static class PageExtensions
    {
        public static By? GetByFrom<T>(this Page<T> page, string propertyName) where T : Page<T>
        {
            How how;
            string? usingStr;
            
            try
            {
                how = page.GetAttributeFrom<FindsByAttribute>(propertyName).How;
                usingStr = page.GetAttributeFrom<FindsByAttribute>(propertyName).Using;
            }
            catch (AttributeNotFoundException e)
            {
                throw new FindsByAttributeNotFoundException($"Unable to extract a '{nameof(By)}' locator", e);
            }

            var by = how switch
            {
                How.Id => By.Id(usingStr),
                How.Name => By.Name(usingStr),
                How.TagName => By.TagName(usingStr),
                How.ClassName => By.ClassName(usingStr),
                How.CssSelector => By.CssSelector(usingStr),
                How.LinkText => By.LinkText(usingStr),
                How.PartialLinkText => By.PartialLinkText(usingStr),
                How.XPath => By.XPath(usingStr),
                How.Custom => throw new ArgumentOutOfRangeException(nameof(how), how,
                    $"'{nameof(How)}.{nameof(How.Custom)}' is not currently supported"),
                _ => null
            };

            return by;
        }
    }
}
