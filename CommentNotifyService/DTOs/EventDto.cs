namespace CommentNotifyService.DTOs
{
    // Cấu trúc hứng sự kiện khi một Task thay đổi trạng thái hoặc được giao
    public class TaskEventDto
    {
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public Guid AssigneeId { get; set; } // ID người được giao việc
        public string Status { get; set; } = string.Empty; // To Do, In Progress, Done...
        public string ActionBy { get; set; } = string.Empty; // Tên người thực hiện hành động
    }
}