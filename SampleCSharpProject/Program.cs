using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SampleCSharpProject
{
    class Program
    {
        static void Main()
        {
            TryCreateTable();
            Console.WriteLine("Simple DB Manipulation Program (C#)\n".ToUpper());
            while (true)
            {
                DisplayMenu();
            }
        }

        /// <summary>
        /// This method attempts to create the SampleProducts SQL Table
        /// Prints an error if table already exists.
        /// </summary>
        static void TryCreateTable()
        {
            using (SqlConnection conn = new SqlConnection(
        SampleCSharpProject.Properties.Settings.Default.SampleProductsConnectionString))
            {
                conn.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                        "CREATE TABLE SampleProducts1 (ItemNum INT, ItemName TEXT, ItemDesc TEXT)", conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Table found, new table not created.".ToUpper());
                }
            }
        }

        /// <summary>
        /// Menu organization
        /// </summary>
        static void DisplayMenu()
        {
            Console.WriteLine("Enter desired operation: DISPLAY, DELETE, UPDATE, INSERT, EXIT");
            Console.Write("\n =>");
            string[] input = Console.ReadLine().Split(',');
            string switch_on = input[0].ToUpper();
            switch (switch_on)
            {
                case "DISPLAY": // Displays products currently in DB
                    Console.WriteLine("Current Product Entries:");
                    DisplayProducts();
                    break;

                case "DELETE":  // Dialog to delete a record from DB
                    Console.WriteLine("Enter the item number to be deleted.");
                    Console.Write("\n =>");
                    string[] toDelete = Console.ReadLine().Split(',');
                    int deleteVal = 0;
                    while (!Int32.TryParse(toDelete[0], out deleteVal))
                    {
                        Console.WriteLine("Enter the item number to be deleted.");
                        Console.Write("\n =>");
                        toDelete = Console.ReadLine().Split(',');
                    }
                    try
                    {
                        RemoveProduct(Convert.ToInt32(toDelete[0]));
                    }
                    catch
                    {
                        Console.WriteLine("Could not Delete record from DB. Please verify Item Number and try again.\n");
                    }
                    break;

                case "UPDATE":  // Dialog to Update a current record in DB
                    Console.WriteLine("Enter the item number to be updated.");
                    Console.Write("\n =>");
                    string[] toUpdate = Console.ReadLine().Split(',');
                    int updateVal = 0;
                    while (!Int32.TryParse(toUpdate[0], out updateVal))
                    {
                        Console.WriteLine("Enter the item number to be updated.");
                        Console.Write("\n =>");
                        toUpdate = Console.ReadLine().Split(',');
                    }
                    Console.WriteLine("Enter the new item name and item description to be updated to.\n" +
                        "Enter in the form: new name, new description");
                    Console.Write("\n =>");
                    string[] newData = Console.ReadLine().Split(',');
                    UpdateProduct(int.Parse(toUpdate[0]), newData[0], newData[1]);
                    break;

                case "INSERT":
                    Console.WriteLine("Enter Data in Form:\nitem num, item name, item description");
                    Console.Write("\n =>");
                    string[] insertData = Console.ReadLine().Split(',');
                    try
                    {
                        int itemNum = int.Parse(insertData[0]); // The item num.
                        string itemName = insertData[1];           // The name string.
                        string itemDesc = insertData[2];          // The item description string.
                        AddProduct(itemNum, itemName, itemDesc);      // Add the data to the SQL database.
                    }
                    catch
                    {
                        Console.WriteLine("Could not insert data into DB.\n" +
                            "Please verify data is in correct format and try again.\n");
                    }
                    break;

                case "EXIT":
                    Environment.Exit(0);
                    break;

                case "FILL":
                    FillDB();
                    break;
                default:
                    Console.WriteLine("Input not recognized as valid command. Please enter a different command.\n");
                    break;
            }
        }

        /// <summary>
        /// Insert Product into database
        /// </summary>
        /// <param name="ItemNum">The Item number for the product.</param>
        /// <param name="ItemName">The Product Name</param>
        /// <param name="ItemDesc">The Item Description</param>
        static void AddProduct(int ItemNum, string ItemName, string ItemDesc)
        {
            using (SqlConnection conn = new SqlConnection(
        SampleCSharpProject.Properties.Settings.Default.SampleProductsConnectionString))
            {
                conn.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                        "INSERT INTO SampleProducts1 VALUES(@ItemNum, @ItemName, @ItemDesc)", conn))
                    {
                        command.Parameters.Add(new SqlParameter("ItemNum", ItemNum));
                        command.Parameters.Add(new SqlParameter("ItemName", ItemName));
                        command.Parameters.Add(new SqlParameter("ItemDesc", ItemDesc));
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Could not insert product into DB.");
                }
            }
        }

        /// <summary>
        /// Update Product in database
        /// </summary>
        /// <param name="ItemNum">The Item number for the product.</param>
        /// <param name="ItemName">The Product Name</param>
        /// <param name="ItemDesc">The Item Description</param>
        static void UpdateProduct(int ItemNum, string newName, string newDesc)
        {
            using (SqlConnection conn = new SqlConnection(
        SampleCSharpProject.Properties.Settings.Default.SampleProductsConnectionString))
            {
                conn.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                        "UPDATE SampleProducts1 SET ItemName=@ItemName, ItemDesc=@ItemDesc WHERE ItemNum=@ItemNum", conn))
                    {
                        command.Parameters.Add(new SqlParameter("ItemNum", ItemNum));
                        command.Parameters.Add(new SqlParameter("ItemName", newName));
                        command.Parameters.Add(new SqlParameter("ItemDesc", newDesc));
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Could not update product into DB.");
                }
            }
        }

        /// <summary>
        /// Remove Product from database
        /// </summary>
        /// <param name="Num">The Item number for the product.</param>
        static void RemoveProduct(int Num)
        {
            using (SqlConnection conn = new SqlConnection(
        SampleCSharpProject.Properties.Settings.Default.SampleProductsConnectionString))
            {
                conn.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                        "DELETE SampleProducts1 WHERE ItemNum=@ItemNum", conn))
                    {
                        command.Parameters.Add(new SqlParameter("@ItemNum", Num));
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Could not Delete product from DB.");
                }
            }
        }

        ///<summary>
        ///Build up database of products
        ///</summary>
        static void FillDB()
        {
            List<Product> products = new List<Product>();
            products.Add(new Product(10, "Filet Mignon", "Super tender steak"));
            products.Add(new Product(11, "PorterHouse", "2 in 1 steak"));
            products.Add(new Product(12, "Top Sirloin", "Good quality Steak"));
            products.Add(new Product(13, "Twice Baked Potatoes", "Potatoes covered with cheese and baked twice"));
            products.Add(new Product(14, "Lobster Tails", "North Atlantic Lobster Tails"));

            foreach (Product iter in products)
            {
                AddProduct(iter.m_ItemNum, iter.m_ItemName, iter.m_ItemDesc);
            }
        }

        /// <summary>
        /// Read in all rows from SampleProducts1 table and store in a list.
        /// </summary>
        static void DisplayProducts()
        {
            List<Product> products = new List<Product>();
            using (SqlConnection conn = new SqlConnection(
        SampleCSharpProject.Properties.Settings.Default.SampleProductsConnectionString))
            {
                conn.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM SampleProducts1", conn))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int ItemNum = reader.GetInt32(0);    // ItemNum int
                        string ItemName = reader.GetString(1);  // ItemName string
                        string ItemDesc = reader.GetString(2); // ItemDesc string
                        products.Add(new Product(ItemNum, ItemName, ItemDesc));
                    }
                }
            }
            products.Sort((x, y) => x.m_ItemNum.CompareTo(y.m_ItemNum));
            foreach (Product iter in products)
            {
                Console.WriteLine(iter);
            }
            Console.WriteLine("\n");
        }
    }

    class Product
    {
        public Product(int num, string name, string desc)
        {
            m_ItemNum = num;
            m_ItemName = name;
            m_ItemDesc = desc;
        }
        public int m_ItemNum { get; set; }
        public string m_ItemName { get; set; }
        public string m_ItemDesc { get; set; }
        public override string ToString()
        {
            return string.Format("Item Number: {0}, Item Name: {1}, Item Description: {2}",
                m_ItemNum, m_ItemName, m_ItemDesc);
        }
    }
}
