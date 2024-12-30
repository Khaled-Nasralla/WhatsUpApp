using Microsoft.Maui.Controls;
using WhatsUpApp.Model;
using System.Diagnostics;

namespace WhatsUpApp.View
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MyMessageTemplate { get; set; }
        public DataTemplate OtherMessageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = item as MessageModel;
            if (message == null)
                return null;

            Debug.WriteLine($"Message from: {message.Sender}");

            if (message.Sender == "Me")
            {
                Debug.WriteLine("Using MyMessageTemplate");
                return MyMessageTemplate;
            }
            else
            {
                Debug.WriteLine("Using OtherMessageTemplate");
                return OtherMessageTemplate;
            }
        }

    }
}