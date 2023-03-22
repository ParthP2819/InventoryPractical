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
            List<Inventory> inventory = new List<Inventory>();
            List<InnVM> inventoryVMs = new List<InnVM>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
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

            var month = inventory.GroupBy(x => x.Date.Month).ToList();
            foreach (var item in month)
            {
                var product = item.GroupBy(x => x.ProductCode).ToList();
                foreach (var data in product)
                {
                    var date = "";
                    int totalpurchaseqty = 0;
                    double totalpur_Amt = 0;
                    int totalsaleqty = 0;
                    double totalsale_amt = 0;
                    double profitloss = 0;
                    double Pamount = 0;
                    double Samount = 0;
                    double saletotal = 0;

                    foreach (var type in data)
                    {
                        totalpurchaseqty = 0;
                        Pamount = 0;
                        totalsaleqty = 0;
                        Samount = 0;
                        if (type.EventType == 1)
                        {
                            totalpurchaseqty = type.Quantity;
                            Pamount = type.Price;
                            totalpur_Amt = totalpurchaseqty * Pamount;
                        }
                        else
                        {
                            totalsaleqty = type.Quantity;
                            Samount = type.Price;
                            totalsale_amt = totalsaleqty * Samount;
                            profitloss = (totalsale_amt) - (totalsaleqty * Pamount);
                        }
                        inventoryVMs.Add(new InnVM
                        {
                            Date = type.Date.ToShortDateString(),
                            ProductCode = type.ProductCode,
                            TotalPurchaseQty = totalpurchaseqty.ToString(),
                            Total_Pur_Amt = Pamount.ToString(),
                            Total_Sale_Qty = totalsaleqty.ToString(),
                            Total_Sale_Amt = Samount.ToString(),
                            Profit_Loss = profitloss.ToString()
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