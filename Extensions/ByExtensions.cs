using OpenQA.Selenium;

namespace VaxCare.Core.Extensions
{
    public static class ByExtensions
    {
        public static By Id(this string id) => By.Id(id);
        public static By XPath(this string xpath) => By.XPath(xpath);
        public static By ClassName(this string className) => By.ClassName(className);
        public static By LinkText(this string linkText) => By.LinkText(linkText);
        public static By CssSelector(this string cssSelector) => By.CssSelector(cssSelector);
        public static By PartialLinktext(this string partialLinkText) => By.PartialLinkText(partialLinkText);
        public static By TagName(this string tagName) => By.TagName(tagName);
        public static By Name(this string name) => By.Name(name);
    }
}
