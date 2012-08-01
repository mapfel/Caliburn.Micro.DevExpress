using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace WPFSample1.Model
{
  public class Product
  {
    public int ID { get; set; }

    public string Name { get; set; }

    public static BindableCollection<Product> GetProductList()
    {
      BindableCollection<Product> list = new BindableCollection<Product>();
      list.Add(new Product() { ID = 1, Name = "Coffee" });
      list.Add(new Product() { ID = 2, Name = "Tea" });
      list.Add(new Product() { ID = 3, Name = "Sugar" });
      return list;
    }
  }
}