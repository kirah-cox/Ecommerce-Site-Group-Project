
using System;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SalesHistoryEntry
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    
    public SalesHistoryEntry(Guid productId, int quantity, DateTime date)
    {
        ProductId = productId;
        Quantity = quantity;
        Date = date;
    }
}

public class SalesHistory
{
    const string dbPath = "salesHistory.json";

    public static Dictionary<Guid, List<SalesHistoryEntry>> SalesData { get; set; } = new();

    public static async Task InitializeSalesHistory()
    {
        await LoadSalesHistoryAsync();
    }

    private static async Task LoadSalesHistoryAsync()
    {
        if (!File.Exists(dbPath))
        {
            SalesData = new Dictionary<Guid, List<SalesHistoryEntry>>();
        }
        else
        {
            var json = await File.ReadAllTextAsync(dbPath);
            SalesData = JsonSerializer.Deserialize<Dictionary<Guid, List<SalesHistoryEntry>>>(json) ?? new Dictionary<Guid, List<SalesHistoryEntry>>();
        }
    }

    private static async Task SaveSalesHistoryAsync()
    {
        var json = JsonSerializer.Serialize(SalesData);
        await File.WriteAllTextAsync(dbPath, json);
    }

    public static async Task AddSale(Guid userId, Guid productId, int quantity)
    {
        if (!SalesData.ContainsKey(userId))
        {
            SalesData[userId] = new List<SalesHistoryEntry>();
        }
        SalesData[userId].Add(new SalesHistoryEntry(productId, quantity, DateTime.UtcNow));
        await SaveSalesHistoryAsync();
    }

    public static async Task RefundSale(Guid userId, Guid productId, int quantity)
    {
        if (SalesData.ContainsKey(userId))
        {
            var sales = SalesData[userId];
            var sale = sales.FirstOrDefault(s => s.ProductId == productId && s.Quantity >= quantity);
            if (sale != null)
            {
                sales.Remove(sale);
                if (sale.Quantity > quantity)
                {
                    sales.Add(new SalesHistoryEntry(productId, sale.Quantity - quantity, sale.Date));
                }
            }
        }

        await SaveSalesHistoryAsync();
    }

    public static int GetTotalPurchasesForIndividualProduct(Guid productID)
    {
        int totalPurchases = 0;
        foreach (var userSales in SalesData)
        {
            var sales = userSales.Value;
            var totalQuantity = sales.Where(s => s.ProductId == productID).Sum(s => s.Quantity);
            if (totalQuantity > 0)
            {
                totalPurchases += totalQuantity;
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