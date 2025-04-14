using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class MediaService
    {
        public readonly MediaProvider _mediaProvider;
        public MediaService()
        {
            _mediaProvider = new MediaProvider();
        }

        public bool CreateMultiMedia(List<Media> records)
        {
            return _mediaProvider.CreateMultiMedia(records);
        }

        public bool UpdateMultiMedia(IEnumerable<Media> records)
        {
            return _mediaProvider.UpdateMultiMedia(records);
        }

        public bool RemoveMultiMedia(List<Media> records)
        {
            return _mediaProvider.RemoveMultiMedia(records);
        }

        public List<Media> GetMediaByQuestionId(int questionId)
        {
            return _mediaProvider.GetMediaByQuestionId(questionId);
        }

        public Media GetMediaById(int id)
        {
            return _mediaProvider.GetMediaById(id);
        }
    }
}
