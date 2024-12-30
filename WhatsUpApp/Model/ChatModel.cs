using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsUpApp.Model
{
    public class ChatModel
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public string LastMessage { get; set; }

    }
}
