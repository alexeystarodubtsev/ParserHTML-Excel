using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using NmarketTestTask.Models;

namespace NmarketTestTask.Parsers
{
    public class ExcelParser : IParser
    {
        public IList<House> GetHouses(string path)
        {
            XLWorkbook wb = new XLWorkbook(path);
            IXLWorksheet sheet = wb.Worksheets.First();
            IXLRange RangeSheet = sheet.RangeUsed();
            int rightBorder = RangeSheet.LastColumn().ColumnNumber(); //установим правую границу данных
            int downBorder = RangeSheet.LastRow().RowNumber(); //установим нижнюю границу данных
            List<IXLCell> Xlhouses = sheet.Cells().Where(c => c.GetValue<string>().Contains("Дом")).ToList(); //получим ячейки, в которых номера домов
            List<House> houses = new List<House>();
            foreach (var Xlhouse in Xlhouses)
            {
                House house = new House();
                house.Name = Xlhouse.GetValue<string>();
                house.Flats = new List<Flat>();
                IXLCell cell = Xlhouse.CellBelow();
                
                while (   !(    cell.GetValue<string>() == ""
                             && cell.CellBelow().GetValue<string>() == ""
                             && cell.CellRight().GetValue<string>() == "" 
                             && cell.CellRight().CellBelow().GetValue<string>() == "" 
                           ) 
                       && cell.WorksheetColumn().ColumnNumber() <= rightBorder
                      )       //рассматриваем, есть ли значение в ячейке под названием дома, а также в ближайших от нее соседей
                {

                    IXLCell cellBellowHouse  = cell;
                    while (   !(  cell.GetValue<string>() == "" 
                               && cell.CellBelow().GetValue<string>() == ""
                               ) 
                             && cell.WorksheetRow().RowNumber() <= downBorder
                          ) //смотрим есть ли в текущей ячейке значение или той, которая находится снизу
                     {

                        if (cell.GetValue<string>().Contains("№")) 
                        {
                            Flat flat = new Flat();
                            flat.Number = cell.GetValue<string>().Substring(1);
                            flat.Price = cell.CellBelow().GetValue<string>();
                            house.Flats.Add(flat);
                        }
                        cell = cell.CellBelow();
                    }

                    cell = cellBellowHouse.CellRight(); //после того, как закончили с колонкой, переходим в следующую колонку
                                                        //самую верхнюю под номером дома
                }

                houses.Add(house);
            }

            return houses;
            //throw new NotImplementedException();
        }
    }
}
