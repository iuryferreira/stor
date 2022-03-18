using Stor.Communication.Messages;

namespace Stor.Shop.Data;

internal interface IRepository
{
    public IReadOnlyCollection<ProductMessage.Product> All();
    void SetProductList(IEnumerable<ProductMessage.Product> products);
}

internal class Repository : IRepository
{
    private readonly List<ProductMessage.Product> _products = new();

    public void SetProductList(IEnumerable<ProductMessage.Product> products)
    {
        _products.Clear();
        _products.AddRange(products);
    }

    public IReadOnlyCollection<ProductMessage.Product> All() => _products;
}