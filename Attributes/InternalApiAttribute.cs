using System;

namespace AirCargoRatesAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InternalApiAttribute : Attribute
    {
    }
}