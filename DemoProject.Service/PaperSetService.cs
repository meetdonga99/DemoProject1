using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class PaperSetService
    {
        public readonly PaperSetProvider _paperSetProvider;
        public PaperSetService()
        {
            _paperSetProvider = new PaperSetProvider();
        }

        public List<PaperSet> GetAllPaperSets()
        {
            return _paperSetProvider.GetAllPaperSets();
        }

        public IQueryable<PaperSetGridModel> GetAllPaperSetsGrid()
        {
            return _paperSetProvider.GetAllPaperSetsGrid();
        }

        public int CreatePaperSet(PaperSet paperSet)
        {
            return _paperSetProvider.CreatePaperSet(paperSet);
        }

        public int UpdatePaperSet(PaperSet paperSet)
        {
            return _paperSetProvider.UpdatePaperSet(paperSet);
        }

        public PaperSet GetPaperSetById(int id)
        {
            return _paperSetProvider.GetPaperSetById(id);
        }

        public bool DeletePaperSet(int paperSetId)
        {
            return _paperSetProvider.DeletePaperSet(paperSetId);
        }

        public void SaveChanges()
        {
           _paperSetProvider.SaveChanges();
        }
    }
}
