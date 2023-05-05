using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReasonTogetherForum.Models;

namespace ReasonTogetherForum.Data
{
    public interface IApplicationUser
    {
        ApplicationUser GetById(string id);
        IEnumerable<ApplicationUser> GetAll();
        Task SetProfileImage(string id, Uri uri);
        Task UpdateUserRating(string userId, Type type);
    }
}
