
public class SalesHistory
{
    public static Dictionary<Guid, List<(Guid ProductId, int Quantity, DateTime Date)>> SalesData;

    public static void AddSale(Guid userId, Guid productId, int quantity)
    {
        if (!SalesData.ContainsKey(userId))
        {
            SalesData[userId] = new List<(Guid, int, DateTime)>();
        }
        SalesData[userId].Add((productId, quantity, DateTime.UtcNow));
    }

    public static void RefundSale(Guid userId, Guid productId, int quantity)
    {
        if (SalesData.ContainsKey(userId))
        {
            var sales = SalesData[userId];
            var sale = sales.FirstOrDefault(s => s.ProductId == productId && s.Quantity >= quantity);
            if (sale != default)
            {
                sales.Remove(sale);
                if (sale.Quantity > quantity)
                {
                    sales.Add((productId, sale.Quantity - quantity, sale.Date));
                }
            }
        }
    }

    public static Dictionary<Guid, int> GetTotalIndividualItemPurchases(Guid productID)
    {
        var totalPurchases = new Dictionary<Guid, int>();
        foreach (var userSales in SalesData)
        {
            var sales = userSales.Value;
            var totalQuantity = sales.Where(s => s.ProductId == productID).Sum(s => s.Quantity);
            if (totalQuantity > 0)
            {
                totalPurchases[productID] = totalQuantity;
            }
        }
        return totalPurchases;
    }

    public static Dictionary<Guid, int> GetTotalPurchasesByUser(Guid userId)
    {
        var totalPurchases = new Dictionary<Guid, int>();
        if (SalesData.ContainsKey(userId))
        {
            var sales = SalesData[userId];
            foreach (var sale in sales)
            {
                if (!totalPurchases.ContainsKey(sale.ProductId))
                {
                    totalPurchases[sale.ProductId] = 0;
                }
                totalPurchases[sale.ProductId] += sale.Quantity;
            }
        }
        return totalPurchases;
    }
}
