using System.Text;
using PelmeniCompilers.Models;

namespace PelmeniCompilers.ExtensionsMethods;

public static class StringBuilderExtension
{
    public static Token GetToken(this StringBuilder stringBuilder)
    {
        var asd = stringBuilder.ToString();
        return new Token();
    }
}