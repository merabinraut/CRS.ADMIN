use CRS

INSERT INTO tbl_menus(MenuId
,MenuName
,MenuUrl
,MenuGroup
,ParentGroup
,CssClass
,Status
,MenuAccessCategory
,ActionUser
,ActionIP
,ActionDate)
values
(
'1',
'Dashboard',
'/',
'',
'',
'fas fa-tachometer-alt',
'A',
'Admin',
'rabin.raut',
'::1',
GETDATE()
)

INSERT INTO tbl_menus(MenuId
,MenuName
,MenuUrl
,MenuGroup
,ParentGroup
,CssClass
,Status
,MenuAccessCategory
,ActionUser
,ActionIP
,ActionDate)
values
(
'3',
'Profile',
'/ProfileManagement/Index',
'',
'',
'fas fa-tachometer-alt',
'A',
'Admin',
'rabin.raut',
'::1',
GETDATE()
)

INSERT INTO tbl_menus(MenuId
,MenuName
,MenuUrl
,MenuGroup
,ParentGroup
,CssClass
,Status
,MenuAccessCategory
,ActionUser
,ActionIP
,ActionDate)
values
(
'5',
'Roles and Privileges',
'/RoleManagement/RoleList',
'',
'Admin Management',
'fa fa-universal-access',
'A',
'Admin',
'rabin.raut',
'::1',
GETDATE()
)


INSERT INTO tbl_menus(MenuId
,MenuName
,MenuUrl
,MenuGroup
,ParentGroup
,CssClass
,Status
,MenuAccessCategory
,ActionUser
,ActionIP
,ActionDate)
values
(
'7',
'Assign Roles',
'/RoleManagement/RoleList',
'',
'Admin Management',
'fa fa-universal-access',
'A',
'Admin',
'rabin.raut',
'::1',
GETDATE()
)

INSERT INTO tbl_menus(MenuId
,MenuName
,MenuUrl
,MenuGroup
,ParentGroup
,CssClass
,Status
,MenuAccessCategory
,ActionUser
,ActionIP
,ActionDate)
values
(
'8',
'Commission Setup',
'/CommissionManagement/CategoryList',
'',
'Commission Management',
'fa fa-universal-access',
'A',
'Admin',
'rabin.raut',
'::1',
GETDATE()
)

INSERT INTO tbl_menus(MenuId
,MenuName
,MenuUrl
,MenuGroup
,ParentGroup
,CssClass
,Status
,MenuAccessCategory
,ActionUser
,ActionIP
,ActionDate)
values
(
'9',
'Assign Commission',
'/CommissionManagement/AssignCommission',
'',
'Commission Management',
'fa fa-universal-access',
'A',
'Admin',
'rabin.raut',
'::1',
GETDATE()
)

INSERT INTO tbl_menus(MenuId
,MenuName
,MenuUrl
,MenuGroup
,ParentGroup
,CssClass
,Status
,MenuAccessCategory
,ActionUser
,ActionIP
,ActionDate)
values
(
'10',
'Assign Commission',
'/CustomerManagement/CustomerList',
'',
'',
'fa fa-universal-access',
'A',
'Admin',
'rabin.raut',
'::1',
GETDATE()
)

INSERT INTO tbl_menus(MenuId
,MenuName
,MenuUrl
,MenuGroup
,ParentGroup
,CssClass
,Status
,MenuAccessCategory
,ActionUser
,ActionIP
,ActionDate)
values
(
'10',
'Club Management',
'/ClubManagement/ClubList',
'',
'',
'fa fa-universal-access',
'A',
'Admin',
'rabin.raut',
'::1',
GETDATE()
)

insert into tbl_menus 
(MenuId
,MenuName
,MenuUrl
,Status
,MenuAccessCategory
,ActionUser
,ActionIP
,ActionDate)

values(29, 'SMS Log', '/SMSLog/Index', 'A', 'Admin', 'rabin.raut', '::1', getdate()),
(30, 'Email Log', '/EmailLog/Index', 'A', 'Admin', 'rabin.raut', '::1', getdate())


INSERT INTO tbl_menus
(
    MenuId,
    MenuName,
    MenuUrl,
    MenuGroup,
    ParentGroup,
    CssClass,
    Status,
    MenuAccessCategory,
    ActionUser,
    ActionIP,
    ActionDate
)
VALUES
('33', 'Bookmark Management', '/BookmarkManagement/Index', '', '', '', 'A', 'Customer', 'rabin.raut', '::1', GETDATE());


INSERT INTO tbl_menus
(
    MenuId,
    MenuName,
    MenuUrl,
    MenuGroup,
    ParentGroup,
    CssClass,
    Status,
    MenuAccessCategory,
    ActionUser,
    ActionIP,
    ActionDate
)
VALUES
('34', 'Notification Management', '/NotificationManagement/ViewAllNotifications', '', '', '', 'A', 'Customer', 'rabin.raut', '::1', GETDATE());

INSERT INTO tbl_menus
(
    MenuId,
    MenuName,
    MenuUrl,
    MenuGroup,
    ParentGroup,
    CssClass,
    Status,
    MenuAccessCategory,
    ActionUser,
    ActionIP,
    ActionDate
)
VALUES
('35', 'Profile Management', '/ProfileManagement/Index', '', '', '', 'A', 'Customer', 'rabin.raut', '::1', GETDATE());

INSERT INTO tbl_menus
(
    MenuId,
    MenuName,
    MenuUrl,
    MenuGroup,
    ParentGroup,
    CssClass,
    Status,
    MenuAccessCategory,
    ActionUser,
    ActionIP,
    ActionDate
)
VALUES
('36', 'Reservation Management', '/ReservationManagementV2/Index', '', '', '', 'A', 'Customer', 'rabin.raut', '::1', GETDATE());


INSERT INTO tbl_menus
(
    MenuId,
    MenuName,
    MenuUrl,
    MenuGroup,
    ParentGroup,
    CssClass,
    Status,
    MenuAccessCategory,
    ActionUser,
    ActionIP,
    ActionDate
)
VALUES
('37', 'Reservation History Management', '/ReservationHistoryManagementV2/ReservationHistory', '', '', '', 'A', 'Customer', 'rabin.raut', '::1', GETDATE());

INSERT INTO tbl_menus
(
    MenuId,
    MenuName,
    MenuUrl,
    MenuGroup,
    ParentGroup,
    CssClass,
    Status,
    MenuAccessCategory,
    ActionUser,
    ActionIP,
    ActionDate
)
VALUES
('38', 'Review Management', '/ReviewManagement/ReviewList', '', '', '', 'A', 'Customer', 'rabin.raut', '::1', GETDATE());


INSERT INTO dbo.tbl_menus ( MenuId,MenuName, MenuUrl, MenuGroup, ParentGroup, MenuOrderPosition, GroupOrderPosition, CssClass, Status, MenuAccessCategory, ActionUser, ActionIP, ActionDate)
VALUES ( 39,'Points Setup', '/PointSetup/PointSetupUserTypeList', NULL, NULL, NULL, NULL, NULL, 'A', 'Admin', 'kiran.acharya', NULL, GETDATE())
GO