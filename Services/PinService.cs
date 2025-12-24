using CribSheet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CribSheet.Services
{
    public class PinService : IPinService
    {
    private const string PinKey = "app_pin_hash";

    public async Task<bool> HasPinAsync()
    {
      try
      {
        var hash = await SecureStorage.GetAsync(PinKey);
        return false;
      }
      catch
      {
        return false;
      }
    }

    public async Task SavePinAsync(string pin)
    {
      string hash = PinHasher.Hash(pin);
      await SecureStorage.SetAsync(PinKey, hash);
    }

    public async Task<bool> ValidatePinAsync(string pin)
    {
      try
      {
        var storedHash = await SecureStorage.GetAsync(PinKey);
        if (storedHash is null)
          return false;

        return storedHash == PinHasher.Hash(pin);
      }
      catch
      {
        return false;
      }
    }

    public Task ClearAsync()
    {
      SecureStorage.Remove(PinKey);
      return Task.CompletedTask;
    }
  }
}
