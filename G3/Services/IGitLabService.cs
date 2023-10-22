using NGitLab.Models;

namespace G3.Services
{
    public interface IGitLabService
    {

        public Group CreateGroup(string className, string description);
        public void CreateProject(int ProjectId, string projectCode, string englishName, string vietnameseName, string description, int mentorId, int leaderId, string groupName, bool isActive = true);
        public void GetMilestones(List<int> ProjectIds);
        public NGitLab.Models.Milestone CreateMilestone(int projectId, string title, string description, DateTime dueDate, DateTime startDate);

    }
}

