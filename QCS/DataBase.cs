using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCS
{
    class DataBase
    {
        public class UserContext : DbContext
        {
            public UserContext()
                : base("DbConnection")
            { }

            public DbSet<Product> Products { get; set; }
            public DbSet<Batch> Batchs { get; set; }
            public DbSet<Requirement> Requirements { get; set; }
            public DbSet<Norm> Norms { get; set; }
            public DbSet<Test> Tests { get; set; }
            public DbSet<Certificate> Certificates { get; set; }
            public DbSet<Act> Acts { get; set; }
        }
    }

    [Table("Product")]
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string PName { get; set; }
        public decimal Cost { get; set; }
    }

    [Table("Batch")]
    public class Batch
    {
        [Key]
        public int BatchID { get; set; }
        public int ProductID { get; set; }
        public DateTime ProductionDate { get; set; }
        public int Counts { get; set; }

        public Product Product { get; set; }
    }

    [Table("Requirement")]
    public class Requirement
    {
        [Key]
        public int RequirementID { get; set; }
        public string RName { get; set; }
    }

    [Table("Norm")]
    public class Norm
    {
        [Key]
        public int NormID { get; set; }
        public int ProductID { get; set; }
        public int RequirementID { get; set; }
        public string NValue { get; set; }

        public Norm()
        {
        }

        public Norm (NewNorm newNorm)
        {
            NormID = newNorm.NormID;
            ProductID = newNorm.ProductID;
            RequirementID = newNorm.RequirementID;
            NValue = newNorm.NValue;
        }
    }

    public class NewNorm
    {
        [Key]
        public int NormID { get; set; }
        public int ProductID { get; set; }
        public int RequirementID { get; set; }
        public string NValue { get; set; }
        public Product Product { get; set; }
        public Requirement Requirement { get; set; }

        public NewNorm() { }

        public NewNorm(string nValue, int productID, int requirementID)
        {
            NValue = nValue;
            ProductID = productID;
            RequirementID = requirementID;
        }

        public NewNorm(Norm norm)
        {
            NormID = norm.NormID;
            ProductID = norm.ProductID;
            RequirementID = norm.RequirementID;
            NValue = norm.NValue;
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                foreach (Requirement requirement in db.Requirements)
                {
                    foreach (Product product in db.Products)
                    {
                        if (product.ProductID == norm.ProductID && requirement.RequirementID == norm.RequirementID)
                        {
                            Product = product;
                            Requirement = requirement;
                            break;
                        }
                    }
                }
            }
        }
    }

    [Table("Test")]
    public class Test
    {
        [Key]
        public int TestID { get; set; }
        public int BatchID { get; set; }
        public int NormID { get; set; }
        public string TValue { get; set; }
        public DateTime TestDate { get; set; }
        public bool Result { get; set; }

        public Test(NewTest newTest)
        {
            BatchID = newTest.BatchID;
            NormID = newTest.NormID;
            TValue = newTest.TValue;
            TestDate = newTest.TestDate;
            Result = newTest.Result;
        }

        public Test() 
        {
            TestDate = DateTime.Now;
        }
    }

    public class NewTest
    {
        [Key]
        public int TestID { get; set; }
        public int BatchID { get; set; }
        public int NormID { get; set; }
        public string TValue { get; set; }
        public DateTime TestDate { get; set; }
        public bool Result { get; set; }
        public Batch Batch { get; set; }
        public Norm Norm { get; set; }
        public Product Product { get; set; }
        public Requirement Requirement { get; set; }

        public NewTest(Test test)
        {
            TestID = test.TestID;
            BatchID = test.BatchID;
            NormID = test.NormID;
            TValue = test.TValue;
            TestDate = test.TestDate;
            Result = test.Result;
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                foreach (Norm norm in db.Norms)
                {
                    foreach (Batch batch in db.Batchs)
                    if (norm.NormID == NormID && batch.BatchID == BatchID)
                    {
                        Norm = norm;
                        Batch = batch;
                    }
                }

                foreach (Batch batch in db.Batchs)
                {
                    if (batch.BatchID == BatchID)
                    {
                        Batch = batch;
                        break;
                    }
                }

                foreach (Requirement requirement in db.Requirements)
                {
                    foreach (Product product in db.Products)
                    {
                        if (product.ProductID == Norm.ProductID && requirement.RequirementID == Norm.RequirementID)
                        {
                            Product = product;
                            Requirement = requirement;
                            break;
                        }
                    }
                }
            }
        }

        public NewTest(Product product, Batch batch, Requirement requirement)
        {
            BatchID = batch.BatchID;
            TValue = "-";
            TestDate = DateTime.Now;
            Product = product;
            Batch = batch;
            Requirement = requirement;
            Result = false;
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                foreach (Norm norm in db.Norms)
                {
                    if (norm.RequirementID == Requirement.RequirementID && Product.ProductID == norm.ProductID)
                    {
                        Norm = norm;
                        break;
                    }
                }
            }
            NormID = Norm.NormID;
        }

        public NewTest() { }
    }

    [Table("Сertificate")]
    public class Certificate
    {
        [Key]
        public int СertificateID { get; set;}
        public int BatchID { get; set;}
        public DateTime RegistrationDate { get; set; }

        public Certificate(Test test)
        {
            BatchID = test.BatchID;
            RegistrationDate = DateTime.Now;
        }

        public Certificate() { }
    }

    [Table("Act")]
    public class Act
    {
        [Key]
        public int ActID { get; set; }
        public int BatchID { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Act(Test test)
        {
            BatchID = test.BatchID;
            RegistrationDate = DateTime.Now;
        }

        public Act() { }
    }
}
