using System.ComponentModel.DataAnnotations;

namespace dffbackend.Models;

public class Signature
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid FactoringCompanyId { get; set; }

    [Required]
    public FactoringCompany FactoringCompany { get; set; }

    public string Signature1 { get; set; }
    
    public string Signature2 { get; set; }

    public string Signature3 { get; set; }

    public string Signature4 { get; set; }

    public string Signature5 { get; set; }

    public DateTime CreationDate { get; set; }
}