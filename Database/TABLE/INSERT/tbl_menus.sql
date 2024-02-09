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

