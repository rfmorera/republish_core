using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Services.DTOs.AnuncioHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utils
{
    public static class SeleniumInsert
    {
        public static void Reinsert(FormInsertAnuncio formInsertAnuncio)
        {
            IWebDriver driver = new FirefoxDriver("~\\Services\\BrowsersDrivers\\geckodriver.exe");
        }
    }
}
