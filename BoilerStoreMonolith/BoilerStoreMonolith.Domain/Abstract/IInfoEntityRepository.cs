using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;

namespace BoilerStoreMonolith.Domain.Abstract
{
    public interface IInfoEntityRepository
    {
        IEnumerable<InfoEntity> InfoEntities { get; }
        void SaveInfoEntity(InfoEntity infoEntity);
    }
}
