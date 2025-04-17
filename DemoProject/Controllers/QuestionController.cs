using DemoProject.Model;
using DemoProject.Models;
using DemoProject.Service;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Kendo.Mvc.Extensions;
using DemoProject.Helper;
using System.Xml.Linq;
using System.IO;
using ClosedXML.Excel;
using Newtonsoft.Json;

namespace DemoProject.Controllers
{
    public class QuestionController : BaseController
    {

        private readonly QuestionService _questionService;
        private readonly SubjectService _subjectService;
        private readonly QuestionTypeService _questionTypeService;
        private readonly CommonLookupService _lookupService;
        private readonly OptionService _optionService;
        private readonly MediaService _mediaService;


        public QuestionController()
        {
            _questionService = new QuestionService();
            _questionTypeService = new QuestionTypeService();
            _subjectService = new SubjectService();
            _lookupService = new CommonLookupService();
            _optionService = new OptionService();
            _mediaService = new MediaService();
        }

        // GET: Question
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
                return RedirectToAction("AccessDenied", "Base");


            QuestionModel model = new QuestionModel();
            if (id > 0)
            {
                var getQuestion = _questionService.GetQuestionById(id.Value);
                if (getQuestion != null)
                {
                    model.Id = id.Value;
                    model.SubjectId = getQuestion.Subjects.Id;
                    model.QuestionTypeId = getQuestion.QuestionTypes.Id;
                    model.QuestionText = getQuestion.QuestionText;
                    model.DefaultMarks = getQuestion.DefaultMarks;
                    model.DifficultyLevel = getQuestion.DifficultyLevel;
                    model.IsActive = getQuestion.IsActive;
                    model.options = _optionService.GetOptionsByQuestionId(id.Value).Select(o => new OptionModel { Id = o.Id, QuestionId = o.QuestionId,OptionText = o.OptionText, IsCorrect = o.IsCorrect }).ToList();
                    model.mediaFiles = _mediaService.GetMediaByQuestionId(id.Value)
                .Select(m => new MediaModel
                {
                    Id = m.Id,
                    QuestionId = m.QuestionId,
                    MediaName = m.MediaName,
                    MediaType = m.MediaType,
                    IsDeleted = false
                }).ToList();
                }
            } 
            BindSubject(ref model);
            BindQuestionType(ref model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(QuestionModel model, IEnumerable<HttpPostedFileBase> files)
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            if (model.options.Count < 2 && (model.QuestionTypeId == 1 || model.QuestionTypeId == 2))
            {
                ModelState.AddModelError("options", "At least 2 options are required.");
            }

            int correctAnswers = model.options.Count(o => o.IsCorrect);

            if (model.QuestionTypeId == 1) // Radio (Single Choice)
            {
                if (correctAnswers != 1)
                {
                    ModelState.AddModelError("options", "For Radio questions, exactly 1 option must be correct.");
                }
            }
            else if (model.QuestionTypeId == 2) // Checkbox (Multiple Choice)
            {
                if (correctAnswers < 1)
                {
                    ModelState.AddModelError("options", "For Checkbox questions, at least 1 correct option is required.");
                }
            }

            if (files != null)
            {
                foreach (var file in files.Where(f => f != null && f.ContentLength > 0))
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        ModelState.AddModelError("mediaFiles", "Only JPG and PNG files are allowed.");
                        break;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                SaveUpdateQuestion(model, files);
                return RedirectToAction("Index");
            }
            else
            {
                BindSubject(ref model);
                BindQuestionType(ref model);
                return View(model);
            }
        }



        public QuestionModel BindSubject(ref QuestionModel model)
        {
            var getsubjects = _subjectService.GetAllSubjects().OrderBy(a => a.Name);
            model._SubjectList.Add(new SelectListItem() { Text = "Select Subject", Value = "" });
            foreach (var item in getsubjects)
            {
                model._SubjectList.Add(new SelectListItem() { Text = item.Name.Trim(), Value = item.Id.ToString() });
            }
            return model;
        }

        public QuestionModel BindQuestionType(ref QuestionModel model)
        {
            var getQuestionTypes = _questionTypeService.GetAllQuestionTypes().OrderBy(a => a.TypeCode);
            model._QuestionTypeList.Add(new SelectListItem() { Text = "Select Question Type", Value = "" });
            foreach(var item in getQuestionTypes)
            {
                model._QuestionTypeList.Add(new SelectListItem() { Text = item.TypeName.Trim(), Value = item.Id.ToString() });
            }
            return model;
        }

