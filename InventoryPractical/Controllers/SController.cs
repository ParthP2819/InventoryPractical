using ExcelDataReader;
using InventoryPractical.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryPractical.Controllers
{
    public class SController : Controller
    {
        public IActionResult Index()
        {
            return View(new List<InnVM>());
        }
    }
}
//---------------------------------------------------------------------------
//namespace InventoryExcel.Controllers
//{
//    public class InventoryController : Controller
//    {
        
//        [HttpPost]
//        public IActionResult Index(IFormFileCollection form)
//        {
//            List<Inventory> inventory = new List<Inventory>();
//            //var filepath = "C:\\Users\\sumant.kumar\\Desktop\\inevtory task.xlsx";
//            var filepath = "C:\\Users\\sumant.kumar\\Desktop\\Practice.xlsx";
//            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
//            using (var stream = System.IO.File.Open(filepath, FileMode.Open, FileAccess.Read))
//            {
//                using (var reader = ExcelReaderFactory.CreateReader(stream))
//                {

//                    while (reader.Read()) //Each row of the file
//                    {
//                        inventory.Add(new Inventory
//                        {
//                            ProductCode = reader.GetValue(0).ToString(),
//                            EventType = int.Parse(reader.GetValue(1).ToString()),
//                            Quantity = int.Parse(reader.GetValue(2).ToString()),
//                            Price = double.Parse(reader.GetValue(3).ToString()),
//                            Date = DateTime.Parse(reader.GetValue(4).ToString()),
//                        });
//                    }
//                }

//                List<InnVM> inventories = new List<InnVM>();
//                var monthwise = inventory.GroupBy(x => x.Date.Month).ToList();
//                InventoryVM abc = new InventoryVM();
//                foreach (var month in monthwise)
//                {

//                    var productcode = month.GroupBy(x => x.ProductCode).ToList();

//                    foreach (var product in productcode)
//                    {
//                        abc.ClosingQuantity = 0;
//                        abc.TotalPurQuan = 0;
//                        abc.TotalSaleQuan = 0;
//                        abc.TotalPurchseAmt = 0;
//                        abc.TotalSaleAmt = 0;

//                        foreach (var item in product)
//                        {
//                            if (item.EventType == 1)
//                            {
//                                abc.TotalPurQuan += item.Quantity;
//                                abc.TotalPurchseAmt += item.Quantity * item.Price;
//                            }
//                            else
//                            {
//                                abc.TotalSaleQuan += item.Quantity;
//                                abc.TotalSaleAmt += abc.TotalSaleQuan * item.Price;
//                            }



//                            abc.SellPrice = abc.TotalSaleAmt / abc.TotalSaleQuan;
//                            abc.ProductCode = item.ProductCode;
//                            abc.Month = item.Date.ToString("MMMM-yyyy");


//                        }



//                        var a = inventories.LastOrDefault(x => x.ProductCode == abc.ProductCode);


//                        if (a != null)
//                        {
//                            if (abc.TotalPurQuan != 0)
//                            {
//                                abc.CostPrice = ((abc.TotalPurchseAmt + (a.ClosingQuantity * a.CostPrice)) / (a.ClosingQuantity + abc.TotalPurQuan));
//                            }
//                            else
//                            {
//                                abc.CostPrice = a.CostPrice;
//                            }
//                            inventories.Add(new InventoryVM
//                            {
//                                CostPrice = abc.CostPrice,
//                                SellPrice = abc.SellPrice,
//                                ProfitOrLoss = Math.Round(((abc.SellPrice - abc.CostPrice) * abc.TotalSaleQuan), 2),
//                                TotalPurchseAmt = abc.TotalPurchseAmt,
//                                TotalSaleAmt = abc.TotalSaleAmt,
//                                Month = abc.Month,
//                                ProductCode = abc.ProductCode,
//                                TotalPurQuan = abc.TotalPurQuan,
//                                TotalSaleQuan = abc.TotalSaleQuan,
//                                ClosingQuantity = (abc.TotalPurQuan + a.ClosingQuantity) - abc.TotalSaleQuan,
//                                OpeningQuantity = a.ClosingQuantity,
//                            });
//                        }
//                        else
//                        {

//                            abc.CostPrice = abc.TotalPurchseAmt / abc.TotalPurQuan;


//                            inventories.Add(new InventoryVM
//                            {
//                                ProfitOrLoss = (abc.SellPrice - abc.CostPrice) * abc.TotalSaleQuan,
//                                TotalPurchseAmt = abc.TotalPurchseAmt,
//                                TotalSaleAmt = abc.TotalSaleAmt,
//                                Month = abc.Month,
//                                ProductCode = abc.ProductCode,
//                                TotalPurQuan = abc.TotalPurQuan,
//                                TotalSaleQuan = abc.TotalSaleQuan,
//                                ClosingQuantity = abc.TotalPurQuan - abc.TotalSaleQuan,
//                                OpeningQuantity = 0,
//                                CostPrice = abc.CostPrice,
//                                SellPrice = abc.SellPrice
//                            });
//                        }
//                    }

//                }

//                return View(inventories);
//            }
//        }

//    }
//}
