using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GameZone.Repository.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is null)
                return result;

            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry is null
                    || entry.State != EntityState.Detached
                    || !(entry.Entity is ISoftDeleteable entity))
                    continue;
                entry.State = EntityState.Modified;
                entity.Delete();
            }
            return result;
        }
    }
}
