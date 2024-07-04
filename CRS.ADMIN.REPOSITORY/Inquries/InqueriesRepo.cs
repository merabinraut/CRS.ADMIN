using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.Inquries
{
    public class InqueriesRepo: IInqueriesRepo
    {
        RepositoryDao _DAO;
        public InqueriesRepo()
        {
            _DAO = new RepositoryDao();
        }
    }
}
