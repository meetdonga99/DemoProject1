using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class SubjectService
    {
        public readonly SubjectProvider _subjectProvider;
        public SubjectService()
        {
            _subjectProvider = new SubjectProvider();
        }

        public List<Subject> GetAllSubjects()
        {
            return _subjectProvider.GetAllSubjects();
        }
        public IQueryable<SubjectGridModel> GetAllSubjectsGrid()
        {
            return _subjectProvider.GetAllSubjectsGrid();
        }

        public int CreateSubject(Subject subject)
        {
            return _subjectProvider.CreateSubject(subject);
        }

        public int UpdateSubject(Subject subject)
        {
            return _subjectProvider.UpdateSubject(subject);
        }

        public Subject GetSubjectById(int id)
        {
            return _subjectProvider.GetSubjectById(id);
        }

        public Subject GetSubjectByName(string subjectName)
        {
            return _subjectProvider.GetSubjectByName(subjectName);
        }
        public List<Subject> CheckDuplicateSubjectCode(string SubjectCode)
        {
            return _subjectProvider.CheckDuplicateSubjectCode(SubjectCode);
        }
    }
}
