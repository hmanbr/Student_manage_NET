using NanoidDotNet;
using NGitLab;
using NGitLab.Models;

namespace G3.Services
{
    public class GitLabService
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

        public void CreateProject(int ProjectId, string projectCode, string englishName, string vietnameseName, string description, int mentorId, int leaderId, string groupName, bool isActive = true)
        {
            //
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

