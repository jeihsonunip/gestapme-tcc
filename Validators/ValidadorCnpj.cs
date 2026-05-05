using System.ComponentModel.DataAnnotations;

namespace GestaPME.Validators;

public class ValidadorCnpj : ValidationAttribute{
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx){
        var cnpj = (value as string ?? string.Empty).Replace(".", "").Replace("/", "").Replace("-", "").Trim();
        if (cnpj.Length != 14 || !cnpj.All(char.IsDigit))
            return new ValidationResult("CNPJ inválido.");
        if (cnpj.Distinct().Count() == 1)
            return new ValidationResult("CNPJ inválido.");

        var pesos1 = new[]{ 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var pesos2 = new[]{ 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int soma = 0;
        for (int i = 0; i < 12; i++) soma += (cnpj[i] - '0') * pesos1[i];
        int d1 = soma % 11 < 2 ? 0 : 11 - soma % 11;

        soma = 0;
        for (int i = 0; i < 13; i++) soma += (cnpj[i] - '0') * pesos2[i];
        int d2 = soma % 11 < 2 ? 0 : 11 - soma % 11;

        if (d1 != cnpj[12] - '0' || d2 != cnpj[13] - '0')
            return new ValidationResult("CNPJ inválido.");

        return ValidationResult.Success;
    }
}