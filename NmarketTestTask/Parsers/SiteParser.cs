using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using NmarketTestTask.Models;

namespace NmarketTestTask.Parsers
{
    public class SiteParser : IParser
    {
        public IList<House> GetHouses(string path)
        {
            var doc = new HtmlDocument();
            doc.Load(path);

            HtmlNode node = doc.DocumentNode.SelectSingleNode("//thead");
            string text = node.InnerText;
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(".//th");

            node = doc.DocumentNode.SelectSingleNode("//tbody");

            List<House> Houses = new List<House>();
            foreach (var tr in node.ChildNodes)
            {

                House CurrentHouse = new House();
                Flat CurrentFlat = new Flat();

                foreach (var td in tr.ChildNodes)
                {
                    string test = td.GetAttributeValue("class", String.Empty);
                    switch (td.GetAttributeValue("class", String.Empty))
                    {
                        case "house":
                            foreach (var house in Houses)
                            {
                                if (house.Name == td.InnerText.Trim())
                                {
                                    CurrentHouse = house;
                                    break;
                                }

                            }
                            if (CurrentHouse.Name is null)
                            {
                                CurrentHouse.Name = td.InnerText.Trim();
                                Houses.Add(CurrentHouse);
                            }
                            break;

                        case "number":
                            CurrentFlat.Number = td.InnerText.Trim();
                            break;

                        case "price":
                            CurrentFlat.Price = td.InnerText.Trim();
                            break;
                    }
                    
                }
                if (CurrentHouse.Flats == null && !(CurrentFlat.Number is null))
                {
                    CurrentHouse.Flats = new List<Flat>();
                }
                if (!(CurrentFlat.Number is null))
                  CurrentHouse.Flats.Add(CurrentFlat);
            }
            
            return Houses;
            //throw new System.NotImplementedException();
        }
    }
}
