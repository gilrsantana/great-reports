namespace GreatReports.Shared;

public record ValidationError(Error[] Errors) 
    : Error("Validation.Error", "Ocorreram um ou mais erros de validação.")
{
    public static ValidationError FromErrors(Error[] errors)
    {
        return new ValidationError(errors);
    }
}
