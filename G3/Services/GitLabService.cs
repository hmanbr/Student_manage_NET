using NanoidDotNet;
using NGitLab;
using NGitLab.Models;

namespace G3.Services
{
    public class GitLabService: IGitLabService
    {
        private readonly IConfiguration Configuration;
        public GitLabClient client;

        public GitLabService(IConfiguration configuration)
        {
            Configuration = configuration;
            client = new GitLabClient("https://gitlab.com", Configuration["GitLab:PrivateToken"]);
        }

        public List<NGitLab.Models.Milestone> GetMilestoneByGroupId(int groupId)
        {
            IMilestoneClient milestoneClient = client.GetGroupMilestone(groupId);
            return milestoneClient.All.ToList();
        }
    }
}

