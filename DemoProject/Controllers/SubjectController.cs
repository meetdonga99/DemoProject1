using DemoProject.Model;
using DemoProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoProject.Service;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using DemoProject.Helper;
using DemoProject.Controllers;
using ClosedXML.Excel;
using System.IO;

namespace DemoProject.Controllers
{
    [Authorize]
    public class SubjectController : BaseController
    {
        // GET: Roles

        private readonly SubjectService _subjectService;
        private readonly MessageService _messageService;
        public SubjectController()
        {
            _subjectService = new SubjectService();
            _messageService = new MessageService();
        }
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            return View();
        }

        public ActionResult Create(int? id)
        {
            string actionPermission = "";
            if (id == null)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if ((id ?? 0) > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            int userId = SessionHelper.UserId;
            SubjectModel model = new SubjectModel();
            if (id.HasValue)
            {
                var subjectDetail = _subjectService.GetSubjectById(id.Value);
                if (subjectDetail != null)
                {
                    model.Id = id.Value;
                    model.Name = subjectDetail.Name;
                    model.Code = subjectDetail.Code;
                    model.IsActive = subjectDetail.IsActive;
                    model.CreatedBy = userId;
                    model.CreatedOn = DateTime.UtcNow;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SubjectModel model)
        {
            string actionPermission = "";
            if (model.Id == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.Id > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveUpdateSubjects(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
         
        public SubjectModel SaveUpdateSubjects(SubjectModel model)
        {
            Subject obj = new Subject();
            if (model.Id > 0)
            {
                obj = _subjectService.GetSubjectById(model.Id);
            }
            int userId = SessionHelper.UserId;
            obj.Id = model.Id;
            obj.Name = model.Name;
            obj.IsActive = model.IsActive;
            obj.UpdatedBy = userId;
            obj.UpdatedOn = DateTime.UtcNow;
            if (obj.Id == 0)
            {
                obj.Code = model.Code;
                model.Id = _subjectService.CreateSubject(obj);
            }
            else
            {
                _subjectService.UpdateSubject(obj);
            }
            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request, string searchTerm)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _subjectService.GetAllSubjectsGrid();

            var materializedData = data.ToList().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                materializedData = materializedData.Where(x =>
                    (x.Name != null && x.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.Code != null && x.Code.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }

            return Json(materializedData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public JsonResult CheckDuplicateSubjectCode(string Code, int Id)
        {
            var getSubjectDetails = _subjectService.CheckDuplicateSubjectCode(Code);
            if (Id > 0)
            {
                getSubjectDetails = getSubjectDetails.Where(a => a.Id != Id).ToList();
            }
            if (getSubjectDetails.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(Constants.MessageCode.CODEEXIST);
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase excelFile)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), AccessPermission.IsAdd))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            if (excelFile == null || excelFile.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "Please select an Excel file to import.";
                return RedirectToAction("Index");
            }

            try
            {
                // Check file extension
                string fileExtension = Path.GetExtension(excelFile.FileName).ToLower();
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    TempData["ErrorMessage"] = "Please upload a valid Excel file (.xls or .xlsx).";
                    return RedirectToAction("Index");
                }

                List<SubjectModel> subjectsToImport = new List<SubjectModel>();
                List<string> errorMessages = new List<string>();
                int rowIndex = 0;

                using (var stream = new MemoryStream())
                {
                    excelFile.InputStream.CopyTo(stream);
                    stream.Position = 0;

                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1); // Get first worksheet
                        var rows = worksheet.RowsUsed();

                        bool isFirstRow = true;
                        foreach (var row in rows)
                        {
                            // Skip header row
                            if (isFirstRow)
                            {
                                isFirstRow = false;
                                continue;
                            }

                            rowIndex = row.RowNumber();

                            string name = row.Cell(1).GetString().Trim();
                            string code = row.Cell(2).GetString().Trim();
                            string isActiveStr = row.Cell(3).GetString().Trim();

                            // Basic validation
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                errorMessages.Add($"Row {rowIndex}: Subject Name is required.");
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(code))
                            {
                                errorMessages.Add($"Row {rowIndex}: Subject Code is required.");
                                continue;
                            }

                            bool isActive = true;
                            if (!string.IsNullOrWhiteSpace(isActiveStr))
                            {
                                isActiveStr = isActiveStr.ToLower();
                                if (isActiveStr == "no" || isActiveStr == "false" || isActiveStr == "0")
                                {
                                    isActive = false;
                                }
                            }

                            // Check for duplicate code in database
                            var existingSubjectsWithCode = _subjectService.CheckDuplicateSubjectCode(code);
                            if (existingSubjectsWithCode.Count > 0)
                            {
                                errorMessages.Add($"Row {rowIndex}: Subject Code '{code}' already exists in the system.");
                                continue;
                            }

                            // Check for duplicate code in imported data
                            if (subjectsToImport.Any(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
                            {
                                errorMessages.Add($"Row {rowIndex}: Duplicate Subject Code '{code}' found in import file.");
                                continue;
                            }

                            var subject = new SubjectModel
                            {
                                Name = name,
                                Code = code,
                                IsActive = isActive,
                                CreatedBy = SessionHelper.UserId,
                                CreatedOn = DateTime.UtcNow
                            };

                            subjectsToImport.Add(subject);
                        }
                    }
                }

                // If there are errors, return them to the user
                if (errorMessages.Count > 0)
                {
                    TempData["ErrorMessage"] = "Import failed with the following errors:<br/>" + string.Join("<br/>", errorMessages);
                    return RedirectToAction("Index");
                }

                // Save all valid subjects
                int importedCount = 0;
                foreach (var subject in subjectsToImport)
                {
                    SaveUpdateSubjects(subject);
                    importedCount++;
                }

                TempData["SuccessMessage"] = $"Successfully imported {importedCount} subjects.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred during import: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public ActionResult DownloadTemplate()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Subjects");

                // Add headers
                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Code";
                worksheet.Cell(1, 3).Value = "IsActive (Yes/No)";

                // Format headers
                var headerRange = worksheet.Range(1, 1, 1, 3);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Add sample data (optional)
                worksheet.Cell(2, 1).Value = "Mathematics";
                worksheet.Cell(2, 2).Value = "MATH101";
                worksheet.Cell(2, 3).Value = "Yes";

                worksheet.Cell(3, 1).Value = "Physics";
                worksheet.Cell(3, 2).Value = "PHYS101";
                worksheet.Cell(3, 3).Value = "Yes";

                // Auto fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SubjectsImportTemplate.xlsx");
                }
            }
        }

        public ActionResult ExportToExcel(string searchTerm)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            try
            {
                // Get all subjects data
                var data = _subjectService.GetAllSubjectsGrid();
                var materializedData = data.ToList().AsQueryable();


                // Apply filter if search term is provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    data = (IQueryable<Model.SubjectGridModel>)materializedData.Where(x =>
                        (x.Name != null && x.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (x.Code != null && x.Code.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    ).ToList();
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Subjects");

                    // Add headers
                    worksheet.Cell(1, 1).Value = "Name";
                    worksheet.Cell(1, 2).Value = "Code";
                    worksheet.Cell(1, 3).Value = "Active";

                    // Format headers
                    var headerRange = worksheet.Range(1, 1, 1, 3);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Add data
                    int row = 2;
                    foreach (var subject in data)
                    {
                        worksheet.Cell(row, 1).Value = subject.Name;
                        worksheet.Cell(row, 2).Value = subject.Code;
                        worksheet.Cell(row, 3).Value = subject.IsActive ? "Yes" : "No";
                        row++;
                    }

                    // Auto fit columns
                    worksheet.Columns().AdjustToContents();

                    // Prepare the response
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string fileName = $"Subjects_Export_{timestamp}.xlsx";

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred during export: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

    }
}