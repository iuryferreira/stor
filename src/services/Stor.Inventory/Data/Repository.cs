using Stor.Inventory.Data.Models;

namespace Stor.Inventory.Data;

public class Repository
{
    private readonly List<Product> _products = new();

    public Repository()
    {
        _products.Add(new Product {Name = "Mouse", CreatedAt = DateTime.Now, Price = 323.20m});
    }

    public IReadOnlyCollection<Product> All() => _products;
}