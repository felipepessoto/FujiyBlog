namespace FujiyBlog.Core.DomainObjects
{
    public enum Permission
    {
        #region Misc

        /// <summary>
        /// A user is allowed to view exception messages.
        /// </summary>
        //ViewDetailedErrorMessages,

        /// <summary>
        /// A user is allowed to access administration pages.
        /// Typically, a blog where self-registration is allowed
        /// would restrict this right from guest users.
        /// </summary>
        AccessAdminPages,

        /// <summary>
        /// A user is allowed to access admin settings pages.
        /// </summary>
        AccessAdminSettingsPages,

        /// <summary>
        /// A user is allowed to manage widgets.
        /// </summary>
        ManageWidgets,

        #endregion

        #region "Comments"

        /// <summary>
        /// A user is allowed to view comments on a post.
        /// </summary>
        ViewPublicComments,

        /// <summary>
        /// A user is allowed to view comments that have not been moderation yet.
        /// </summary>
        ViewUnmoderatedComments,

        /// <summary>
        /// A user is allowed to create and submit comments for posts or pages.
        /// </summary>
        CreateComments,

        /// <summary>
        /// User can approve, delete, or mark comments as spam.
        /// </summary>
        ModerateComments,

        #endregion
        
        #region Posts

        /// <summary>
        /// A user is allowed to view posts that are both published and public.
        /// </summary>
        
        ViewPublicPosts,

        /// <summary>
        /// A user is allowed to view unpublished posts.
        /// </summary>
        
        ViewUnpublishedPosts,

        /// <summary>
        /// A user is allowed to view non-public posts.
        /// </summary>
        // 11/6/2010 - commented out, we don't currently have "private" posts, just unpublished.
        //
        //ViewPrivatePosts,

        /// <summary>
        /// A user can create new posts. 
        /// </summary>
        
        CreateNewPosts,

        /// <summary>
        /// A user can edit their own posts. 
        /// </summary>
        
        EditOwnPosts,

        /// <summary>
        /// A user can edit posts created by other users.
        /// </summary>
        
        EditOtherUsersPosts,

        /// <summary>
        /// A user can delete their own posts.
        /// </summary>
        
        DeleteOwnPosts,

        /// <summary>
        /// A user can delete posts created by other users.
        /// </summary>
        
        DeleteOtherUsersPosts,

        /// <summary>
        /// A user can set whether or not their own posts are published.
        /// </summary>
        
        PublishOwnPosts,

        /// <summary>
        /// A user can set whether or not another user's posts are published.
        /// </summary>
        
        PublishOtherUsersPosts,

        #endregion

        #region Pages

        /// <summary>
        /// A user can view public, published pages.
        /// </summary>
        
        ViewPublicPages,

        /// <summary>
        /// A user can view unpublished pages.
        /// </summary>
        
        ViewUnpublishedPages,

        /// <summary>
        /// A user can create new pages.
        /// </summary>
        
        CreateNewPages,

        /// <summary>
        /// A user can edit pages they've created.
        /// </summary>
        
        EditOwnPages,

        /// <summary>
        /// A user can edit pages other users have created.
        /// </summary>
        
        EditOtherUsersPages,

        /// <summary>
        /// A user can delete pages they've created.
        /// </summary>
        
        DeleteOwnPages,

        /// <summary>
        /// A user can delete pages other users have created.
        /// </summary>
        
        DeleteOtherUsersPages,

        /// <summary>
        /// A user can set whether or not their own pages are published.
        /// </summary>
        
        PublishOwnPages,

        /// <summary>
        /// A user can set whether or not another user's pages are published.
        /// </summary>
        
        PublishOtherUsersPages,

        #endregion

        #region "Ratings"
        /// <summary>
        /// A user can view ratings on posts.
        /// </summary>
        //ViewRatingsOnPosts,

        /// <summary>
        /// A user can submit ratings on posts.
        /// </summary>
        //SubmitRatingsOnPosts,
        #endregion

        #region Roles

        /// <summary>
        /// A user can view roles.
        /// </summary>
        ViewRoles,

        /// <summary>
        /// A user can create new roles.
        /// </summary>
        CreateNewRoles,

        /// <summary>
        /// A user can edit existing roles.
        /// </summary>
        EditRoles,

        /// <summary>
        /// A user can delete existing roles.
        /// </summary>
        DeleteRoles,

        /// <summary>
        /// A user is allowed to edit their own roles.
        /// </summary>
        EditOwnRoles,

        /// <summary>
        /// A user is allowed to edit the roles of other users.
        /// </summary>
        EditOtherUsersRoles,

        #endregion

        #region Users

        /// <summary>
        /// A user is allowed to register/create a new account. 
        /// </summary>
        CreateNewUsers,

        /// <summary>
        /// A user is allowed to delete their own account.
        /// </summary>
        DeleteUserSelf,

        /// <summary>
        /// A user is allowed to delete accounts they do not own.
        /// </summary>
        DeleteUsersOtherThanSelf,

        /// <summary>
        /// A user is allowed to edit their own account information.
        /// </summary>
        EditOwnUser,

        /// <summary>
        /// A user is allowed to edit the account information of other users.
        /// </summary>
        EditOtherUsers,

        #endregion
    }
}
