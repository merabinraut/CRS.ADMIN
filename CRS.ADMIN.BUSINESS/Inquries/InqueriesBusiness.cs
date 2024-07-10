using CRS.ADMIN.REPOSITORY.ClubManagement;
using CRS.ADMIN.REPOSITORY.Inquries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.Inquries
{
    public class InqueriesBusiness: IInqueriesBusiness
    {
        IInqueriesRepo _REPO;
        public InqueriesBusiness(InqueriesRepo REPO)
        {
            _REPO = REPO;
        }

    }
}
