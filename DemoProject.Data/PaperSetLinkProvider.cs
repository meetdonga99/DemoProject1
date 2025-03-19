using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class PaperSetLinkProvider : BaseProvider
    {
        public PaperSetLinkProvider()
        {

        }
        public int CreatePaperSetLink(PaperSetLink paperSetLink)
        {
            try
            {
                _db.PaperSetLink.Add(paperSetLink);
                _db.SaveChanges();
                return paperSetLink.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdatePaperSetLink(PaperSetLink paperSetLink)
        {
            try
            {
                _db.Entry(paperSetLink).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return paperSetLink.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public PaperSetLink GetPaperSetLinkByPaperSetId(int paperSetId)
        {
            var link = (from c in _db.PaperSetLink where c.PaperSetId == paperSetId && c.IsActive == true select c).FirstOrDefault();
            return link;
        }

        public PaperSetLink GetPaperSetLinkByToken(string token)
        {
            var link = (from c in _db.PaperSetLink where c.Token == token && c.IsActive == true select c).FirstOrDefault();
            return link;
        }
    }
}
