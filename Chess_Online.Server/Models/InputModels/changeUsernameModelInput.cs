using Microsoft.AspNetCore.Mvc;

namespace Chess_Online.Server.Models.InputModels
{
    public class changeUsernameModelInput
    {
        public string oldLogin { get; set; }
        public string newLogin { get; set; }
    }
}
