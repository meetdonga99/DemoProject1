using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class OptionProvider : BaseProvider
    {
        public OptionProvider()
        {

        }

        public int CreateOption(Option option)
        {
            try
            {
                _db.Option.Add(option);
                _db.SaveChanges();
                return option.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateOption(Option option)
        {
            try
            {
                _db.Entry(option).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return option.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteOption(int id)
        {
            try
            {
                var option = _db.Option.Find(id);
                if (option == null)
                {
                    return false;
                }

                _db.Option.Remove(option);
                _db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Option GetOptionById(int id)
        {
            return _db.Option.Find(id);
        }

        public List<Option> GetOptionsByQuestionId(int id)
        {
            var data = (from a in _db.Option where a.QuestionId == id select a).ToList();
            return data;
        }
    }
}
