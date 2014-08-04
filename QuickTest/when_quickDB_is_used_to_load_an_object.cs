using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickDB.Core.Session;
using System;

namespace QuickTest
{
    [TestClass]
    public class when_quickDB_is_used_to_load_an_object
    {
        public class Order
        {
            public long OrderNumber { set; get; }

            public string OrderItem { get; set; }
        }

        [TestMethod]
        public void it_should_be_able_to_persist_changes_and_reload_it_back_from_disc()
        {
            //Arrange
            var item = "QuickDB" + DateTime.UtcNow;
            var orderNumber = DateTime.UtcNow.Ticks;
            const string documentId = "order1000";
            const int sampleValue = 100;
            

            using (var session = new QuickDBSessionFor<Order>())
            {
                //load default instance,creating new
                var order = session.LoadNew();

                //Acts
                order.OrderItem = item;
                order.OrderNumber = orderNumber;
                session.SaveChanges();

                var retrievedOrder = session.Load();

                //Assert
                Assert.AreEqual(orderNumber, retrievedOrder.OrderNumber);
                Assert.AreEqual(item, retrievedOrder.OrderItem);
                Assert.AreNotEqual(sampleValue, retrievedOrder.OrderNumber);
                Assert.AreNotEqual(sampleValue.ToString(), retrievedOrder.OrderItem);
            }

            using (var session = new QuickDBSessionFor<Order>())
            {
                //load instance based on documentId,creating new
                var order = session.LoadNew(documentId);
                order.OrderItem = sampleValue.ToString();
                order.OrderNumber = sampleValue;
                session.SaveChanges();

                var retrievedOrder = session.Load(documentId);

                //Assert
                Assert.AreEqual(sampleValue, retrievedOrder.OrderNumber);
                Assert.AreEqual(sampleValue.ToString(), retrievedOrder.OrderItem);
                Assert.AreNotEqual(orderNumber, retrievedOrder.OrderNumber);
                Assert.AreNotEqual(item, retrievedOrder.OrderItem);
            }


           //delete the two documents created
            var newSession = new QuickDBSessionFor<Order>();
            newSession.Administration.DeleteDocumentPermanently();
            newSession.Administration.DeleteDocumentPermanently(documentId);

        }
    }
}