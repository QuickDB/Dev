using QuickDB.Core.Session;

using System;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    public class HomeController : Controller
    {
        public class Order
        {
            public long OrderNumber { set; get; }

            public string OrderItem { get; set; }
        }

        private string item = "QuickDB" + DateTime.UtcNow;
        private long orderNumber = DateTime.UtcNow.Ticks;
        private string documentId = "order1000";
        private long sampleValue = 100;

        public ActionResult Index()
        {
            using (var session = new QuickDBSessionFor<Order>())
            {
                try
                {
                    ViewBag.raw = session.Administration.LoadRawModel();
                }
                catch (Exception)
                {
                    ViewBag.raw = "";
                }
                return View();
            }
        }

        public ActionResult Create()
        {
            using (var session = new QuickDBSessionFor<Order>())
            {
                var order = session.LoadAndCreateIfItDoesntExist();
                order.OrderItem = item;
                order.OrderNumber = orderNumber;
                session.SaveChanges();
                //todo : save changes in atransaction
                var order2 = session.LoadAndCreateIfItDoesntExist(documentId);
                order2.OrderItem = sampleValue.ToString();
                order2.OrderNumber = sampleValue;
                session.SaveChanges();
            }

            return View();
        }

        public ActionResult Clean()
        {
            using (var session = new QuickDBSessionFor<Order>())
            {
                try
                {
                    session.Administration.DeleteDocumentPermanently();
                    session.Administration.DeleteDocumentPermanently(documentId);
                }
                catch (Exception)
                {
                }

                return View();
            }
        }
    }
}