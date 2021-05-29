using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_manager
{
    public class Extracter
    {
        public static List<Books> Extract(string title_book, int count)
        {
            try
            {
                // готовимся
                List<Books> books_for_view = new List<Books>();
                var address = $"https://www.amazon.com/";
                var chrome = new ChromeOptions();
                chrome.AddArgument("disable-extensions");
                chrome.AddArgument("headless");

                using (var browser = new ChromeDriver(chrome))
                {
                    // качаем страничку
                    browser.Navigate().GoToUrl(address);
                    IWebElement searchInput = browser.FindElement(By.Id("twotabsearchtextbox"));
                    searchInput.SendKeys(title_book + OpenQA.Selenium.Keys.Enter);
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(browser.PageSource);
                    var page = doc.DocumentNode.SelectNodes(".//div[@class='sg-col-4-of-12 s-result-item s-asin sg-col-4-of-16 sg-col sg-col-4-of-20']");

                    if (page != null)
                    {
                        // проходимся по страничке
                        foreach (var table in page)
                        {
                            if (books_for_view.Count < count)
                            {
                                Books book = new Books();
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
                                books_for_view.Add(book);
                            }
                        }
                    }
                }

                return books_for_view;
            }
            catch (Exception) { return null; }
        }
    }
}