        public JsonResult GetDiffiCultyLevelData([DataSourceRequest] DataSourceRequest request)
        {
            List<SelectListItem> _DifficultyLevel = new List<SelectListItem>();
            var data = _lookupService.GetLookupByType(LookupType.DifficultyLevel);
            if(data.Count > 0)
            {
                data = data.OrderBy(x => x.Name).ToList();
            }
            _DifficultyLevel.Add(new SelectListItem() { Text = "Select Difficulty Level", Value = "" });
            foreach(var item in data)
            {
                _DifficultyLevel.Add(new SelectListItem() { Text = item.Name, Value = item.Code.ToString() });
            }
            return Json(_DifficultyLevel, JsonRequestBehavior.AllowGet);
        }

        public QuestionModel SaveUpdateQuestion(QuestionModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int userId = SessionHelper.UserId;
            Question obj = new Question();  
            if(model.Id > 0)
            {
                obj = _questionService.GetQuestionById(model.Id);
            }
            obj.Id = model.Id;
            obj.SubjectId = model.SubjectId;
            obj.QuestionTypeId = model.QuestionTypeId;
            obj.QuestionText = model.QuestionText;
            obj.DefaultMarks = model.DefaultMarks;
            obj.DifficultyLevel = model.DifficultyLevel;
            
            
            obj.IsActive = model.IsActive;
            if (obj.Id == 0)
            {
                obj.CreatedBy = SessionHelper.UserId;
                obj.CreatedOn = DateTime.UtcNow;
                model.Id = _questionService.CreateQuestion(obj);
              }
            else
            {
                obj.UpdatedBy = SessionHelper.UserId;
                obj.UpdatedOn = DateTime.UtcNow;
                _questionService.UpdateQuestion(obj);
            }

            if (model.options != null && model.options.Any())
            {
                var existingOptions = _optionService.GetOptionsByQuestionId(obj.Id).ToList();

                
                var incomingOptionIds = model.options.Select(o => o.Id).ToHashSet();

                foreach (var option in model.options)
                {
                    if (option.Id == 0)
                    {
                        
                        Option newOption = new Option
                        {
                            QuestionId = obj.Id,
                            OptionText = option.OptionText,
                            IsCorrect = option.IsCorrect,
                            CreatedBy = userId,
                            CreatedOn = DateTime.Now,
                        };
                        _optionService.CreateOption(newOption);
                    }
                    else
                    {
                        
                        Option existingOption = existingOptions.FirstOrDefault(o => o.Id == option.Id);
                        if (existingOption != null)
                        {
                            existingOption.OptionText = option.OptionText;
                            existingOption.IsCorrect = option.IsCorrect;
                            existingOption.UpdatedBy = userId;
                            existingOption.UpdatedOn = DateTime.Now;
                            _optionService.UpdateOption(existingOption);
                        }
                    }
                }

               
                var optionsToDelete = existingOptions.Where(o => !incomingOptionIds.Contains(o.Id)).ToList();
                foreach (var option in optionsToDelete)
                {
                    _optionService.DeleteOption(option.Id); 
                }
            }

            if (model.mediaFiles != null && model.mediaFiles.Any())
            {
                var filesToDelete = model.mediaFiles.Where(m => m.IsDeleted && m.Id > 0).ToList();
                var filesToDeleteList = new List<Media>();
                foreach (var file in filesToDelete)
                {
                    var media = _mediaService.GetMediaById(file.Id);
                    if (media != null)
                    {
                        filesToDeleteList.Add(media);
                        // Delete the physical file
                        var filePath = Server.MapPath("~/Content/QuestionImages/") + media.MediaName;
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                       
                    }
                }
                _mediaService.RemoveMultiMedia(filesToDeleteList);
            }

            if (files != null)
            {
                var filesToBeCreate = new List<Media>();
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(file.FileName).ToLower();

                        // Only process allowed file types
                        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string filePath = Server.MapPath("~/Content/QuestionImages/") + fileName;

                            // If the file already exists, add a timestamp to make it unique
                            if (System.IO.File.Exists(filePath))
                            {
                                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                                extension = Path.GetExtension(fileName);
                                fileName = $"{fileNameWithoutExtension}_{timestamp}{extension}";
                                filePath = Server.MapPath("~/Content/QuestionImages/") + fileName;
                            }

                            file.SaveAs(filePath);

                            // Save to media table
                            Media media = new Media
                            {
                                QuestionId = obj.Id,
                                MediaName = fileName,
                                MediaType = extension
                            };
                            filesToBeCreate.Add(media);
                        }
                    }
                }
                _mediaService.CreateMultiMedia(filesToBeCreate);
            }

            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request, string searchTerm)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _questionService.GetAllQuestionsGrid();

            var materializedData = data.ToList().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                materializedData = materializedData.Where(x =>
                    (x.SubjectName != null && x.SubjectName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.QuestionType != null && x.QuestionType.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.QuestionText != null && x.QuestionText.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.DifficultyLevel != null && x.DifficultyLevel.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    x.DefaultMarks.ToString().Contains(searchTerm)
                );
            }

            return Json(materializedData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Delete(int id)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsDelete))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            
                var result = _questionService.DeleteQuestion(id);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Question not found or deletion failed.";
                    return RedirectToAction("Index");
                }
        }


        public ActionResult ViewQuestion(int id, bool? IsShowExtraFields, bool onlyShowBody = false)
        {
            var getQuestion = _questionService.GetQuestionById(id);
            
            
                QuestionModel model = new QuestionModel();
                model.Id = id;
                model.SubjectId = getQuestion.Subjects.Id;
                model.QuestionTypeId = getQuestion.QuestionTypes.Id;
                model.QuestionText = getQuestion.QuestionText;
                model.DefaultMarks = getQuestion.DefaultMarks;
                model.DifficultyLevel = getQuestion.DifficultyLevel;
                model.IsActive = getQuestion.IsActive;
                model.options = _optionService.GetOptionsByQuestionId(id).Select(o => new OptionModel { Id = o.Id, QuestionId = o.QuestionId, OptionText = o.OptionText, IsCorrect = o.IsCorrect }).ToList();
            model.mediaFiles = _mediaService.GetMediaByQuestionId(id).Select(m => new MediaModel
            {
                Id = m.Id,
                QuestionId = m.QuestionId,
                MediaName = m.MediaName,
                MediaType = m.MediaType,
                IsDeleted = false
            }).ToList();


            TempData["IsIsShowExtraFields"] = IsShowExtraFields;

            if (onlyShowBody)
            {
                return PartialView("_ViewQuestionBody", model);
            }

            return PartialView("_ViewQuestion", model);
        }


       
