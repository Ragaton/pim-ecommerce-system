namespace PIM.Services;

// This class represents a single changelog entry.
// It is used both for writing JSON files and for binding to the changelog UI
public class ChangeLogEntry
{
    public string Timestamp     { get; set; } = "";
    public string OperationType { get; set; } = "";
    public string Before        { get; set; } = "";
    public string After         { get; set; } = "";
}