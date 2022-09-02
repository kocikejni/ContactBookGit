
using ContactBook.ViewModel;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ContactBook.Controllers
{
    public class HomeController : Controller
    {

        
        public ActionResult Index()
        {
            List<ContactModel> contacts = new List<ContactModel>();
            using (MyContactBookEntities dc = new MyContactBookEntities())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             FirstName = a.ContactFirstName,
                             LastName = a.ContactLastName,
                             ContactNo1 = a.ContactNo1,
                             ContactNo2 = a.ContactNo2,
                             Email = a.Email,
                             Country = b.CountryName,
                             Adress = a.Adress
                         }).ToList();
                contacts = v;
            }
            return View(contacts);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult Add()
        {
            List<Country> AllCountry = new List<Country>();
            using (MyContactBookEntities dc = new MyContactBookEntities())
            {
                AllCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();
            }
            ViewBag.Country = new SelectList(AllCountry, "CountryID", "CountryName");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Contact c)
        {
            
            if (ModelState.IsValid)
            {
                using (MyContactBookEntities dc = new MyContactBookEntities())
                {
                    dc.Contacts.Add(c);
                    dc.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
            
        }

        [Authorize]
        public ActionResult View(int id)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qRCodeGenerator.CreateQrCode("https://localhost:44369/Home/View/"+ id.ToString(), QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                using (Bitmap bitmap = qrCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }

            }

            Contact c = null;
            c = GetContact(id);
            return View(c);
        }

        public ActionResult ViewContact (int id)
        {
            using (MyContactBookEntities dc = new MyContactBookEntities())
            {
                var contact = dc.Contacts.FirstOrDefault(x => x.ContactID == id);
            }
            return View();
        }

        [Authorize(Roles ="Admin")]
        private Contact GetContact(int contactID)
        {
            Contact contact = null;
            using (MyContactBookEntities dc = new MyContactBookEntities())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         where a.ContactID.Equals(contactID)
                         select new
                         {
                             a,
                             b.CountryName

                         }).FirstOrDefault();
                if (v != null)
                {
                    contact = v.a;
                    contact.CountryName = v.CountryName;
                }

            }
            return contact;
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Contact c = null;
            c = GetContact(id);

            if (c == null)
            {
                return HttpNotFound("Contact not found!");
            }
            List<Country> allCountry = new List<Country>();
            using (MyContactBookEntities dc = new MyContactBookEntities())
            {
                allCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();
            }
            ViewBag.Country = new SelectList(allCountry, "CountryID", "CountryName", c.CountryName);

            return View(c);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contact c)
        {
            if (ModelState.IsValid)
            {
                using (MyContactBookEntities dc = new MyContactBookEntities())
                {
                    var v = dc.Contacts.Where(a => a.ContactID.Equals(c.ContactID)).FirstOrDefault();
                    if ( v != null)
                    {
                        
                        v.ContactFirstName = c.ContactFirstName;
                        v.ContactLastName = c.ContactLastName;
                        v.ContactNo1 = c.ContactNo1;
                        v.ContactNo2 = c.ContactNo2;
                        v.Email = c.Email;
                        v.CountryID = c.CountryID;
                        v.Adress = c.Adress;
                        
                    }
                    dc.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(c);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {

            Contact c = null;
            c = GetContact(id);
            return View(c);
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            using (MyContactBookEntities dc = new MyContactBookEntities())
            {
                var contact = dc.Contacts.Where(a => a.ContactID.Equals(id)).FirstOrDefault();
                if(contact != null)
                {
                    dc.Contacts.Remove(contact);
                    dc.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound("Contact not found");
                }
            }
            
        }

        public ActionResult Search(string searching)
        {
            MyContactBookEntities db = new MyContactBookEntities();

            return View(db.Contacts.Where(x => x.ContactFirstName.Contains(searching) || (searching) == null).ToList());
        }

        [Authorize]
        public ActionResult Export()
        {
            List<ContactModel> allContacts = new List<ContactModel>();
            using (MyContactBookEntities dc = new MyContactBookEntities())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             FirstName = a.ContactFirstName,
                             LastName = a.ContactLastName,
                             ContactNo1 = a.ContactNo1,
                             ContactNo2 = a.ContactNo2,
                             Email = a.Email,
                             Country = b.CountryName,
                             Adress = a.Adress


                         }).ToList();
                allContacts = v;
                         
            }
            return View(allContacts);
        }

        
        [HttpPost]
        [ActionName("Export")]
        public FileResult ExportData()
        {
            List<ContactModel> allContacts = new List<ContactModel>();
            using (MyContactBookEntities dc = new MyContactBookEntities())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         select new ContactModel
                         {
                             ContactID = a.ContactID,
                             FirstName = a.ContactFirstName,
                             LastName = a.ContactLastName,
                             ContactNo1 = a.ContactNo1,
                             ContactNo2 = a.ContactNo2,
                             Email = a.Email,
                             Country = b.CountryName,
                             Adress = a.Adress
                         }).ToList();
                allContacts = v;
            }

            var grid = new WebGrid(source: allContacts, canPage: false, canSort: false);
            string exportData = grid.GetHtml(
                            columns: grid.Columns(
                                        grid.Column("ContactID", "Contact ID"),
                                        grid.Column("FirstName", "First Name"),
                                        grid.Column("LastName", "Last Name"),
                                        grid.Column("ContactNo1", "Contact No1"),
                                        grid.Column("ContactNo2", "Contact No2"),
                                        grid.Column("Email", "Email")
                                    )
                                ).ToHtmlString();
            return File(new System.Text.UTF8Encoding().GetBytes(exportData),
                    "application/vnd.ms-excel",
                    "Contacts.xls");
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult Import(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if(postedFile != null)
            {
                string path = Server.MapPath("~/Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls":
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx":
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                conString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.Contacts";

                        sqlBulkCopy.ColumnMappings.Add("ContactID", "ContactID");
                        sqlBulkCopy.ColumnMappings.Add("ContactFirstName", "ContactFirstName");
                        sqlBulkCopy.ColumnMappings.Add("ContactLastName", "ContactLastName");
                        sqlBulkCopy.ColumnMappings.Add("ContactNo1", "ContactNo1");
                        sqlBulkCopy.ColumnMappings.Add("ContactNo2", "ContactNo2");
                        sqlBulkCopy.ColumnMappings.Add("Email", "Email");
                        sqlBulkCopy.ColumnMappings.Add("Country", "Country");
                        sqlBulkCopy.ColumnMappings.Add("Adress", "Adress");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }
            return View();
        }
        /*
        public ActionResult QrCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QrCode(string qrcode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qRCodeGenerator.CreateQrCode(qrcode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                using (Bitmap bitmap = qrCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }

            }

            return View();
        }
        public ActionResult GenerateQrCodePerUser(string userId)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qRCodeGenerator.CreateQrCode(userId, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                using (Bitmap bitmap = qrCode.GetGraphic(20))
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }

            }

            return View();
        }
        */

    }
}