using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;

namespace MyAppWPF
{
    public static class FileSaver
    {
        private static Excel.Application excelApp;
        public static void SaveExcelFile(Order order)
        {
            string orderName = "Накладная от ";
            //if (Convert.ToDouble(order.TotalS) < 0)
            //{
            //    orderName = "Возврат ";
            //}
            //else
            //{
            //    orderName = "Заказ ";
            //}
            string fileName = @"D:\Baraban Orders\" + (order.Clients.Name + "_" + order.Id + "_" + order.OrderDate.Year + "-" + order.OrderDate.Month + "-" + order.OrderDate.Day).ToString();
            excelApp = new Excel.Application();
            excelApp.Visible = false;

            excelApp.SheetsInNewWorkbook = 1;
            excelApp.Workbooks.Add(Type.Missing);

            Excel.Workbooks excelappworkbooks = excelApp.Workbooks;
            Excel.Workbook excelappworkbook = excelappworkbooks[1];

            //Получаем ссылку на лист 1
            var xlWorkSheet = (Excel.Worksheet)excelappworkbook.Worksheets.get_Item(1);
            //Выбираем ячейку для вывода 
            var excelcells = xlWorkSheet.get_Range("A1", "F1");//Дата заказа
            excelcells.Merge(Type.Missing);
            excelcells.HorizontalAlignment = Excel.Constants.xlCenter;
            excelcells.Font.Size = 14;
            xlWorkSheet.Cells[1, 1] = orderName + order.OrderDate.ToShortDateString();

            //Заполнение строк заказа
            int i = 0;
            foreach (OrderLine ol in order.OrderLines.OrderBy(o => o.Products.Name))
            {
                excelcells = xlWorkSheet.get_Range(("A" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                xlWorkSheet.Cells[3 + i, 1] = (1 + i).ToString();//порядковый номер

                excelcells = xlWorkSheet.get_Range((("B" + (3 + i)).ToString()), (("B" + (3 + i)).ToString()));
                excelcells.Borders.ColorIndex = 15;
                excelcells.EntireColumn.AutoFit();
                xlWorkSheet.Cells[3 + i, 2] = ol.Products.Name;//название товара

                excelcells = xlWorkSheet.get_Range(("C" + (3 + i)).ToString());
                excelcells.HorizontalAlignment = Excel.Constants.xlCenter;
                excelcells.Borders.ColorIndex = 15;
                xlWorkSheet.Cells[3 + i, 3] = ol.Products.Unit;//единица измерения

                excelcells = xlWorkSheet.get_Range(("D" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                xlWorkSheet.Cells[3 + i, 4] = Convert.ToDouble(ol.Quantity);//количество товара

                excelcells = xlWorkSheet.get_Range(("E" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                excelcells.NumberFormat = "0.000";
                xlWorkSheet.Cells[3 + i, 5] = Math.Round(Convert.ToDouble(ol.Price), 3);//цена

                excelcells = xlWorkSheet.get_Range(("F" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                excelcells.NumberFormat = "0.00";
                xlWorkSheet.Cells[3 + i, 6] = Math.Round(Convert.ToDouble(ol.Total), 2);//сумма

                i++;
            }

            //надпись "Итого:"
            string cellName1 = ("A" + (3 + i)).ToString();
            string cellName2 = ("E" + (3 + i)).ToString();
            excelcells = xlWorkSheet.get_Range(cellName1, cellName2);
            excelcells.Merge(Type.Missing);
            excelcells.Font.Size = 14;
            excelcells.HorizontalAlignment = Excel.Constants.xlRight;
            xlWorkSheet.Cells[3 + i, 1] = "Итого:";

            //Итоговая Сумма
            string cellName3 = ("F" + (3 + i)).ToString();
            excelcells = xlWorkSheet.get_Range(cellName3);
            excelcells.Font.Size = 14;
            excelcells.NumberFormat = "0.00";
            xlWorkSheet.Cells[3 + i, 6] = Math.Round(Convert.ToDouble(order.TotalS), 2);
            excelcells.EntireColumn.AutoFit();


            if (Convert.ToDouble(order.PaymentS) != 0)
            {
                cellName1 = ("A" + (4 + i)).ToString();
                cellName2 = ("E" + (4 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName1, cellName2);
                excelcells.Merge(Type.Missing);
                excelcells.Font.Size = 14;
                excelcells.HorizontalAlignment = Excel.Constants.xlRight;
                xlWorkSheet.Cells[4 + i, 1] = "Оплачено:";


                cellName3 = ("F" + (4 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName3);
                excelcells.Font.Size = 14;
                excelcells.NumberFormat = "0.00";
                xlWorkSheet.Cells[4 + i, 6] = Math.Round(Convert.ToDouble(order.PaymentS), 2);
                excelcells.EntireColumn.AutoFit();


                cellName1 = ("A" + (5 + i)).ToString();
                cellName2 = ("E" + (5 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName1, cellName2);
                excelcells.Merge(Type.Missing);
                excelcells.Font.Size = 14;
                excelcells.HorizontalAlignment = Excel.Constants.xlRight;
                xlWorkSheet.Cells[5 + i, 1] = "Общий долг:";

                cellName3 = ("F" + (5 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName3);
                excelcells.Font.Size = 14;
                excelcells.NumberFormat = "0.00";
                xlWorkSheet.Cells[5 + i, 6] = Math.Round(Convert.ToDouble(order.Clients.DebtS), 2);
                excelcells.EntireColumn.AutoFit();

            }
            else
            {
                cellName1 = ("A" + (4 + i)).ToString();
                cellName2 = ("E" + (4 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName1, cellName2);
                excelcells.Merge(Type.Missing);
                excelcells.Font.Size = 14;
                excelcells.HorizontalAlignment = Excel.Constants.xlRight;
                xlWorkSheet.Cells[4 + i, 1] = "Общий долг:";

                cellName3 = ("F" + (4 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName3);
                excelcells.Font.Size = 14;
                excelcells.NumberFormat = "0.00";
                xlWorkSheet.Cells[4 + i, 6] = Math.Round(Convert.ToDouble(order.Clients.DebtS), 2);
                excelcells.EntireColumn.AutoFit();

            }

            //Получаем набор ссылок на объекты Workbook (на созданные книги)
            excelappworkbooks = excelApp.Workbooks;
            //Получаем ссылку на книгу 1 - нумерация от 1
            excelappworkbook = excelappworkbooks[1];
            //Ссылку можно получить и так, но тогда надо знать имена книг,
            //причем, после сохранения - знать расширение файла
            //Запроса на сохранение для книги не должно быть
            excelappworkbook.Saved = true;
            excelApp.DefaultSaveFormat = Excel.XlFileFormat.xlWorkbookDefault;

            //Будем спрашивать разрешение на запись поверх существующего документа
            excelApp.DisplayAlerts = false;//нет, не будем
            excelappworkbook = excelappworkbooks[1];
            excelappworkbook.SaveAs(fileName);//Сохраняем книгу
            excelApp.Workbooks[1].Close();
            excelApp.Quit();

        }
        public static void SaveExcelFile(Order order, string filename)
        {
            string orderName = "Накладная от ";
            
            string fileName = filename;
            excelApp = new Excel.Application();
            excelApp.Visible = false;

            excelApp.SheetsInNewWorkbook = 1;
            excelApp.Workbooks.Add(Type.Missing);

            Excel.Workbooks excelappworkbooks = excelApp.Workbooks;
            Excel.Workbook excelappworkbook = excelappworkbooks[1];

            //Получаем ссылку на лист 1
            var xlWorkSheet = (Excel.Worksheet)excelappworkbook.Worksheets.get_Item(1);
            //Выбираем ячейку для вывода 
            var excelcells = xlWorkSheet.get_Range("A1", "F1");//Дата заказа
            excelcells.Merge(Type.Missing);
            excelcells.HorizontalAlignment = Excel.Constants.xlCenter;
            excelcells.Font.Size = 14;
            xlWorkSheet.Cells[1, 1] = orderName + order.OrderDate.ToShortDateString();

            //Заполнение строк заказа
            int i = 0;
            foreach (OrderLine ol in order.OrderLines.OrderBy(o => o.Products.Name))
            {
                excelcells = xlWorkSheet.get_Range(("A" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                xlWorkSheet.Cells[3 + i, 1] = (1 + i).ToString();//порядковый номер

                excelcells = xlWorkSheet.get_Range(("B" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                excelcells.EntireColumn.AutoFit();
                xlWorkSheet.Cells[3 + i, 2] = ol.Products.Name;//название товара

                excelcells = xlWorkSheet.get_Range(("C" + (3 + i)).ToString());
                excelcells.HorizontalAlignment = Excel.Constants.xlCenter;
                excelcells.Borders.ColorIndex = 15;
                xlWorkSheet.Cells[3 + i, 3] = ol.Products.Unit;//единица измерения

                excelcells = xlWorkSheet.get_Range(("D" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                xlWorkSheet.Cells[3 + i, 4] = Convert.ToDouble(ol.Quantity);//количество товара

                excelcells = xlWorkSheet.get_Range(("E" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                excelcells.NumberFormat = "0.000";
                xlWorkSheet.Cells[3 + i, 5] = Math.Round(Convert.ToDouble(ol.Price), 3);//цена

                excelcells = xlWorkSheet.get_Range(("F" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                excelcells.NumberFormat = "0.00";
                xlWorkSheet.Cells[3 + i, 6] = Math.Round(Convert.ToDouble(ol.Total), 2);//сумма

                i++;
            }

            //надпись "Итого:"
            string cellName1 = ("A" + (3 + i)).ToString();
            string cellName2 = ("E" + (3 + i)).ToString();
            excelcells = xlWorkSheet.get_Range(cellName1, cellName2);
            excelcells.Merge(Type.Missing);
            excelcells.HorizontalAlignment = Excel.Constants.xlRight;
            excelcells.Font.Size = 14;
            xlWorkSheet.Cells[3 + i, 1] = "Итого:";

            //Итоговая Сумма
            string cellName3 = ("F" + (3 + i)).ToString();
            excelcells = xlWorkSheet.get_Range(cellName3);
            excelcells.Font.Size = 14;
            excelcells.NumberFormat = "0.00";
            xlWorkSheet.Cells[3 + i, 6] = Math.Round(Convert.ToDouble(order.TotalS), 2);
            excelcells.EntireColumn.AutoFit();

            if (Convert.ToDouble(order.PaymentS) != 0)
            {
                cellName1 = ("A" + (4 + i)).ToString();
                cellName2 = ("E" + (4 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName1, cellName2);
                excelcells.Merge(Type.Missing);
                excelcells.Font.Size = 14;
                excelcells.HorizontalAlignment = Excel.Constants.xlRight;
                xlWorkSheet.Cells[4 + i, 1] = "Оплачено:";

                cellName3 = ("F" + (4 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName3);
                excelcells.Font.Size = 14;
                excelcells.NumberFormat = "0.00";
                xlWorkSheet.Cells[4 + i, 6] = Math.Round(Convert.ToDouble(order.PaymentS), 2);
                excelcells.EntireColumn.AutoFit();


                cellName1 = ("A" + (5 + i)).ToString();
                cellName2 = ("E" + (5 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName1, cellName2);
                excelcells.Merge(Type.Missing);
                excelcells.Font.Size = 14;
                excelcells.HorizontalAlignment = Excel.Constants.xlRight;
                xlWorkSheet.Cells[5 + i, 1] = "Общий долг:";

                cellName3 = ("F" + (5 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName3);
                excelcells.Font.Size = 14;
                excelcells.NumberFormat = "0.00";
                xlWorkSheet.Cells[5 + i, 6] = Math.Round(Convert.ToDouble(order.Clients.DebtS), 2);
                excelcells.EntireColumn.AutoFit();
            }
            else
            {
                cellName1 = ("A" + (4 + i)).ToString();
                cellName2 = ("E" + (4 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName1, cellName2);
                excelcells.Merge(Type.Missing);
                excelcells.Font.Size = 14;
                excelcells.HorizontalAlignment = Excel.Constants.xlRight;
                xlWorkSheet.Cells[4 + i, 1] = "Общий долг:";

                cellName3 = ("F" + (4 + i)).ToString();
                excelcells = xlWorkSheet.get_Range(cellName3);
                excelcells.Font.Size = 14;
                excelcells.NumberFormat = "0.00";
                xlWorkSheet.Cells[4 + i, 6] = Math.Round(Convert.ToDouble(order.Clients.DebtS), 2);
            }

            //Получаем набор ссылок на объекты Workbook (на созданные книги)
            excelappworkbooks = excelApp.Workbooks;
            //Получаем ссылку на книгу 1 - нумерация от 1
            excelappworkbook = excelappworkbooks[1];
            //Ссылку можно получить и так, но тогда надо знать имена книг,
            //причем, после сохранения - знать расширение файла
            //Запроса на сохранение для книги не должно быть
            excelappworkbook.Saved = true;
            excelApp.DefaultSaveFormat = Excel.XlFileFormat.xlWorkbookDefault;

            //Будем спрашивать разрешение на запись поверх существующего документа
            excelApp.DisplayAlerts = false;//нет, не будем
            excelappworkbook = excelappworkbooks[1];
            excelappworkbook.SaveAs(fileName);//Сохраняем книгу
            excelApp.Workbooks[1].Close();
            excelApp.Quit();

        }
        public static void SaveExcelFile(Client client)
        {
            Model1 _entities = new Model1();
            string fileName = @"D:\Baraban Orders\" + (client.Name + "_Price_" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day).ToString();
            excelApp = new Excel.Application();
            excelApp.Visible = false;

            excelApp.SheetsInNewWorkbook = 1;
            excelApp.Workbooks.Add(Type.Missing);

            Excel.Workbooks excelappworkbooks = excelApp.Workbooks;
            Excel.Workbook excelappworkbook = excelappworkbooks[1];

            //Получаем ссылку на лист 1
            var xlWorkSheet = (Excel.Worksheet)excelappworkbook.Worksheets.get_Item(1);
            //Выбираем ячейку для вывода 
            var excelcells = xlWorkSheet.get_Range("A1", "D1");//Дата заказа
            excelcells.Merge(Type.Missing);
            excelcells.Font.Bold = true;
            excelcells.HorizontalAlignment = Excel.Constants.xlCenter;
            excelcells.Font.Size = 14;
            xlWorkSheet.Cells[1, 1] = "Прайслист " + "    " + DateTime.Now.ToShortDateString();

            //Заполнение строк заказа
            int i = 0;
            foreach (Product p in _entities.Products.Where(c=>c.ClientId == client.Id).OrderBy(o => o.Name))
            {
                excelcells = xlWorkSheet.get_Range(("A" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                excelcells.EntireColumn.AutoFit();
                xlWorkSheet.Cells[3 + i, 1] = (1 + i).ToString();//порядковый номер

                excelcells = xlWorkSheet.get_Range(("B" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                excelcells.EntireColumn.AutoFit();
                xlWorkSheet.Cells[3 + i, 2] = p.Name;//название товара

                excelcells = xlWorkSheet.get_Range(("C" + (3 + i)).ToString());
                excelcells.HorizontalAlignment = Excel.Constants.xlCenter;
                excelcells.Borders.ColorIndex = 15;
                xlWorkSheet.Cells[3 + i, 3] = p.Unit;//единица измерения


                excelcells = xlWorkSheet.get_Range(("D" + (3 + i)).ToString());
                excelcells.Borders.ColorIndex = 15;
                excelcells.NumberFormat = "0.000";
                xlWorkSheet.Cells[3 + i, 4] = Math.Round(Convert.ToDouble(p.PriceEnter), 3);//цена

                i++;
            }

            //Получаем набор ссылок на объекты Workbook (на созданные книги)
            excelappworkbooks = excelApp.Workbooks;
            //Получаем ссылку на книгу 1 - нумерация от 1
            excelappworkbook = excelappworkbooks[1];
            //Ссылку можно получить и так, но тогда надо знать имена книг,
            //причем, после сохранения - знать расширение файла
            //Запроса на сохранение для книги не должно быть
            excelappworkbook.Saved = true;
            excelApp.DefaultSaveFormat = Excel.XlFileFormat.xlWorkbookDefault;

            //Будем спрашивать разрешение на запись поверх существующего документа
            excelApp.DisplayAlerts = false;//нет, не будем
            excelappworkbook = excelappworkbooks[1];
            excelappworkbook.SaveAs(fileName);//Сохраняем книгу
            excelApp.Workbooks[1].Close();
            excelApp.Quit();
            MessageBox.Show("Прайслист успешно сохранен в файл\n" + fileName, "Сохранение прайслиста", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public static void DeleteExcelFile(Order order, string filename)
        {
            string fileName;
            excelApp = new Excel.Application();
            if (excelApp.Version.ToString() == "15.0")
            {
                fileName = filename + ".xlsx";
            }
            else
            {
                fileName = filename + ".xls";
            }
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
            else MessageBox.Show("Файл не существует");
        }
    }
}
