using System.ComponentModel.DataAnnotations;

public class ItemInput
{
    [Required]
    public string title { get; set; }
    [Required]
    public string description{get; set;}
    public float startBid { get; set; }




}