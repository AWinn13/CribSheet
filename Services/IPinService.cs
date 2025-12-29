using System;
using System.Collections.Generic;
using System.Text;

namespace CribSheet.Services
{
  public interface IPinService
  {
    Task<bool> HasPinAsync();
    Task SavePinAsync(string pin);
    Task<bool> ValidatePinAsync(string pin);
    Task ClearAsync();
    Task SetRemindUser(bool remind);
    Task<bool> ShouldRemindUserAsync();
  }
}
