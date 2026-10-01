namespace TexasMediaDart.Identity.Application.Abstractions.Email;

public interface IEmailVerificationLinkBuilder
{
    string Build(string token);
}