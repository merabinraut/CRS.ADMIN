INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '1', '1', 'Dashboard', '/Home/Dashboard', 'A', 'rabin.raut', '::1', GETDATE()
)

--Select * from tbl_menus where menuId = '3'
INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '3', '3', 'Profile', '/ProfileManagement/Index', 'A', 'rabin.raut', '::1', GETDATE()
)

INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '5', '3', 'Change Password', '/ProfileManagement/ChangePassword', 'A', 'rabin.raut', '::1', GETDATE()
)
--Select * from tbl_menus where menuId = '3'
INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '6', '3', 'Manage Profile Image', '/ProfileManagement/ChangeProfileImage', 'A', 'rabin.raut', '::1', GETDATE()
)

--Select * from tbl_menus where menuId = '5'
INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '8', '5', 'Role List', '/RoleManagement/RoleList', 'A', 'rabin.raut', '::1', GETDATE()
),
('10', '5', 'Role Type List', '/RoleManagement/RoleTypeList', 'A', 'rabin.raut', '::1', GETDATE()),
('12', '5', 'Manage Role Type', '/RoleManagement/AddRoleType', 'A', 'rabin.raut', '::1', GETDATE()),
('14', '5', 'Manage Menus', '/RoleManagement/Menus', 'A', 'rabin.raut', '::1', GETDATE()),
('16', '5', 'Manage Functions', '/RoleManagement/Functions', 'A', 'rabin.raut', '::1', GETDATE())