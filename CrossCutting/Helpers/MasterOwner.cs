using CrossCutting.Constants;

namespace CrossCutting.Helpers;

public static class MasterOwner
{
    public static bool Validate(string ownerId) =>
       string.IsNullOrEmpty(ownerId) || ownerId != AppConstants.AppName.ToLower();
}
