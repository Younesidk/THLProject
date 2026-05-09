namespace THL_Project.Models;

public class RecognitionResult
{
    public bool Accepted { get; set; }  
    public string? RejectionReason { get; set; }
    public List<string> StepLog { get; set; } = new();
}
