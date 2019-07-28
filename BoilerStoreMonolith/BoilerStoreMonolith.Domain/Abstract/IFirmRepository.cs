﻿using System.Collections.Generic;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Abstract
{
    public interface IFirmRepository
    {
        IEnumerable<Firm> Firms { get; }
        void SaveFirm(Firm firm);
        Firm DeleteFirm(int firmId);
        Firm DeleteFirmByName(string firmName);
        List<Firm> DeleteFirms(List<Firm> firmsToDelete);
    }
}