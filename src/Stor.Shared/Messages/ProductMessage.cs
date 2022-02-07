namespace Stor.Shared.Messages;

public static class ProductMessage
{
    public record Product(string Id, string Name, decimal Price);

    public record GetAllProducts();
    public record AllProducts(List<Product> Products);
    public record AddProduct(string Name, decimal Price);
    public record AddedProduct(string Id, string Name, decimal Price, DateTime CreatedAt);
}