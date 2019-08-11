using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class FirmRepository : IFirmRepository
    {
        private ApplicationContext context = new ApplicationContext();

        public IEnumerable<Firm> Firms => context.Firms;

        public void SaveFirm(Firm firm)
        {
            if (firm.Id == 0)
            {
                context.Firms.Add(firm);
            }
            else
            {
                Firm dbEntry = context.Firms.Find(firm.Id);
                if (dbEntry != null)
                {
                    dbEntry.Id = firm.Id;
                    dbEntry.Name = firm.Name;
                    dbEntry.ImageData = firm.ImageData;
                    dbEntry.ImageMimeType = firm.ImageMimeType;

                }
            }
            context.SaveChanges();
        }

        public Firm DeleteFirm(int firmId)
        {
            Firm dbEntry = context.Firms.Find(firmId);
            if (dbEntry != null)
            {
                context.Firms.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public List<Firm> DeleteFirms(List<Firm> firmsToDelete)
        {
            if (firmsToDelete != null)
            {
                context.Firms.RemoveRange(firmsToDelete);
                context.SaveChanges();
            }
            return firmsToDelete;
        }
    }
}
