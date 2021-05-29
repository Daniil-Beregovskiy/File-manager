using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_manager
{
    public class Parser
    {
        public static List<Book> Parse(string nameLang, int count)
        {
            try
            {
                List<Book> resultBooks = new List<Book>();
                var address = $"https://www.amazon.com/";
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("disable-extensions");
                chromeOptions.AddArgument("headless");
                using (var browser = new ChromeDriver(chromeOptions))
                {
                    browser.Navigate().GoToUrl(address);
                    IWebElement searchInput = browser.FindElement(By.Id("twotabsearchtextbox"));
                    searchInput.SendKeys(nameLang + OpenQA.Selenium.Keys.Enter);
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(browser.PageSource);
                    var tables = doc.DocumentNode.SelectNodes(".//div[@class='sg-col-4-of-12 s-result-item s-asin sg-col-4-of-16 sg-col sg-col-4-of-20']");
                    if (tables != null)
                    {
                        foreach (var table in tables)
                        {
                            if (resultBooks.Count < count)
                            {
                                Book book = new Book();
                                book.Name = table.SelectSingleNode(".//span[@class='a-size-base-plus a-color-base a-text-normal']") == null ? "null" : table.SelectSingleNode(".//span[@class='a-size-base-plus a-color-base a-text-normal']").InnerText;
                                if (book.Name == "null")
                                    book.Name = "-";
                                book.Author = table.SelectSingleNode(".//a[@class='a-size-base a-link-normal']") == null ? "null" : table.SelectSingleNode(".//a[@class='a-size-base a-link-normal']").InnerText;
                                if (book.Author == "null")
                                    book.Author = "-";
                                book.Price = table.SelectSingleNode(".//span[@class='a-offscreen']") == null ? "null" : table.SelectSingleNode(".//span[@class='a-offscreen']").InnerText;
                                if (book.Price == "null")
                                    book.Price = "$00.00";
                                book.Rating = table.SelectSingleNode(".//span[@class='a-icon-alt']") == null ? "null" : table.SelectSingleNode(".//span[@class='a-icon-alt']").InnerText.Substring(0, 3);
                                if (book.Rating == "null")
                                    book.Rating = "-";
                                book.Link = table.SelectSingleNode(".//a[@class='a-link-normal a-text-normal']") == null ? "null" : "http://amazon.com" + table.SelectSingleNode(".//a[@class='a-link-normal a-text-normal']").Attributes["href"].Value;
                                resultBooks.Add(book);
                            }
                        }
                    }
                }
                return resultBooks;
            }
            catch (Exception) { return null; }
        }
    }
}
