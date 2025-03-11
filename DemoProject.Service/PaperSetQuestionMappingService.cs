using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class PaperSetQuestionMappingService
    {
        public readonly PaperSetQuestionMappingProvider _paperSetQuestionMappingProvider;
        public PaperSetQuestionMappingService()
        {
            _paperSetQuestionMappingProvider = new PaperSetQuestionMappingProvider();
        }

        public int AddQuestionInPaper(PaperSetQuestionMapping rec)
        {
            return _paperSetQuestionMappingProvider.AddQuestionInPaper(rec);
        }

        public int UpdateMapping(PaperSetQuestionMapping mapping)
        {
            return _paperSetQuestionMappingProvider.UpdateMapping(mapping);
        }

        public bool RemoveQuestionFromPaper(int recId)
        {
            return _paperSetQuestionMappingProvider.RemoveQuestionFromPaper(recId);
        }

        public bool RemoveQuestionsFromPaper(List<PaperSetQuestionMapping> mappings)
        {
            return _paperSetQuestionMappingProvider.RemoveQuestionsFromPaper(mappings);
        }

        public List<PaperSetQuestionMapping> GetMappingsByPaperSetId(int paperSetId)
        {
            return _paperSetQuestionMappingProvider.GetMappingsByPaperSetId(paperSetId);
        }
    }
}
