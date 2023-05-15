namespace dffbackend.BusinessLogic.FactoringCompanies.DTOs;

public class FactoringCompanyDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string ApiKey { get; set; }

    public bool IsDeleted { get; set; }
}