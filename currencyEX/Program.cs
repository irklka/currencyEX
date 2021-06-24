using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;

namespace currencyEX
{

    class Program
    {
        //      8.     მოცემულია საქართველოს ეროვნული ბანკის RSS feed-ის მისამართი: http://www.nbg.ge/rss.php ,
        //      რომელიც არბუნებს მიმდინარე ვალუტის კურსებს. დაწერეთ ფუნქცია, რომელსაც გადაეცემა ორი ვალუტის
        //      იდენტიფიკატორი(USD, GEL, EUR…) და აბრუნებს ვალუტებს შორის გაცვლის კურსს.
        
           


        public static XmlDocument LoadWeb()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("http://www.nbg.ge/rss.php");

            return xdoc;
        }
        public static string FromXmlToString(XmlDocument xdoc)
        {
            //----------------From XML To String-----------------------
            XmlNodeList desc = xdoc.GetElementsByTagName("description");
            string table = desc[1].InnerText;

            return table;

        }
        public static List<HtmlNode> FromStringToHtml(string data)
        {
            //----------------From String To Html-----------------------
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            List<HtmlNode> nodes = new List<HtmlNode>();
            nodes = doc.DocumentNode.SelectNodes("//td").ToList();


            return nodes;

        }

        public static string ReturnOnlyNumbers(string tmp)
        {
            string pattern = @"\D+";
            Regex rg = new Regex(pattern);

            tmp = rg.Replace(tmp, "");

            return tmp;

        }
        public static string ReturnOnlyLetters(string tmp)
        {
            string pattern = @"[^a-zA]";
            Regex rg = new Regex(pattern);

            tmp = rg.Replace(tmp, "");

            return tmp;

        }

        public static Dictionary<string, decimal> MakeListOfCurrencies(List<HtmlNode> nodes)
        {
            var curr = new Dictionary<string, decimal> ();// list for currency Name and Rate

           
            int counter = 0;
            string currency = "", rate = "", tmp = "";
            foreach (var item in nodes)
            {
                if (counter == 5) counter = 0;

                if (counter == 0)
                {
                    currency = item.InnerText.ToString();
                }
                if (counter == 1)
                {
                    tmp = item.InnerText.ToString();

                   tmp = ReturnOnlyNumbers(tmp);
                }
                if (counter == 2)
                {
                    rate = item.InnerText.ToString();
                    decimal multipi = Convert.ToDecimal(rate);
                    int b = Int32.Parse(tmp);
                    multipi /= b;
                    curr.Add(currency, multipi);

                }


                counter++;
            }

            return curr;
        }


        public static void PrintAllCurrencies(Dictionary<string, decimal> currencyList)
        {
            foreach (var i in currencyList)
            {
                Console.WriteLine("Currency: {0}   Rate = {1}", i.Key, i.Value);
            }
            Console.WriteLine("There Are: {0} Currencies At The Moment.",currencyList.Count());


        }

        private static void GetExchangeRateFor()
        {
            XmlDocument xdoc = LoadWeb();

            string data = FromXmlToString(xdoc);

            List<HtmlNode> nodes = FromStringToHtml(data);

            Dictionary<string, decimal> currency = MakeListOfCurrencies(nodes);

            PrintAllCurrencies(currency);

            string firstCurr, secondCurr,result;
            decimal res1 = 0, res2 = 0;

            Console.WriteLine();
            Console.WriteLine("Enter Any Currencies From Above ^^^");

            Console.Write("First Currency: ");
            firstCurr = ReturnOnlyLetters( Console.ReadLine()).ToString().ToUpper();

            Console.Write("Second Currency: ");
            secondCurr = ReturnOnlyLetters( Console.ReadLine()).ToString().ToUpper();

            if(currency.ContainsKey(firstCurr))  res1 = currency[firstCurr];
            else { Console.WriteLine("Please Enter Correctly..."); }

            if (currency.ContainsKey(secondCurr))  res2 = currency[secondCurr];
            else { Console.WriteLine("Please Enter Correctly..."); }

            result = String.Format("{0:0.000000}", res1/res2);

            Console.WriteLine("1 {0} is {1} {2}",firstCurr,result,secondCurr);

        }



        static void Main()
        {
            try
            {
             GetExchangeRateFor();

            }
            catch(DivideByZeroException e)
            {
                Console.WriteLine(e.Message);
            }

        }

       
    }
}
