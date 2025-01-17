public static class CurrencyHelper
{
    // Convert smallest currency unit to standart (e.g. cents to USD)
    public static decimal ToStandartUnit(decimal value)
    {
        return value / 100;
    }  
}