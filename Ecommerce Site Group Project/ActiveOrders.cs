using System.Net.Http.Headers;

public class ActiveOrders
{
    public List<Product> Order { get; private set; } = new List<Product> { };
    public decimal PriceTotal { get; private set; }

    ActiveOrders()
    {
        PriceTotal = 0;
    }

    public void AddItemToOrder(Product item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Order.Add(item);
        }
    }

    public bool RemoveItem(Product item)
    {
        if (Order.Contains(item))
        {
            Order.Remove(item);
            return true;
        }
        else
            return false;
    }
    public void RemoveItems(Product item)
    {
        int i = 0;
        foreach (Product toRemove in Order)
        {
            if (toRemove == item)
            {
                i++;
            }
        }
        for (int j = 0; j < i; j++)
        {
            Order.Remove(item);
        }
    }
    public void ClearCart()
    {
        Order.Clear();
    }
    public decimal GetPriceTotal()
    {
        decimal sum = 0;
        foreach (Product item in Order)
        {
            sum += item.Price;
        }

        PriceTotal = sum;

        if (sum > 0)
        {
            sum += sum * 0.0485m; //sales tax for utah
        }

        return sum;
    }
}