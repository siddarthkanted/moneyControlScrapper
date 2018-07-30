using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace HackingConsoleApp
{
    public class MoneyControlMutualFund
    {
        private string startUrl = "http://www.moneycontrol.com/mutual-funds/performance-tracker/returns/diversified-equity.html";
        private HashSet<string> mutualFundLinksHashSet = new HashSet<string>();
        private string prefixUrl = "http://www.moneycontrol.com/";

        public void ScrapFundsList()
        {
            HtmlDocument htmlDocument = new HtmlWeb().Load(startUrl);
            HtmlNodeCollection htmlNodeCollection =
                htmlDocument.DocumentNode.SelectNodes("//a[@class='bl_12']");
            if (htmlNodeCollection == null)
            {
                Console.WriteLine("html is empty");
            }
            else
            {
                Console.WriteLine("html is non-empty");
                foreach (HtmlNode htmlNode in htmlNodeCollection)
                {
                    mutualFundLinksHashSet.Add(htmlNode.Attributes["href"].Value);

                }
            }
            List<string> fileContent = new List<string>();
            fileContent.Add("Link\tName\t2018\t2017\t2016\t2015\t2014\t2013");
            fileContent.AddRange(mutualFundLinksHashSet.Select(x => this.MutualFundTsvData(x).ToString()));
            File.WriteAllLines(@"C:\files\mutualFund\diversifiedReturnsData.tsv", fileContent);
        }

        public StringBuilder MutualFundTsvData(string mutualFundLink)
        {
            Console.WriteLine(mutualFundLink);
            StringBuilder output = new StringBuilder();
            string fullLink = prefixUrl + mutualFundLink;
            HtmlDocument htmlDocument = new HtmlWeb().Load(fullLink);


            //title  <h1 class="pcstname">ICICI Prudential Bluechip Fund - Direct Plan (G)</h1>
            output.Append(fullLink);
            output.Append("\t" + htmlDocument.DocumentNode.SelectNodes("//h1[@class ='pcstname']").First().InnerText);

            HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes(
                "//div[@class='col-500 ml-5 FL']/div/table[@class ='responsive']/tbody/tr/td/span[@class='red_txt14' or @class='green_txt14']");
                
            if (htmlNodeCollection == null)
            {
                Console.WriteLine("html is empty");
            }
            else
            {
                int count = 0;
                while (count < htmlNodeCollection.Count)
                {
                    output.Append("\t" + SumYear(htmlNodeCollection, count));
                    count += 5;
                }
            }

            return output;
        }

        private double GetDouble(string value)
        {
            double output = 0;
            double.TryParse(value, out output);
            return output;
        }

        //4 quarters
        //So sum up all the 4 quarters and ignore the annual
        private double SumYear(HtmlNodeCollection htmlNodeCollection, int start)
        {
            double sum = 0;
            for (int i = start; i <= start + 3; i++)
            {
                if(i < htmlNodeCollection.Count && !string.IsNullOrEmpty(htmlNodeCollection[i].InnerText))
                sum += GetDouble(htmlNodeCollection[i].InnerText);
            }
            return sum;
        }

    }
}
