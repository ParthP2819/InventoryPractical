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
            var prodCs = "";

            double TotalPurchase_qty = 0;
            double Purchase_price = 0;
            double Sale_qty = 0;
            double Sale_price = 0;
                      
            double profitloss = 0;
            double TotalPurcharseAmt = 0;
            double totalSale = 0;



            List<Inventory> inventory = new List<Inventory>();
            List<InnVM> inventoryVMs = new List<InnVM>();

            // creating a list to store ExcelData
            

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
                    prodCs = "";

                    TotalPurchase_qty = 0;
                    Purchase_price = 0;
                    Sale_qty = 0;
                    Sale_price = 0;

                    profitloss = 0;
                    TotalPurcharseAmt = 0;
                    totalSale = 0;                    
                    double closeQty = 0;
                    double openQty = 0;
                    Purchase_price= 0;
                    foreach (var type in data)
                    {
                        var acp = inventoryVMs.LastOrDefault(x => x.ProductCode == productcode.ToString());
                        date = type.Date;
                        productcode = type.ProductCode;
                        if (type.EventType == 1)
                        {
                           TotalPurchase_qty += type.Quantity;
                            TotalPurcharseAmt = TotalPurcharseAmt + (Convert.ToDouble(type.Price) * type.Quantity);
                        }
                        else
                        {
                            Sale_qty += type.Quantity;
                            Sale_price += type.Price;
                            totalSale = Convert.ToDouble(Sale_qty) * Sale_price;
                            if (Purchase_price == 0 && acp != null)
                            {
                                profitloss = (Sale_price * Sale_qty) - (Sale_qty * acp.Total_Purchase_Amount);
                            }
                            profitloss = (Sale_price * Sale_qty) - (Sale_qty * Purchase_price);

                        }
                        Purchase_price = TotalPurcharseAmt/ TotalPurchase_qty;

                    }
                    var acp2 = inventoryVMs.LastOrDefault(x => x.ProductCode == productcode.ToString());

                    if (acp2 != null)
                    {
                        inventoryVMs.Add(new InnVM
                        {
                            Date = date.ToString("d/MMMM/yyyy"),
                            ProductCode = productcode,
                            Purchase_Price = TotalPurcharseAmt/TotalPurchase_qty,
                            Total_Purchase_Quantity = Convert.ToDouble(TotalPurchase_qty),
                            Total_Purchase_Amount = TotalPurcharseAmt,
                            Total_Sale_Quantity = Sale_qty.ToString(),
                            Total_Sale_Amount = Convert.ToString(totalSale),
                            Profit_Loss = ((Sale_qty * Sale_price) - (acp2.Purchase_Price * TotalPurchase_qty)).ToString(),
                            Closing_Quantity = (acp2.Closing_Quantity) + TotalPurchase_qty - Sale_qty,
                            Opening_Quantity = acp2.Closing_Quantity 
                            
                        });
                    }
                    else
                    {
                        inventoryVMs.Add(new InnVM
                        {
                            Date = date.ToString("d/MMMM/yyyy"),
                            ProductCode = productcode,
                            Purchase_Price = TotalPurcharseAmt / TotalPurchase_qty,
                            Total_Purchase_Quantity = Convert.ToDouble(TotalPurchase_qty),
                            Total_Purchase_Amount = TotalPurcharseAmt,
                            Total_Sale_Quantity = Sale_qty.ToString(),
                            Total_Sale_Amount = Convert.ToString(totalSale),
                            Profit_Loss = profitloss.ToString(),
                            Closing_Quantity = (TotalPurchase_qty - Sale_qty),
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