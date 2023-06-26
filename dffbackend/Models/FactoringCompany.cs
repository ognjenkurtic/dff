using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dffbackend.Models;

[Table("FactoringCompanies")]
public class FactoringCompany
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string ApiKey { get; set; }

    public List<Signature> Signatures { get; set; }

    public bool? IsDeleted { get; set; }

    public FactoringCompany()
    {
        Signatures = new List<Signature>();
    }
}