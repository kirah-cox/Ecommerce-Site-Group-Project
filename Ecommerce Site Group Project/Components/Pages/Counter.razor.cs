namespace Ecommerce_Site_Group_Project.Components.Pages;

public partial class Counter
{
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
    int role = PageHiding.role;
    public static int Admin = 1;
    public static int Employee = 2;
    public static int Customer = 3;
}