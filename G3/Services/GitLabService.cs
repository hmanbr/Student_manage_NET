using NanoidDotNet;
using NGitLab;
using NGitLab.Models;

namespace G3.Services
{
    public class GitLabService : IGitLabService
    {
        private readonly IConfiguration Configuration;
        public GitLabClient client;
        public SWPContext context;

        public GitLabService(IConfiguration configuration, SWPContext context)
        {
            Configuration = configuration;
            client = new GitLabClient("https://gitlab.com", Configuration["GitLab:PrivateToken"]);
            this.context = context;
        }

        public Group CreateGroup(string className, string description)
        {
            var group = client.Groups.Create(new GroupCreate {
                Name = className,
                Path = className + Nanoid.Generate(size: 10),
                Visibility = NGitLab.Models.VisibilityLevel.Private,
                Description= description,
            });
            return group;
        }

        public void CreateProject(int ProjectId, string projectCode, string englishName, string vietnameseName, string description, int mentorId, int leaderId, string groupName, bool isActive = true)
        {
            //context.Projects.Add(new Models.Project
            //{
            //    Id = ProjectId,
            //    ProjectCode = projectCode,
            //    EnglishName = englishName,
            //    VietNameseName = vietnameseName,
            //    Description = description,
            //    MentorId = mentorId,
                
            //});
        }

        public void GetMilestones(List<int> ProjectIds)
        {
            foreach (var projectId in ProjectIds)
            {
                var milestoneClient = client.GetMilestone(projectId: projectId);
                List<NGitLab.Models.Milestone> milestones = milestoneClient.All.ToList();
            }
        }

        public NGitLab.Models.Milestone CreateMilestone(int projectId, string title, string description, DateTime dueDate, DateTime startDate)
        {
            var milestone = client.GetMilestone(projectId: projectId).Create(new NGitLab.Models.MilestoneCreate
            {
                Title = title,
                Description = description,
                StartDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                DueDate = dueDate.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return milestone;
        }
    }
}

