using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using QuizManagement.Data.Entities.Quiz;
using QuizManagement.Data.Entities.System;
using QuizManagement.Data.Enum;

namespace QuizManagement.DataEF.Connector
{
    public class DbInitializer
    {
        private readonly AppDbContext _appDbContext;
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;

        public DbInitializer(AppDbContext appDbContext, UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Seed()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new AppRole()
                {
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Description = "Top manager"
                });
                await _roleManager.CreateAsync(new AppRole()
                {
                    Name = "Teacher",
                    NormalizedName = "Teacher",
                    Description = "Teacher"
                });
            }

            if (!_userManager.Users.Any())
            {
                await _userManager.CreateAsync(new AppUser()
                {
                    UserName = "admin",
                    FullName = "Administrator",
                    Email = "admin@gmail.com",
                    Balance = 0,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Status = Status.Active
                }, "123654$");
                var user = await _userManager.FindByNameAsync("admin");
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            if (!_appDbContext.Chapters.Any())
            {
                List<Chapter> listChapter = new List<Chapter>();
                for (int i = 1; i <= 10; i++)
                {
                    Chapter chapter = new Chapter() {Name = "Chapter" + i, Status = Status.Active};
                    listChapter.Add(chapter);
                }

                _appDbContext.Chapters.AddRange(listChapter);
            }

            if (!_appDbContext.Subjects.Any())
            {
                List<Subject> listSubject = new List<Subject>()
                {
                    new Subject() {Name = "Mạng máy tính"},
                    new Subject() {Name = "Thông tin di động"},
                    new Subject() {Name = "Thông tin vô tuyến"},
                    new Subject() {Name = "Lập trình nâng cao"},
                    new Subject() {Name = "Cơ sở truyền số liệu"}
                };
                _appDbContext.Subjects.AddRange(listSubject);
            }

//            if (!_appDbContext.SubjectChapterDetails.Any())
//            {
//                List<SubjectChapterDetail> listSubjectChapterDetails = new List<SubjectChapterDetail>();
//                for (int i = 6; i <= 10; i++)
//                {
//                    for (int j = 11; j <= 20; j++)
//                    {
//                        SubjectChapterDetail subjectChapterDetail = new SubjectChapterDetail()
//                            {SubjectId = i, ChapterId = j};
//                        listSubjectChapterDetails.Add(subjectChapterDetail);
//                    }
//                }
//
//                _appDbContext.SubjectChapterDetails.AddRange(listSubjectChapterDetails);
//            }

            if (!_appDbContext.Functions.Any())
            {
                _appDbContext.Functions.AddRange(new List<Function>()
                {
                    new Function()
                    {
                        Id = "SYSTEM", Name = "System", ParentId = null, SortOrder = 1, Status = Status.Active,
                        URL = "/", IconCss = "fa fa-desktop"
                    },
                    new Function()
                    {
                        Id = "ROLE", Name = "Role", ParentId = "SYSTEM", SortOrder = 1, Status = Status.Active,
                        URL = "/admin/role/index", IconCss = "fa fa-home"
                    },
                    new Function()
                    {
                        Id = "USER", Name = "User", ParentId = "SYSTEM", SortOrder = 2, Status = Status.Active,
                        URL = "/admin/user/index", IconCss = "fa-home"
                    },
                    new Function()
                    {
                        Id = "MANAGE", Name = "Manage", ParentId = null, SortOrder = 2, Status = Status.Active,
                        URL = "/admin/manage/index", IconCss = "fa fa-clone"
                    },
                    new Function()
                    {
                        Id = "CHAPTER", Name = "Chapter", ParentId = "MANAGE", SortOrder = 1, Status = Status.Active,
                        URL = "/admin/chapter/index", IconCss = "fa fa-clone"
                    },
                    new Function()
                    {
                        Id = "SUBJECT", Name = "Subject", ParentId = "MANAGE", SortOrder = 2, Status = Status.Active,
                        URL = "/admin/subject/index", IconCss = "fa fa-clone"
                    },
                    new Function()
                    {
                        Id = "QUESTION", Name = "Question", ParentId = "MANAGE", SortOrder = 3, Status = Status.Active,
                        URL = "/admin/question/index", IconCss = "fa fa-clone"
                    },
                    new Function()
                    {
                        Id = "EXAM", Name = "Exam", ParentId = "MANAGE", SortOrder = 4, Status = Status.Active,
                        URL = "/admin/exam/index", IconCss = "fa fa-clone"
                    }
                });
            }

            await _appDbContext.SaveChangesAsync();
        }
    }
}