﻿using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class FormsService
    {

        public readonly FormsProvider _formsProvider;
        public FormsService()
        {
            _formsProvider = new FormsProvider();
        }

        public int CreateForms(Forms forms)
        {
            return _formsProvider.CreateForms(forms);
        }


        public int UpdateForms(Forms forms)
        {
            return _formsProvider.UpdateForms(forms);
        }


        public Forms GetFormsById(int id)
        {
            return _formsProvider.GetFormsById(id);
        }
        public Forms GetFormsByCode(string formcode)
        {
            return _formsProvider.GetFormsByCode(formcode);
        }
        public List<Forms> GetAllForms()
        {
            return _formsProvider.GetAllForms();
        }
        public IQueryable<FormsGridModel> GetAllFormsGrid()
        {
            return _formsProvider.GetAllFormsGrid();
        }
        public List<Forms> CheckDuplicateFormAccessCode(string formAccessCode)
        {
            return _formsProvider.CheckDuplicateFormAccessCode(formAccessCode);
        }
    }
}
