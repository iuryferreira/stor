using Stor.Communication.Messages;

namespace Stor.Inventory.Data.Models;

public class Product
{
    public string Id { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
    public string Name { get; init; } = "";
    public decimal Price { get; init; }
    public DateTime? CreatedAt { get; init; }

    public static implicit operator ProductMessage.Product(Product product) =>
        new ProductMessage.Product(product.Id, product.Name, product.Price);
}