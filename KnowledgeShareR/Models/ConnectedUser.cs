namespace KnowledgeShareR.Models
{
    public class ConnectedUser
    {
        public int ConnectedUserId { get; set; }
        public string AspNetUserId { get; set; }
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
    }
}