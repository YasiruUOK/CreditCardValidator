using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private List<int> listOfInts = new List<int>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Index(long creditcardnumber)
        {
            string CardType = getCardType(creditcardnumber);
            ViewBag.Name = string.Format("{0}", CardType);
            return View();
        }

        private string getCardType(long creditCardNumber)
        {
            int[] creditCardNumberArray = GetIntArray(creditCardNumber);
            string returnValue = String.Empty;
            string creditCardNumberString=creditCardNumber.ToString();
            returnValue = checkTheCardIsAmex(creditCardNumberArray, creditCardNumberString);
            return returnValue;
        }

        private string checkTheCardIsVisaCard(int[] creditCardNumberArray, string creditCardNumberString)
        {
            if (creditCardNumberArray.Length == 13 || creditCardNumberArray.Length == 16)
            {
                if (creditCardNumberArray[0] == 4)
                {
                    return "Visa: " + creditCardNumberString + " "+ checkCardIsValid(creditCardNumberArray);
                }
            }
            return "Unknown: " + creditCardNumberString + " " + " (invalid)"; 
        }

        private string checkCardIsValid(int[] creditCardNumberArray)
        {
            int sumOfCardDigits = 0;
            listOfInts.Reverse();
            
            for (int i = 0; i < listOfInts.Count; i++)
            {
                if (i % 2 != 0)
                {
                    int value = listOfInts[i] * 2;
                    int valueDividedByTen = value / 10;
                    if (valueDividedByTen > 0)
                    {
                        sumOfCardDigits += valueDividedByTen + (value % 10);
                    }
                    else
                    {
                        sumOfCardDigits += value;
                    }
                }
                else
                {
                    sumOfCardDigits += listOfInts[i];
                }
            }
            if (sumOfCardDigits % 10 == 0)
                return " (valid)";
            else
                return " (invalid)";
        }

        private string checkTheCardIsMasterCard(int[] creditCardNumberArray, string creditCardNumberString)
        {
            if (creditCardNumberArray.Length == 16)
            {
                if (creditCardNumberArray[0] == 5)
                {
                    switch (creditCardNumberArray[1])
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            return "MasterCard: "+ creditCardNumberString+" "+ checkCardIsValid(creditCardNumberArray);
                        default:
                            return "";
                    }
                }
                return checkTheCardIsVisaCard(creditCardNumberArray, creditCardNumberString);
            }
            return checkTheCardIsVisaCard(creditCardNumberArray, creditCardNumberString); 
        }

        private string checkTheCardIsDiscover(int[] creditCardNumberArray, string creditCardNumberString)
        {
            if (creditCardNumberArray.Length == 16)
            {
                string creditCardFirstFourDigts = "";
                for (int i = 0; i < 4; i++)
                {
                    creditCardFirstFourDigts += creditCardNumberArray[i];
                }
                if (creditCardFirstFourDigts == "6011")
                {
                    return "Discover: "+ creditCardNumberString+" "+ checkCardIsValid(creditCardNumberArray);
                }
            }
            return checkTheCardIsMasterCard(creditCardNumberArray, creditCardNumberString);
        }

        private string checkTheCardIsAmex(int[] creditCardNumberArray, string creditCardNumberString)
        {
            if (creditCardNumberArray.Length == 15)
            {
                switch (creditCardNumberArray[0].ToString() + creditCardNumberArray[1].ToString())
                {
                    case ("34"):
                        return "AMEX: "+ creditCardNumberString + " " + checkCardIsValid(creditCardNumberArray);
                    case ("37"):
                        return "AMEX: "+ creditCardNumberString + " " + checkCardIsValid(creditCardNumberArray);
                }
            }
            return checkTheCardIsDiscover(creditCardNumberArray, creditCardNumberString);
        }

        int[] GetIntArray(long num)
        {
            while (num > 0)
            {
                int i = (int)(num % 10);
                listOfInts.Add(i);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }
    }
}