public ActionResult ExportToExcel(string searchTerm)
    {
        if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsView))
        {
            return RedirectToAction("AccessDenied", "Base");
        }

        var data = _questionService.GetAllQuestionsGrid();
        var materializedData = data.ToList();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            materializedData = materializedData.Where(x =>
                (x.SubjectName != null && x.SubjectName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (x.QuestionType != null && x.QuestionType.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (x.QuestionText != null && x.QuestionText.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (x.DifficultyLevel != null && x.DifficultyLevel.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                x.DefaultMarks.ToString().Contains(searchTerm)
            ).ToList();
        }

        // Create Excel workbook
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Questions");

            // Set headers
            worksheet.Cell(1, 1).Value = "Subject";
            worksheet.Cell(1, 2).Value = "Question Type";
            worksheet.Cell(1, 3).Value = "Question Text";
            worksheet.Cell(1, 4).Value = "Default Marks";
            worksheet.Cell(1, 5).Value = "Difficulty Level";
            worksheet.Cell(1, 6).Value = "Active";
            worksheet.Cell(1, 7).Value = "Options";

            // Format header row
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Add data rows
            int rowIndex = 2;
            foreach (var question in materializedData)
            {
                worksheet.Cell(rowIndex, 1).Value = question.SubjectName;
                worksheet.Cell(rowIndex, 2).Value = question.QuestionType;
                worksheet.Cell(rowIndex, 3).Value = question.QuestionText;
                worksheet.Cell(rowIndex, 4).Value = question.DefaultMarks;
                worksheet.Cell(rowIndex, 5).Value = question.DifficultyLevel;
                worksheet.Cell(rowIndex, 6).Value = question.IsActive ? "Yes" : "No";

                // Get options for this question
                var options = _optionService.GetOptionsByQuestionId(question.Id);

                // Convert options to the requested JSON format
                var optionsJson = options.Select(o => new {
                    optionText = o.OptionText,
                    IsCorrect = o.IsCorrect
                }).ToList();

                // Serialize to JSON
                string jsonOptions = JsonConvert.SerializeObject(optionsJson, Formatting.Indented);
                worksheet.Cell(rowIndex, 7).Value = jsonOptions;

                rowIndex++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Ensure the options column isn't too wide
            worksheet.Column(7).Width = 100;

            // Set wrap text for options column
            worksheet.Column(7).Style.Alignment.WrapText = true;

            // Prepare for download
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Questions_Export.xlsx");
        }
    }

        // Add these methods to your QuestionController class

        [HttpGet]
        public ActionResult Import()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsAdd))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelFile)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsAdd))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            if (excelFile == null || excelFile.ContentLength <= 0)
            {
                ModelState.AddModelError("", "Please select an Excel file to import.");
                return View();
            }

            string extension = Path.GetExtension(excelFile.FileName).ToLower();
            if (extension != ".xlsx")
            {
                ModelState.AddModelError("", "Only .xlsx files are supported.");
                return View();
            }

            List<string> errorMessages = new List<string>();
            int successCount = 0; 
            int errorCount = 0;

            try
            {
                using (var workbook = new XLWorkbook(excelFile.InputStream))
                {
                    var worksheet = workbook.Worksheet(1); 
                    var rows = worksheet.RowsUsed();

                 
                    bool isFirstRow = true;
                    foreach (var row in rows)
                    {
                        if (isFirstRow)
                        {
                            isFirstRow = false;
                            continue;
                        }

                        try
                        {
                            
                            string subjectName = row.Cell(1).GetString().Trim();
                            string questionType = row.Cell(2).GetString().Trim();
                            string questionText = row.Cell(3).GetString().Trim();

                            int defaultMarks;
                            bool validMarks = int.TryParse(row.Cell(4).GetString(), out defaultMarks);
                            if (!validMarks)
                            {
                                errorMessages.Add($"Row {row.RowNumber()}: Invalid Default Marks format");
                                errorCount++;
                                continue;
                            }

                            string difficultyLevel = row.Cell(5).GetString().Trim();
                            bool isActive = row.Cell(6).GetString().Trim().ToLower() == "yes";
                            string optionsJson = row.Cell(7).GetString().Trim();

                           
                            var subject = _subjectService.GetAllSubjects().FirstOrDefault(s => s.Name.Trim().Equals(subjectName, StringComparison.OrdinalIgnoreCase));
                            if (subject == null)
                            {
                                errorMessages.Add($"Row {row.RowNumber()}: Subject '{subjectName}' not found");
                                errorCount++;
                                continue;
                            }

                            var qType = _questionTypeService.GetAllQuestionTypes().FirstOrDefault(qt => qt.TypeName.Trim().Equals(questionType, StringComparison.OrdinalIgnoreCase));
                            if (qType == null)
                            {
                                errorMessages.Add($"Row {row.RowNumber()}: Question Type '{questionType}' not found");
                                errorCount++;
                                continue;
                            }

                            
                            var difficultyLevelData = _lookupService.GetLookupByType(LookupType.DifficultyLevel)
                                .FirstOrDefault(d => d.Name.Equals(difficultyLevel, StringComparison.OrdinalIgnoreCase));
                            if (difficultyLevelData == null)
                            {
                                errorMessages.Add($"Row {row.RowNumber()}: Difficulty Level '{difficultyLevel}' not found");
                                errorCount++;
                                continue;
                            }

                            
                            List<OptionModel> options = new List<OptionModel>();
                            try
                            {
                                dynamic optionsData = JsonConvert.DeserializeObject(optionsJson);
                                foreach (var option in optionsData)
                                {
                                    options.Add(new OptionModel
                                    {
                                        OptionText = option.optionText,
                                        IsCorrect = option.IsCorrect
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                errorMessages.Add($"Row {row.RowNumber()}: Error parsing options: {ex.Message}");
                                errorCount++;
                                continue;
                            }

                           
                            if (options.Count < 2 && (qType.Id == 1 || qType.Id == 2))
                            {
                                errorMessages.Add($"Row {row.RowNumber()}: At least 2 options are required for {questionType}");
                                errorCount++;
                                continue;
                            }

                            int correctAnswers = options.Count(o => o.IsCorrect);

                            if (qType.Id == 1) 
                            {
                                if (correctAnswers != 1)
                                {
                                    errorMessages.Add($"Row {row.RowNumber()}: For Radio questions, exactly 1 option must be correct");
                                    errorCount++;
                                    continue;
                                }
                            }
                            else if (qType.Id == 2) 
                            {
                                if (correctAnswers < 1)
                                {
                                    errorMessages.Add($"Row {row.RowNumber()}: For Checkbox questions, at least 1 correct option is required");
                                    errorCount++;
                                    continue;
                                }
                            }
                            else
                            {
                                
                                if (options.Count < 1)
                                {
                                    errorMessages.Add($"Row {row.RowNumber()}: At least 1 option is required");
                                    errorCount++;
                                    continue;
                                }
                            }

                           
                            QuestionModel model = new QuestionModel
                            {
                                SubjectId = subject.Id,
                                QuestionTypeId = qType.Id,
                                QuestionText = questionText,
                                DefaultMarks = defaultMarks,
                                DifficultyLevel = difficultyLevelData.Code,
                                IsActive = isActive,
                                options = options
                            };

                           
                            SaveUpdateQuestion(model, null);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            errorMessages.Add($"Row {row.RowNumber()}: {ex.Message}");
                            errorCount++;
                        }
                    }
                }

               
                TempData["SuccessCount"] = successCount;
                TempData["ErrorCount"] = errorCount;
                TempData["ErrorMessages"] = errorMessages;

                return View("ImportResult");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error processing the file: {ex.Message}");
                return View();
            }
        }

       

        public ActionResult DownloadTemplate()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Template");

                
                worksheet.Cell(1, 1).Value = "Subject";
                worksheet.Cell(1, 2).Value = "Question Type";
                worksheet.Cell(1, 3).Value = "Question Text";
                worksheet.Cell(1, 4).Value = "Default Marks";
                worksheet.Cell(1, 5).Value = "Difficulty Level";
                worksheet.Cell(1, 6).Value = "Active";
                worksheet.Cell(1, 7).Value = "Options";

               
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                worksheet.Cell(2, 1).Value = "[Subject Name]";
                worksheet.Cell(2, 2).Value = "Radio";
                worksheet.Cell(2, 3).Value = "[Your question text here]";
                worksheet.Cell(2, 4).Value = "1";
                worksheet.Cell(2, 5).Value = "Easy";
                worksheet.Cell(2, 6).Value = "Yes";

                
                string sampleOptionsRadio = @"[
  {
    ""optionText"": ""Option 1"",
    ""IsCorrect"": true
  },
  {
    ""optionText"": ""Option 2"",
    ""IsCorrect"": false
  }
]";
                worksheet.Cell(2, 7).Value = sampleOptionsRadio;

                
                worksheet.Cell(3, 1).Value = "[Subject Name]";
                worksheet.Cell(3, 2).Value = "Checkbox";
                worksheet.Cell(3, 3).Value = "[Your multiple choice question here]";
                worksheet.Cell(3, 4).Value = "2.0";
                worksheet.Cell(3, 5).Value = "Medium";
                worksheet.Cell(3, 6).Value = "Yes";

                
                string sampleOptionsCheckbox = @"[
  {
    ""optionText"": ""Option 1"",
    ""IsCorrect"": true
  },
  {
    ""optionText"": ""Option 2"",
    ""IsCorrect"": true
  },
  {
    ""optionText"": ""Option 3"",
    ""IsCorrect"": false
  }
]";
                worksheet.Cell(3, 7).Value = sampleOptionsCheckbox;

                
                var infoSheet = workbook.Worksheets.Add("Instructions");

               
                infoSheet.Cell(1, 1).Value = "Available Subjects:";
                infoSheet.Cell(1, 1).Style.Font.Bold = true;

                var subjects = _subjectService.GetAllSubjects().OrderBy(a => a.Name).ToList();
                for (int i = 0; i < subjects.Count; i++)
                {
                    infoSheet.Cell(i + 2, 1).Value = subjects[i].Name;
                }

               
                infoSheet.Cell(1, 3).Value = "Available Question Types:";
                infoSheet.Cell(1, 3).Style.Font.Bold = true;

                var questionTypes = _questionTypeService.GetAllQuestionTypes().OrderBy(a => a.TypeCode).ToList();
                for (int i = 0; i < questionTypes.Count; i++)
                {
                    infoSheet.Cell(i + 2, 3).Value = questionTypes[i].TypeName;
                }

                
                infoSheet.Cell(1, 5).Value = "Available Difficulty Levels:";
                infoSheet.Cell(1, 5).Style.Font.Bold = true;

                var difficultyLevels = _lookupService.GetLookupByType(LookupType.DifficultyLevel).OrderBy(x => x.Name).ToList();
                for (int i = 0; i < difficultyLevels.Count; i++)
                {
                    infoSheet.Cell(i + 2, 5).Value = difficultyLevels[i].Name;
                }

               
                infoSheet.Cell(1, 7).Value = "Validation Rules:";
                infoSheet.Cell(1, 7).Style.Font.Bold = true;

                infoSheet.Cell(2, 7).Value = "1. For Radio questions: At least 2 options with exactly 1 correct option";
                infoSheet.Cell(3, 7).Value = "2. For Checkbox questions: At least 2 options with at least 1 correct option";
                infoSheet.Cell(4, 7).Value = "3. For other question types: At least 1 option";
                infoSheet.Cell(5, 7).Value = "4. Subject, Question Type, and Difficulty Level must exist in the system";

                
                worksheet.Columns().AdjustToContents();
                infoSheet.Columns().AdjustToContents();

               
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Question_Import_Template.xlsx");
            }
        }

    }
}