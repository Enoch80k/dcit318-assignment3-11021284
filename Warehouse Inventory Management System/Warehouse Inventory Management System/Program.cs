using System;
using System.Collections.Generic;

// a. Marker Interface IInventoryItem
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// b. Product Classes

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"[Electronic] Id: {Id}, Name: {Name}, Quantity: {Quantity}, Brand: {Brand}, WarrantyMonths: {WarrantyMonths}";
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"[Grocery] Id: {Id}, Name: {Name}, Quantity: {Quantity}, ExpiryDate: {ExpiryDate:yyyy-MM-dd}";
    }
}

// d. Generic Inventory Repository

public class InventoryRepository<T> where T : IInventoryItem
{
    
    private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        }
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T item))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException("Quantity cannot be negative.");
        }

        var item = GetItemById(id); // Will throw if not found
        item.Quantity = newQuantity;
    }
}

// e. Custom Exceptions

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// f. WareHouseManager Class

public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private readonly InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        try
        {
            
            _electronics.AddItem(new ElectronicItem(1, "Smartphone", 50, "BrandA", 24));
            _electronics.AddItem(new ElectronicItem(2, "Laptop", 30, "BrandB", 36));
            _electronics.AddItem(new ElectronicItem(3, "Headphones", 150, "BrandC", 12));

            
            _groceries.AddItem(new GroceryItem(101, "Milk", 200, DateTime.Today.AddDays(7)));
            _groceries.AddItem(new GroceryItem(102, "Bread", 100, DateTime.Today.AddDays(3)));
            _groceries.AddItem(new GroceryItem(103, "Eggs", 300, DateTime.Today.AddDays(14)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SeedData Error]: {ex.Message}");
        }
    }

    
    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        var items = repo.GetAllItems();
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
        if (items.Count == 0)
        {
            Console.WriteLine("No items found.");
        }
    }

    
    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantityToAdd) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            if (quantityToAdd < 0)
                throw new InvalidQuantityException("Increase quantity must be positive.");

            repo.UpdateQuantity(id, item.Quantity + quantityToAdd);
            Console.WriteLine($"Increased stock for item ID {id} by {quantityToAdd}. New quantity: {item.Quantity}");
        }
        catch (DuplicateItemException diex)
        {
            Console.WriteLine($"[Error] Duplicate item: {diex.Message}");
        }
        catch (ItemNotFoundException infex)
        {
            Console.WriteLine($"[Error] Item not found: {infex.Message}");
        }
        catch (InvalidQuantityException iqex)
        {
            Console.WriteLine($"[Error] Invalid quantity: {iqex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] An unexpected error occurred: {ex.Message}");
        }
    }

   
    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed successfully.");
        }
        catch (ItemNotFoundException infex)
        {
            Console.WriteLine($"[Error] Item not found: {infex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] An unexpected error occurred: {ex.Message}");
        }
    }

    
    public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
    public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
}


class Program
{
    static void Main()
    {
        var manager = new WareHouseManager();

        Console.WriteLine("Seeding data...");
        manager.SeedData();

        Console.WriteLine("\n--- Grocery Items ---");
        manager.PrintAllItems(manager.GroceriesRepo);

        Console.WriteLine("\n--- Electronic Items ---");
        manager.PrintAllItems(manager.ElectronicsRepo);

        Console.WriteLine("\n--- Testing Exception Scenarios ---");

        // 1. Add a duplicate item
        try
        {
            Console.WriteLine("\nAdding duplicate item to electronics...");
            var duplicateElectronic = new ElectronicItem(1, "Tablet", 20, "BrandD", 18);
            manager.ElectronicsRepo.AddItem(duplicateElectronic);
        }
        catch (DuplicateItemException diex)
        {
            Console.WriteLine($"DuplicateItemException caught: {diex.Message}");
        }

        // 2. Remove a non-existent item
        try
        {
            Console.WriteLine("\nRemoving non-existent grocery item with ID 999...");
            manager.RemoveItemById(manager.GroceriesRepo, 999);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception caught: {ex.Message}");
        }

        // 3. Update with invalid quantity (negative)
        try
        {
            Console.WriteLine("\nUpdating grocery item quantity with invalid (negative) value...");
            manager.GroceriesRepo.UpdateQuantity(101, -10);
        }
        catch (InvalidQuantityException iqex)
        {
            Console.WriteLine($"InvalidQuantityException caught: {iqex.Message}");
        }

        Console.WriteLine("\nDone.");
    }
}
