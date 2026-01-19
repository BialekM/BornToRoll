using System.Collections.Concurrent;

namespace BornToRollWebApi.Services.RateLimit
{
    public class RateLimitService
    {
        private readonly ConcurrentDictionary<string, List<DateTime>> _attempts = new();
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(15);
        private readonly int _maxAttempts = 3;

        public bool IsRateLimited(string key)
        {
            var now = DateTime.UtcNow;
            
            _attempts.AddOrUpdate(key, 
                _ => new List<DateTime> { now },
                (_, attempts) =>
                {
                    // Remove old attempts
                    attempts.RemoveAll(a => now - a > _timeWindow);
                    attempts.Add(now);
                    return attempts;
                });

            var recentAttempts = _attempts[key].Count(a => now - a <= _timeWindow);
            return recentAttempts > _maxAttempts;
        }

        public void CleanupOldEntries()
        {
            var now = DateTime.UtcNow;
            var keysToRemove = _attempts
                .Where(kvp => kvp.Value.All(a => now - a > _timeWindow))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _attempts.TryRemove(key, out _);
            }
        }
    }
}
