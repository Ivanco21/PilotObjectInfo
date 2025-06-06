using System;

namespace PilotObjectInfo.Models
{
    public record UserStateModel
    {
        public UserStateModel()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }
}