[Flags]
public enum Permissions
{
    None = 0,
    ManageProducts   = 1 << 0,
    ManageCategories = 1 << 1,
    ManageUsers      = 1 << 2,
    EditPermissions  = 1 << 3,
    All = ManageProducts | ManageCategories | ManageUsers | EditPermissions,
}

public enum Role
{
  Customer,
  Admin,
}
