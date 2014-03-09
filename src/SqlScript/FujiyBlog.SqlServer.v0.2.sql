BEGIN TRAN
BEGIN TRY

CREATE TABLE [PostComments] (
    [Id] [int] NOT NULL IDENTITY,
    [AuthorName] [varchar](50),
    [AuthorEmail] [varchar](255),
    [AuthorWebsite] [varchar](200),
    [Comment] [varchar](max) NOT NULL,
    [IpAddress] [varchar](45) NOT NULL,
    [Avatar] [varchar](200),
    [CreationDate] [datetime] NOT NULL,
    [IsApproved] [bit] NOT NULL,
    [IsDeleted] [bit] NOT NULL,
    [Author_Id] [int],
    [ModeratedBy_Id] [int],
    [Post_Id] [int] NOT NULL,
    [ParentComment_Id] [int],
    PRIMARY KEY ([Id])
)
CREATE TABLE [Posts] (
    [Id] [int] NOT NULL IDENTITY,
    [Title] [varchar](200) NOT NULL,
    [Description] [varchar](500),
    [Slug] [varchar](200) NOT NULL,
    [Content] [varchar](max),
    [CreationDate] [datetime] NOT NULL,
    [LastModificationDate] [datetime] NOT NULL,
    [PublicationDate] [datetime] NOT NULL,
    [IsPublished] [bit] NOT NULL,
    [IsCommentEnabled] [bit] NOT NULL,
    [IsDeleted] [bit] NOT NULL,
    [Author_Id] [int] NOT NULL,
    PRIMARY KEY ([Id])
)
CREATE TABLE [Users] (
    [Id] [int] NOT NULL IDENTITY,
    [Username] [varchar](20) NOT NULL,
    [Email] [varchar](255) NOT NULL,
    [Password] [varchar](50) NOT NULL,
    [DisplayName] [varchar](20),
    [FullName] [varchar](100),
    [Location] [varchar](20),
    [CreationDate] [datetime] NOT NULL,
    [LastLoginDate] [datetime],
    [About] [varchar](500),
    [BirthDate] [datetime],
    [Enabled] [bit] NOT NULL,
    PRIMARY KEY ([Id])
)
CREATE TABLE [RoleGroups] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [varchar](50) NOT NULL,
    [AssignedRoles] [varchar](max),
    PRIMARY KEY ([Id])
)
CREATE TABLE [Tags] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [varchar](50) NOT NULL,
    PRIMARY KEY ([Id])
)
CREATE TABLE [Categories] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [varchar](50) NOT NULL,
    PRIMARY KEY ([Id])
)
CREATE TABLE [Settings] (
    [Id] [int] NOT NULL,
    [Description] [varchar](500) NOT NULL,
    [Value] [varchar](max),
    PRIMARY KEY ([Id])
)
CREATE TABLE [WidgetSettings] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [varchar](50) NOT NULL,
    [WidgetZone] [varchar](50) NOT NULL,
    [Position] [int] NOT NULL,
    [Settings] [varchar](max),
    PRIMARY KEY ([Id])
)
CREATE TABLE [Pages] (
    [Id] [int] NOT NULL IDENTITY,
    [Title] [varchar](200) NOT NULL,
    [Description] [varchar](500),
    [Slug] [varchar](200) NOT NULL,
    [Content] [varchar](max),
    [Keywords] [varchar](500),
    [CreationDate] [datetime] NOT NULL,
    [LastModificationDate] [datetime] NOT NULL,
    [PublicationDate] [datetime] NOT NULL,
    [IsPublished] [bit] NOT NULL,
    [IsFrontPage] [bit] NOT NULL,
    [ParentId] [int],
    [ShowInList] [bit] NOT NULL,
    [IsDeleted] [bit] NOT NULL,
    [Author_Id] [int] NOT NULL,
    PRIMARY KEY ([Id])
)
CREATE TABLE [RoleGroupUsers] (
    [RoleGroup_Id] [int] NOT NULL,
    [User_Id] [int] NOT NULL,
    PRIMARY KEY ([RoleGroup_Id], [User_Id])
)
CREATE TABLE [TagPosts] (
    [Tag_Id] [int] NOT NULL,
    [Post_Id] [int] NOT NULL,
    PRIMARY KEY ([Tag_Id], [Post_Id])
)
CREATE TABLE [CategoryPosts] (
    [Category_Id] [int] NOT NULL,
    [Post_Id] [int] NOT NULL,
    PRIMARY KEY ([Category_Id], [Post_Id])
)
ALTER TABLE [PostComments] ADD CONSTRAINT [FK_PostComments_Users_Author_Id] FOREIGN KEY ([Author_Id]) REFERENCES [Users] ([Id])
ALTER TABLE [PostComments] ADD CONSTRAINT [FK_PostComments_Users_ModeratedBy_Id] FOREIGN KEY ([ModeratedBy_Id]) REFERENCES [Users] ([Id])
ALTER TABLE [PostComments] ADD CONSTRAINT [FK_PostComments_Posts_Post_Id] FOREIGN KEY ([Post_Id]) REFERENCES [Posts] ([Id])
ALTER TABLE [PostComments] ADD CONSTRAINT [FK_PostComments_PostComments_ParentComment_Id] FOREIGN KEY ([ParentComment_Id]) REFERENCES [PostComments] ([Id])
ALTER TABLE [Posts] ADD CONSTRAINT [FK_Posts_Users_Author_Id] FOREIGN KEY ([Author_Id]) REFERENCES [Users] ([Id])
ALTER TABLE [Pages] ADD CONSTRAINT [FK_Pages_Pages_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Pages] ([Id])
ALTER TABLE [Pages] ADD CONSTRAINT [FK_Pages_Users_Author_Id] FOREIGN KEY ([Author_Id]) REFERENCES [Users] ([Id])
ALTER TABLE [RoleGroupUsers] ADD CONSTRAINT [FK_RoleGroupUsers_RoleGroups_RoleGroup_Id] FOREIGN KEY ([RoleGroup_Id]) REFERENCES [RoleGroups] ([Id])
ALTER TABLE [RoleGroupUsers] ADD CONSTRAINT [FK_RoleGroupUsers_Users_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [Users] ([Id])
ALTER TABLE [TagPosts] ADD CONSTRAINT [FK_TagPosts_Tags_Tag_Id] FOREIGN KEY ([Tag_Id]) REFERENCES [Tags] ([Id])
ALTER TABLE [TagPosts] ADD CONSTRAINT [FK_TagPosts_Posts_Post_Id] FOREIGN KEY ([Post_Id]) REFERENCES [Posts] ([Id])
ALTER TABLE [CategoryPosts] ADD CONSTRAINT [FK_CategoryPosts_Categories_Category_Id] FOREIGN KEY ([Category_Id]) REFERENCES [Categories] ([Id])
ALTER TABLE [CategoryPosts] ADD CONSTRAINT [FK_CategoryPosts_Posts_Post_Id] FOREIGN KEY ([Post_Id]) REFERENCES [Posts] ([Id])















insert [dbo].[RoleGroups]
       ([Name],
        [AssignedRoles])
values ('Admin' ,
        'AccessAdminPages,AccessAdminSettingsPages,ManageWidgets,ViewPublicComments,ViewUnmoderatedComments,CreateComments,ModerateComments,ViewPublicPosts,ViewUnpublishedPosts,CreateNewPosts,EditOwnPosts,EditOtherUsersPosts,DeleteOwnPosts,DeleteOtherUsersPosts,PublishOwnPosts,PublishOtherUsersPosts,ViewPublicPages,ViewUnpublishedPages,CreateNewPages,EditOwnPages,EditOtherUsersPages,DeleteOwnPages,DeleteOtherUsersPages,PublishOwnPages,PublishOtherUsersPages,ViewRoleGroups,CreateRoleGroups,EditRoleGroups,DeleteRoleGroups,EditOwnRoleGroups,EditOtherUsersRoleGroups,CreateNewUsers,EditOwnUser,EditOtherUsers')



insert [dbo].[RoleGroups]
       ([Name],
        [AssignedRoles])
values ('Editor' ,
        'AccessAdminPages,ViewPublicPosts,ViewUnpublishedPosts,CreateNewPosts,EditOwnPosts,DeleteOwnPosts,PublishOwnPosts,EditOwnUser,ViewPublicComments,ViewUnmoderatedComments,CreateComments,ModerateComments,ViewPublicPages,ViewUnpublishedPages,CreateNewPages,EditOwnPages,DeleteOwnPages,PublishOwnPages' )



insert [dbo].[RoleGroups]
       ([Name],
        [AssignedRoles])
values ('Anonymous' ,
        'ViewPublicPosts,ViewPublicComments,CreateComments,ViewPublicPages' )



insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (1 ,
        'MinRequiredPasswordLength' ,
        '6' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (2 ,
        'PostsPerPage' ,
        '10' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (3 ,
        'BlogName' ,
        'Your Name' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (4 ,
        'BlogDescription' ,
        'BlogDescription' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (5 ,
        'Theme' ,
        'Default' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (6 ,
        'Utc Offset' ,
        'UTC' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (7 ,
        'Email To' ,
        'example@domain.com' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (8 ,
        'Subject Prefix' ,
        'Blog' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (9 ,
        'Smtp Address' ,
        'smtp.domain.com' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (10 ,
        'Smtp Port' ,
        '25' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (11 ,
        'Smtp UserName' ,
        'user' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (12 ,
        'Smtp Password' ,
        '' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (13 ,
        'Smtp SSL' ,
        'False' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (14 ,
        'Enable Comments' ,
        'True' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (15 ,
        'Moderate Comments' ,
        'True' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (16 ,
        'Enable Nested Comments' ,
        'True' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (17 ,
        'Close Comments After Days' ,
        null)


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (18 ,
        'Comments Per Page' ,
        '10' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (19 ,
        'Comments Avatar' ,
        null)


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (20 ,
        'Blog Culture' ,
        'auto' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (21 ,
        'Enable Facebook Like Button For Posts' ,
        'True' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (22 ,
        'Enable Google +1 Button For Posts' ,
        'True' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (23 ,
        'Enable Twitter Share Button For Posts' ,
        'True' )


insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (24 ,
        'Last Database Change' ,
        '0' )

insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (25 ,
        'Custom Code' ,
        '' )

insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (26 ,
        'Alternate Feed Url' ,
        '' )

insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (27 ,
        'Items Shown In Feed' ,
        '10' )

insert [dbo].[Settings]
       ([Id],
        [Description],
        [Value])
values (28 ,
        'Default Feed Output' ,
        'RSS' )

insert [dbo].[Users]
       ([Username],
        [Email],
        [Password],
        [DisplayName],
        [FullName],
        [Location],
        [CreationDate],
        [LastLoginDate],
        [About],
        [BirthDate],
        [Enabled])
values ('admin' ,
        'admin@example.com' ,
        'admin' ,
        null,
        null,
        null,
        GETUTCDATE(),
        null,
        null,
        null,
        1 )



insert [dbo].[Posts]
       ([Title],
        [Description],
        [Slug],
        [Content],
        [CreationDate],
        [LastModificationDate],
        [PublicationDate],
        [IsPublished],
        [IsCommentEnabled],
        [IsDeleted],
        [Author_Id])
values ('Example post. You blog is now installed' ,
        null,
        'example' ,
        'Example post' ,
        GETUTCDATE(),
        GETUTCDATE(),
        GETUTCDATE(),
        1 ,
        1 ,
        0 ,
        1 )



insert [dbo].[RoleGroupUsers]
       ([RoleGroup_Id],
        [User_Id])
values (1 ,
        1 )

COMMIT

END TRY
BEGIN CATCH
    ROLLBACK
END CATCH
