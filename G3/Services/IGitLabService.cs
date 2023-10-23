using NGitLab.Models;

namespace G3.Services
{
    public interface IGitLabService
    {

        public List<NGitLab.Models.Milestone> GetMilestoneByGroupId(int groupId);


    }
}

