using OpenQA.Selenium;
using System;
using System.IO;

namespace AutoTest.Utils
{
    public class ScreenshotHelper
    {
        public static string Capture(IWebDriver driver, string name)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, name + "_" + DateTime.Now.Ticks + ".png");

            var shot = ((ITakesScreenshot)driver).GetScreenshot();
            shot.SaveAsFile(path);

            return path;
        }
    }
}
