using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class OptionService
    {
        public readonly OptionProvider _optionProvider;

        public OptionService()
        {
            _optionProvider = new OptionProvider();
        }

        public int CreateOption(Option option)
        {
            return _optionProvider.CreateOption(option);
        }

        public int UpdateOption(Option option)
        {
            return _optionProvider.UpdateOption(option);
        }

        public Option GetOptionById(int id)
        {
            return _optionProvider.GetOptionById(id);
        }
    }
}
