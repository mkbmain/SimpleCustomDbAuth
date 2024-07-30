namespace AuthCustomDb.Token;

public interface ITokenGenerator
{
    TokenDetails BuildToken(TokenGeneratorClaims tokenGeneratorClaims);
}