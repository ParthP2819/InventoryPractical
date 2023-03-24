using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using InventoryPractical.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryPractical.Controllers
{
    public class FileController : Controller
    {
        static dynamic Tdata;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile file)
        {
            var prodCod = "";

            double Purchase_qty = 0;
            double Purchase_price = 0;
            double Sale_qty = 0;
            double Sale_price = 0;
                      
            double profitloss = 0;
            double totalPurcharse = 0;
            double totalSale = 0;



            List<Inventory> inventory = new List<Inventory>();
            List<InnVM> inventoryVMs = new List<InnVM>();

            // creating a list to store ExcelData
            //var excelData = new List<Inventory>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // opening the excel file using package ExcelDataReader
            using (var stream = file.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read()) //Each row of the file
                    {
                        inventory.Add(new Inventory
                        {
                            ProductCode = reader.GetValue(0).ToString(),
                            EventType = int.Parse(reader.GetValue(1).ToString()),
                            Quantity = int.Parse(reader.GetValue(2).ToString()),
                            Price = double.Parse(reader.GetValue(3).ToString()),
                            Date = DateTime.Parse(reader.GetValue(4).ToString())
                        });
                    }
                }
            }

            // groupby month-wise
            var month = inventory.GroupBy(x => x.Date.Month).ToList();

            foreach (var item in month)
            {
                // groupby product-wise
                var product = item.GroupBy(x => x.ProductCode).ToList();

                foreach (var data in product)
                {                                
                    var date = DateTime.Now;
                    var productcode = "";
                    prodCod = "";

                    Purchase_qty = 0;
                    Purchase_price = 0;
                    Sale_qty = 0;
                    Sale_price = 0;

                    profitloss = 0;
                    totalPurcharse = 0;
                    totalSale = 0;                    
                    double closeQty = 0;
                    double openQty = 0;
                    Purchase_price= 0;
                    foreach (var type in data)
                    {
                        var resultOpen1 = inventoryVMs.LastOrDefault(x => x.ProductCode == productcode.ToString());
                        date = type.Date;
                        productcode = type.ProductCode;
                        if (type.EventType == 1)
                        {
                            Purchase_qty += type.Quantity;
                            Purchase_price += type.Price;
                            totalPurcharse = totalPurcharse + (Convert.ToDouble(type.Price) * type.Quantity);
                        }
                        else
                        {
                            Sale_qty += type.Quantity;
                            Sale_price += type.Price;
                            totalSale = Convert.ToDouble(Sale_qty) * Sale_price;
                            if(Purchase_price==0  && resultOpen1!=null)
                            {
                                profitloss = (Sale_price * Sale_qty) - (Sale_qty * resultOpen1.Total_Purchase_Amount);
                            }
                            profitloss = (Sale_price * Sale_qty) - (Sale_qty * Purchase_price);

                        }
                    }
                    var resultOpen = inventoryVMs.LastOrDefault(x => x.ProductCode == productcode.ToString());

                    if (resultOpen != null)
                    {
                        inventoryVMs.Add(new InnVM
                        {
                            Date = date.ToString("d/MMMM/yyyy"),
                            ProductCode = productcode,
                            Total_Purchase_Quantity = Convert.ToDouble(Purchase_qty),
                            Total_Purchase_Amount = totalPurcharse,
                            Total_Sale_Quantity = Sale_qty.ToString(),
                            Total_Sale_Amount = Convert.ToString(totalSale),
                            Profit_Loss = profitloss.ToString(),
                            //Closing_Quantity = 0,
                            Closing_Quantity = (resultOpen.Closing_Quantity) + Purchase_qty - Sale_qty,
                            Opening_Quantity = resultOpen.Closing_Quantity 
                        });
                    }
                    else
                    {
                        inventoryVMs.Add(new InnVM
                        {
                            Date = date.ToString("d/MMMM/yyyy"),
                            ProductCode = productcode,
                            Total_Purchase_Quantity = Convert.ToDouble(Purchase_qty),
                            Total_Purchase_Amount = totalPurcharse,
                            Total_Sale_Quantity = Sale_qty.ToString(),
                            Total_Sale_Amount = Convert.ToString(totalSale),
                            Profit_Loss = profitloss.ToString(),
                            Closing_Quantity = (Purchase_qty - Sale_qty),
                            Opening_Quantity = 0  
                        });
                    }                                       
                }
            }
            Tdata = inventoryVMs.ToList();
            return RedirectToAction("Read");
        }
        public IActionResult Read()
        {
            return View(Tdata);
        }     
    }
}  