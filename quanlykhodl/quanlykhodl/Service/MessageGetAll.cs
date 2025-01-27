namespace quanlykhodl.Service
{
    public class MessageGetAll
    {
        public int id { get; set; }
        public string? username {  get; set; }
        public string? image { get; set; }
        public List<MessageItem>? dataItem { get; set; }
    }

    public class MessageItem
    {
        public int id { get; set; }
        public int idUser1 { get; set; }
        public int idUser2 { get; set; }
        public string? image_user1 { get; set; }
        public string? image_user2 { get; set; }
        public string? name_user1 { get; set; }
        public string? name_user2 { get; set; }
        public string? message { get; set; }
        public string? imagedata { get; set; }
        public DateTimeOffset? CreateAt { get; set; }
    }
}
