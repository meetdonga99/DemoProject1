using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class PaperSetLinkService
    {
        public readonly PaperSetLinkProvider _paperSetLinkProvider;
        public PaperSetLinkService()
        {
            _paperSetLinkProvider = new PaperSetLinkProvider();
        }

        public int CreatePaperSetLink(PaperSetLink paperSetLink)
        {
            return _paperSetLinkProvider.CreatePaperSetLink(paperSetLink);
        }

        public int UpdatePaperSetLink(PaperSetLink paperSetLink)
        {
            return _paperSetLinkProvider.UpdatePaperSetLink(paperSetLink);
        }

        public PaperSetLink GetPaperSetLinkByPaperSetId(int paperSetId)
        {
            return _paperSetLinkProvider.GetPaperSetLinkByPaperSetId(paperSetId);
        }

        public PaperSetLink GetPaperSetLinkByToken(string token)
        {
            return _paperSetLinkProvider.GetPaperSetLinkByToken(token);
        }
    }
}
