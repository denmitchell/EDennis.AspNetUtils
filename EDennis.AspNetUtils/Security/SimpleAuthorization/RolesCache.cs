using System.Collections.Concurrent;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// A temporary cache that holds the role of each user by user name (cache key).  This
    /// cache should have a singleton lifetime
    /// </summary>
    public class RolesCache: ConcurrentDictionary<string, (DateTime ExpiresAt, string Role)>
    {
    }
}
