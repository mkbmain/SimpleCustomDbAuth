namespace AuthCustomDb.Token;

public record TokenGeneratorClaims(string Email, Guid UserId, string[] Roles);