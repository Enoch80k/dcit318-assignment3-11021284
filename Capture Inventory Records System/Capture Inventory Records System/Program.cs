using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

// b. Marker Interface for Logging
public interface IInventoryEntity
{
    int Id { get; }
}

// a. Immutable Inventory Record
public record InventoryItem : IInventoryEntity
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int Quantity { get; init; }
    public DateTime DateAdded { get; init; }

    public InventoryItem(int id, string name, int quantity, DateTime dateAdded) =>
        (Id, Name, Quantity, DateAdded) = (id, name, quantity, dateAdded);
}

// c. Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

   
    public void Add(T item)
    {
        _log.Add(item);
    }

    
    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    
    public void SaveToFile()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
               
                Converters = { new JsonStringEnumConverter() }
            };

            using FileStream fs = new(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            JsonSerializer.Serialize(fs, _log, options);
        }
        catch (UnauthorizedAccessException uex)
        {
            Console.WriteLine($"Access denied when writing to file '{_filePath}': {uex.Message}");
        }
        catch (IOException ioex)
        {
            Console.WriteLine($"I/O error when writing to file '{_filePath}': {ioex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error when saving to file '{_filePath}': {ex.Message}");
        }
    }

    
    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"File '{_filePath}' does not exist. No data loaded.");
                return;
            }

            using FileStream fs = new(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var items = JsonSerializer.Deserialize<List<T>>(fs);

            if (items != null)
            {
                _log.Clear();
                _log.AddRange(items);
                Console.WriteLine($"Loaded {_log.Count} items from '{_filePath}'.");
            }
            else
            {
                Console.WriteLine($"No items found in file '{_filePath}'.");
            }
        }
        catch (JsonException jex)
        {
            Console.WriteLine($"Data format error when reading file '{_filePath}': {jex.Message}");
        }
        catch (UnauthorizedAccessException uex)
        {
            Console.WriteLine($"Access denied when reading file '{_filePath}': {uex.Message}");
        }
        catch (IOException ioex)
        {
            Console.WriteLine($"I/O error when reading file '{_filePath}': {ioex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error when loading from file '{_filePath}': {ex.Message}");
        }
    }
}


class Program
{
    static void Main()
    {
        string filePath = "inventory_log.json";

        var logger = new InventoryLogger<InventoryItem>(filePath);

        
        logger.LoadFromFile();

        
        logger.Add(new InventoryItem(id: 1, name: "Laptop", quantity: 10, dateAdded: DateTime.Now));
        logger.Add(new InventoryItem(id: 2, name: "Mouse", quantity: 50, dateAdded: DateTime.Now));
        logger.Add(new InventoryItem(id: 3, name: "Keyboard", quantity: 25, dateAdded: DateTime.Now));

        Console.WriteLine("Current inventory items:");
        foreach (var item in logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Added: {item.DateAdded}");
        }

        
        logger.SaveToFile();

        Console.WriteLine("Inventory saved to file.");
    }
}
