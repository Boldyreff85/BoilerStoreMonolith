using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerStoreMonolith.Domain.Concrete
{


    public class InfoEntityRepository : IInfoEntityRepository
    {
        private ApplicationContext context = new ApplicationContext();

        public IEnumerable<InfoEntity> InfoEntities
        {
            get { return context.InfoEntities; }
        }

        public void SaveInfoEntity(InfoEntity infoEntity)
        {
            context.InfoEntities.RemoveRange(context.InfoEntities);
            context.SaveChanges();
            context.InfoEntities.Add(infoEntity);
            context.SaveChanges();
        }


    }
}
