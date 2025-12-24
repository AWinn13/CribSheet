using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CribSheet.Utilities
{
  public class PinHasher
  {
    public static string Hash(string pin)
    {
      using var sha = SHA256.Create();
      var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(pin));
      return Convert.ToBase64String(bytes);
    }
  }
}
