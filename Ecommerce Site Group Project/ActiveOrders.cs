using System.Net.Http.Headers;

public class ActiveOrders
{
    public List<Product> Order { get; private set;} = new List<Product> {};
    public decimal Price {get; private set;}

    ActiveOrders ()
    {
        Price = 0;
    }

    public void AddItem (Product item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Order.Add(item);
        }
    }

    public bool RemoveItem (Product item)
    {
        if (Order.Contains(item))
        {
            Order.Remove(item);
            return true;
        }
        else
            return false;
    }
    public void RemoveItems (Product item)
    {
        foreach(Product toRemove in Order)
        {
            if (toRemove == item)
            {
                Order.Remove(item);
            }
        }
    }
    public void ClearCart ()
    {
        Order.Clear();
    }
    public decimal GetPriceTotal ()
    {
        decimal sum = 0;
        foreach(Product item in Order)
        {
            sum += item.price;
        }

        Price = sum;

        if (sum > 0)
        {
            sum += sum * 0.0485m; //sales tax for utah
        }

        return sum;
    }
}