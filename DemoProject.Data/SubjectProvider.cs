using DemoProject.Model;
using DemoProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class SubjectProvider : BaseProvider
    {
        public SubjectProvider()
        {

        }
        public List<Subject> GetAllSubjects()
        {
            var data = (from a in _db.Subject select a).OrderByDescending(a => a.Id).ToList();
            return data;
        }
        public IQueryable<SubjectGridModel> GetAllSubjectsGrid()
        {
            return (from subject in _db.Subject
                    select new SubjectGridModel()
                    {
                        Id = subject.Id,
                        Name = subject.Name,
                        Code = subject.Code,
                        IsActive = subject.IsActive
                    }).AsQueryable();
        }
        public int CreateSubject(Subject subject)
        {
            try
            {
                _db.Subject.Add(subject);
                _db.SaveChanges();
                return subject.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateSubject(Subject subject)
        {
            try
            {
                _db.Entry(subject).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return subject.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Subject GetSubjectById(int id)
        {
            return _db.Subject.Find(id);
        }

        public Subject GetSubjectByName(string subjectName)
        {
            return _db.Subject.Where(x => x.Name == subjectName).FirstOrDefault();
        }
        public List<Subject> CheckDuplicateSubjectCode(string SubjectCode)
        {
            var getSubjectDetails = (from subject in _db.Subject
                                  where subject.Code.ToUpper().Trim() == SubjectCode.ToUpper().Trim()
                                  select subject).ToList();
            return getSubjectDetails;
        }
    }
}
