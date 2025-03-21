﻿using DemoProject.Data;
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

        public bool DeleteOption(int id)
        {
            return _optionProvider.DeleteOption(id);
        }

        public Option GetOptionById(int id)
        {
            return _optionProvider.GetOptionById(id);
        }

        public List<Option> GetOptionsByQuestionId(int id)
        {
            return _optionProvider.GetOptionsByQuestionId(id);
        }
    }
}
