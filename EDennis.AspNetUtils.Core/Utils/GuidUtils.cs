namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Used to produce a testing SysGuid from an integer.  This kind of Guid is much easier to
    /// visually inspect for accuracy than a random guid.
    /// </summary>
    public static class GuidUtils
    {
        public static Guid FromId(int id)
        {
            return Guid.Parse($"00000000{Math.Abs(id)}"[^8..] + "-0000-0000-0000-" + $"000000000000{Math.Abs(id)}"[^12..]);
        }

    }
}
