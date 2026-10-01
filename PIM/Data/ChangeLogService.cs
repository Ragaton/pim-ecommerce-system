using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PIM.Services;

// This class handles all writing to the changelog.
// Every operation writes two outputs:
//   1. A human-readable entry appended to changelog.txt — acts as a plain text audit trail
//   2. A JSON file per entry saved in the Log/entries/ folder — used by the changelog UI
public static class ChangeLogService
{
    // Path to the human-readable changelog file
    private static readonly string LogFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "PIM", "Log", "changelog.txt");

    // Path to the folder where individual JSON entry files are stored
    private static readonly string EntriesFolder = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "PIM", "Log", "Entries");

    // Writes a create entry to both the txt changelog and as a JSON file.
    // There is no "before" state for a create, so before is left empty.
    public static void LogCreate(string productName, int categoryId, string sku, decimal price, decimal basePrice, decimal costPrice)
    {
        string timestamp = GetTimestamp();

        string after =
            $"Product Name: {productName}\n" +
            $"Category ID: {categoryId}\n" +
            $"SKU: {sku}\n" +
            $"Price: {price}\n" +
            $"Base Price: {basePrice}\n" +
            $"Cost Price: {costPrice}";

        string txtEntry =
            $"[{timestamp}] OPERATION: CREATE\n" +
            $"  AFTER:\n" +
            $"    Product Name : {productName}\n" +
            $"    Category ID  : {categoryId}\n" +
            $"    SKU          : {sku}\n" +
            $"    Price        : {price}\n" +
            $"    Base Price   : {basePrice}\n" +
            $"    Cost Price   : {costPrice}\n" +
            $"  -----------------------------------------------\n";

        WriteToTxt(txtEntry);
        WriteToJson(new ChangeLogEntry
        {
            Timestamp     = timestamp,
            OperationType = "CREATE",
            Before        = "-",
            After         = after
        });
    }

    // Writes an update entry to both the txt changelog and as a JSON file.
    public static void LogUpdate(
        int variantId,
        int productId,
        string beforeProductName, string afterProductName,
        string beforeSku, string afterSku,
        decimal beforePrice, decimal afterPrice,
        decimal beforeBasePrice, decimal afterBasePrice,
        decimal beforeCostPrice, decimal afterCostPrice)
    {
        string timestamp = GetTimestamp();

        string before =
            $"Product Name: {beforeProductName}\n" +
            $"SKU: {beforeSku}\n" +
            $"Price: {beforePrice}\n" +
            $"Base Price: {beforeBasePrice}\n" +
            $"Cost Price: {beforeCostPrice}";

        string after =
            $"Product Name: {afterProductName}\n" +
            $"SKU: {afterSku}\n" +
            $"Price: {afterPrice}\n" +
            $"Base Price: {afterBasePrice}\n" +
            $"Cost Price: {afterCostPrice}";

        string txtEntry =
            $"[{timestamp}] OPERATION: UPDATE\n" +
            $"  Product ID   : {productId}\n" +
            $"  Variant ID   : {variantId}\n" +
            $"  BEFORE:\n" +
            $"    Product Name : {beforeProductName}\n" +
            $"    SKU          : {beforeSku}\n" +
            $"    Price        : {beforePrice}\n" +
            $"    Base Price   : {beforeBasePrice}\n" +
            $"    Cost Price   : {beforeCostPrice}\n" +
            $"  AFTER:\n" +
            $"    Product Name : {afterProductName}\n" +
            $"    SKU          : {afterSku}\n" +
            $"    Price        : {afterPrice}\n" +
            $"    Base Price   : {afterBasePrice}\n" +
            $"    Cost Price   : {afterCostPrice}\n" +
            $"  -----------------------------------------------\n";

        WriteToTxt(txtEntry);
        WriteToJson(new ChangeLogEntry
        {
            Timestamp     = timestamp,
            OperationType = "UPDATE",
            Before        = before,
            After         = after
        });
    }

    // Writes a delete entry to both the txt changelog and as a JSON file.
    public static void LogDelete(int variantId, int productId, string sku)
    {
        string timestamp = GetTimestamp();

        string before =
            $"Product ID: {productId}\n" +
            $"Variant ID: {variantId}\n" +
            $"SKU: {sku}";

        string txtEntry =
            $"[{timestamp}] OPERATION: DELETE\n" +
            $"  BEFORE:\n" +
            $"    Product ID   : {productId}\n" +
            $"    Variant ID   : {variantId}\n" +
            $"    SKU          : {sku}\n" +
            $"  AFTER: Variant permanently deleted\n" +
            $"  -----------------------------------------------\n";

        WriteToTxt(txtEntry);
        WriteToJson(new ChangeLogEntry
        {
            Timestamp     = timestamp,
            OperationType = "DELETE",
            Before        = before,
            After         = "Variant permanently deleted"
        });
    }

    // Writes a status change entry to both the txt changelog and as a JSON file.
    public static void LogActiveStatusChange(int variantId, bool wasActive, bool isNowActive)
    {
        string timestamp = GetTimestamp();

        string txtEntry =
            $"[{timestamp}] OPERATION: STATUS CHANGE\n" +
            $"  Variant ID   : {variantId}\n" +
            $"  BEFORE: {(wasActive ? "Active" : "Inactive")}\n" +
            $"  AFTER : {(isNowActive ? "Active" : "Inactive")}\n" +
            $"  -----------------------------------------------\n";

        WriteToTxt(txtEntry);
        WriteToJson(new ChangeLogEntry
        {
            Timestamp     = timestamp,
            OperationType = "STATUS CHANGE",
            Before        = wasActive ? "Active" : "Inactive",
            After         = isNowActive ? "Active" : "Inactive"
        });
    }

    // Reads all JSON entry files from the entries folder and returns them
    // sorted newest first — used by the changelog UI to load entries.
    public static List<ChangeLogEntry> ReadAllEntries()
    {
        List<ChangeLogEntry> entries = new List<ChangeLogEntry>();

        string fullEntriesFolder = Path.GetFullPath(EntriesFolder);

        if (!Directory.Exists(fullEntriesFolder))
            return entries; // No entries folder yet, return empty list

        // Get all JSON files sorted by filename descending — filenames contain the timestamp so this gives newest first
        string[] files = Directory.GetFiles(fullEntriesFolder, "*.json");
        Array.Sort(files);
        Array.Reverse(files);

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            ChangeLogEntry? entry = JsonSerializer.Deserialize<ChangeLogEntry>(json);
            if (entry != null)
                entries.Add(entry);
        }

        return entries;
    }

    // Returns the current timestamp in DD:MM:YYYY:HH:MM:SS format as specified in the user story
    private static string GetTimestamp()
    {
        return DateTime.Now.ToString("dd:MM:yyyy:HH:mm:ss");
    }

    // Appends the human-readable entry to changelog.txt
    private static void WriteToTxt(string entry)
    {
        string fullPath = Path.GetFullPath(LogFilePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.AppendAllText(fullPath, entry);
    }

    // Writes a single JSON entry file to the entries folder.
    // The filename is based on a sortable timestamp so files naturally sort newest last.
    private static void WriteToJson(ChangeLogEntry entry)
    {
        string fullEntriesFolder = Path.GetFullPath(EntriesFolder);
        Directory.CreateDirectory(fullEntriesFolder);

        // Use a sortable filename: yyyyMMddHHmmss + a unique suffix to avoid collisions
        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N")[..8] + ".json";
        string filePath = Path.Combine(fullEntriesFolder, fileName);

        string json = JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}