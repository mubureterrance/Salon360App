using Salon360App.Models;

namespace Salon360App.Services
{
    public static class EntryHelper
    {
        public static void SetCreatedAudit(BaseEntry entry, int userId)
        {
            entry.CreatedById = userId;
            entry.CreatedAt = DateTime.UtcNow;
            entry.IsActive = true;
        }

        public static void SetUpdatedAudit(BaseEntry entry, int userId)
        {
            entry.UpdatedById = userId;
            entry.UpdatedAt = DateTime.UtcNow;
        }

        public static void SetDeletedAudit(BaseEntry entry, int userId)
        {
            entry.IsDeleted = true;
            entry.DeletedAt = DateTime.UtcNow;
            entry.UpdatedById = userId;
            entry.UpdatedAt = DateTime.UtcNow;
        }

    }
}
