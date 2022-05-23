namespace MikyM.Common.EfCore.ApplicationLayer;

/// <summary>
/// Tells the automatic registration process to skip this data service and allow manual registration.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[PublicAPI]
public class SkipDataServiceRegistrationAttribute : Attribute
{
}