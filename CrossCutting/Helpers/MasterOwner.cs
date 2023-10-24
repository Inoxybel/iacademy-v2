using CrossCutting.Constants;

namespace CrossCutting.Helpers;

public static class MasterOwner
{
    public static bool Validate(string ownerId) =>
       ownerId == AppConstants.AppName.ToLower();
}
