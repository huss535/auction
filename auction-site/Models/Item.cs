using System.ComponentModel.DataAnnotations;

public class Item
{

    [Key]
    public int Id { get; set; }
    public string Owner { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public float StartBid { get; set; }
    public float CurrentBid { get; set; }
    public string State { get; set; }
}