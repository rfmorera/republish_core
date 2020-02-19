using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Services.DTOs.AnuncioHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utils
{
    public static class SeleniumInsert
    {
        public static Tuple<string, string> Reinsert(FormInsertAnuncio formInsertAnuncio)
        {
            FirefoxOptions options = new FirefoxOptions();
            //options.AddArguments("--headless");
            options.AcceptInsecureCertificates = true;
            IWebDriver driver = new FirefoxDriver("C:\\Driver\\", options);

            driver.Navigate().GoToUrl(@"https://www.revolico.com/insertar-anuncio.html");
            IWebElement priceInput = driver.FindElement(By.Name("price"));
            priceInput.SendKeys(formInsertAnuncio.variables.price.ToString());

            IWebElement titleInput = driver.FindElement(By.Name("title"));
            titleInput.SendKeys(formInsertAnuncio.variables.title);

            IWebElement descriptionInput = driver.FindElement(By.XPath("//textarea[@name='description']"));
            descriptionInput.SendKeys(formInsertAnuncio.variables.description);

            IWebElement emailInput = driver.FindElement(By.Name("email"));
            emailInput.SendKeys(formInsertAnuncio.variables.email);

            IWebElement nameInput = driver.FindElement(By.Name("name"));
            nameInput.SendKeys(formInsertAnuncio.variables.name);

            IWebElement phoneInput = driver.FindElement(By.Name("phone"));
            phoneInput.SendKeys(formInsertAnuncio.variables.phone);

            IWebElement selectElement = driver.FindElement(By.Name("subcategory"));
            var selectObject = new SelectElement(selectElement);
            selectObject.SelectByValue(formInsertAnuncio.variables.subcategory);

            // Remove Captcha
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("document.getElementsByTagName('iframe')[0].remove()");

            IWebElement captchaInput = driver.FindElement(By.Id("g-recaptcha-response"));
            js.ExecuteScript("arguments[0].style='display: block;'", captchaInput);
            captchaInput.SendKeys(formInsertAnuncio.variables.captchaResponse);

            IWebElement submitButton = driver.FindElement(By.XPath("//button[@type='submit']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0," + submitButton.Location.Y + ")");
            submitButton.Click();
            Tuple<string, string> answer = new Tuple<string, string>(driver.Url, driver.PageSource);
            driver.Quit();
            return answer;
        }
    }
}
