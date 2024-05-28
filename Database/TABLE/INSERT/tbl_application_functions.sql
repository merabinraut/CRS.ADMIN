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



INSERT INTO tbl_application_functions ([FunctionId], [MenuId], [FunctionName], [FunctionURL], [Status], [ActionUser], [ActionIP], [ActionDate])
VALUES
(  144, 14, 'Event Management', '/ClubManagement/EventList', 'A', 'kiran.acharya', '::1', N'2024-01-22T15:53:22.727' )


INSERT INTO tbl_application_functions ([FunctionId], [MenuId], [FunctionName], [FunctionURL], [Status], [ActionUser], [ActionIP], [ActionDate])
VALUES
(  145, 14, 'Manage Event', '/ClubManagement/ManageEvent', 'A', 'kiran.acharya', '::1', N'2024-01-22T15:53:22.727' )


INSERT INTO tbl_application_functions ([FunctionId], [MenuId], [FunctionName], [FunctionURL], [Status], [ActionUser], [ActionIP], [ActionDate])
VALUES
(  146, 14, 'Delete Event', '/ClubManagement/DeleteEvent', 'A', 'kiran.acharya', '::1', N'2024-01-22T15:53:22.727' )


INSERT INTO tbl_application_functions ([FunctionId], [MenuId], [FunctionName], [FunctionURL], [Status], [ActionUser], [ActionIP], [ActionDate])
VALUES
(  147, 33, 'Bookmark Management', '/BookmarkManagement/Index', 'A', 'rabin.raut', '::1', GETDATE() ),
(  148, 33, 'Bookmark Management', '/BookmarkManagement/ManageBookmark', 'A', 'rabin.raut', '::1', GETDATE() )

INSERT INTO tbl_application_functions ([FunctionId], [MenuId], [FunctionName], [FunctionURL], [Status], [ActionUser], [ActionIP], [ActionDate])
VALUES
(  149, 34, 'View Notifications', '/NotificationManagement/ViewAllNotifications', 'A', 'rabin.raut', '::1', GETDATE() ),
(  150, 34, 'Has UnRead Notification', '/NotificationManagement/HasUnReadNotification', 'A', 'rabin.raut', '::1', GETDATE() ),
(  151, 34, 'Manage Notification Status', '/NotificationManagement/ManageNotificationReadStatus', 'A', 'rabin.raut', '::1', GETDATE() )


INSERT INTO tbl_application_functions ([FunctionId], [MenuId], [FunctionName], [FunctionURL], [Status], [ActionUser], [ActionIP], [ActionDate])
VALUES
(  152, 35, 'Profile Detail', '/ProfileManagement/Index', 'A', 'rabin.raut', '::1', GETDATE() ),
(  153, 35, 'Manage Profile', '/ProfileManagement/UpdateUserProfileDetail', 'A', 'rabin.raut', '::1', GETDATE() ),
(  154, 35, 'Change Password', '/ProfileManagement/ChangePasswordV2', 'A', 'rabin.raut', '::1', GETDATE() ),
(  155, 35, 'Change Profile Image', '/ProfileManagement/ChangeProfileImage', 'A', 'rabin.raut', '::1', GETDATE() ),
(  156, 35, 'Manage Customer Account', '/ProfileManagement/DeleteCustomereAccount', 'A', 'rabin.raut', '::1', GETDATE() )

INSERT INTO tbl_application_functions ([FunctionId], [MenuId], [FunctionName], [FunctionURL], [Status], [ActionUser], [ActionIP], [ActionDate])
VALUES
(  157, 36, 'Initiate Club Reservation', '/ReservationManagementV2/InitiateClubReservationProcess', 'A', 'rabin.raut', '::1', GETDATE() ),
(  158, 36, 'Club Plan', '/ReservationManagementV2/Plan', 'A', 'rabin.raut', '::1', GETDATE() ),
(  159, 36, 'Club Host', '/ReservationManagementV2/Host', 'A', 'rabin.raut', '::1', GETDATE() ),
(  160, 36, 'Reservation Confirmation', '/ReservationManagementV2/Confirmation', 'A', 'rabin.raut', '::1', GETDATE() ),
(  161, 36, 'Reservation Billing', '/ReservationManagementV2/Billing', 'A', 'rabin.raut', '::1', GETDATE() ),
(  162, 36, 'Club Reservation Confirmation', '/ReservationManagementV2/ReservationConfirmation', 'A', 'rabin.raut', '::1', GETDATE() )

INSERT INTO tbl_application_functions ([FunctionId], [MenuId], [FunctionName], [FunctionURL], [Status], [ActionUser], [ActionIP], [ActionDate])
VALUES
(  163, 37, 'Reservation History', '/ReservationHistoryManagementV2/ReservationHistory', 'A', 'rabin.raut', '::1', GETDATE() ),
(  164, 37, 'Reservation History Detail', '/ReservationHistoryManagementV2/ViewHistoryDetail', 'A', 'rabin.raut', '::1', GETDATE() ),
(  165, 37, 'Reschedule Reservation', '/ReservationHistoryManagementV2/RescheduleReservation', 'A', 'rabin.raut', '::1', GETDATE() ),
(  166, 37, 'Cancel Reservation', '/ReservationHistoryManagementV2/CancelReservation', 'A', 'rabin.raut', '::1', GETDATE() ),
(  167, 37, 'Redo Reservation', '/ReservationHistoryManagementV2/RedoReservation', 'A', 'rabin.raut', '::1', GETDATE() ),
(  168, 37, 'Delete Reservation', '/ReservationHistoryManagementV2/DeleteReservation', 'A', 'rabin.raut', '::1', GETDATE() )


INSERT INTO tbl_application_functions ([FunctionId], [MenuId], [FunctionName], [FunctionURL], [Status], [ActionUser], [ActionIP], [ActionDate])
VALUES
(  169, 38, 'Review List', '/ReviewManagement/ReviewList', 'A', 'rabin.raut', '::1', GETDATE() ),
(  170, 38, 'Review', '/ReviewManagement/Review', 'A', 'rabin.raut', '::1', GETDATE() ),
(  171, 38, 'Review 2', '/ReviewManagement/Review2', 'A', 'rabin.raut', '::1', GETDATE() ),
(  172, 38, 'Review 3', '/ReviewManagement/Review3', 'A', 'rabin.raut', '::1', GETDATE() ),
(  173, 38, 'Review 4', '/ReviewManagement/Review4', 'A', 'rabin.raut', '::1', GETDATE() ),
(  174, 38, 'Review Details', '/ReviewManagement/ReviewDetails', 'A', 'rabin.raut', '::1', GETDATE() )


INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '147', '14', 'Manage Manager', '/ClubManagement/ManageManager', 'A', 'kiran.acharya', '::1', GETDATE()
)

INSERT INTO dbo.tbl_application_functions (FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate, AdditionalValue)
VALUES (150, 39, 'Point Setup User Type List', '/PointSetup/PointSetupUserTypeList', 'A', NULL, NULL, NULL, NULL)
GO
INSERT INTO dbo.tbl_application_functions (FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate, AdditionalValue)
VALUES (151, 39, 'Points Category List', '/PointSetup/PointsCategoryList', 'A', NULL, NULL, NULL, NULL)
GO

INSERT INTO dbo.tbl_application_functions (FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate, AdditionalValue)
VALUES (152, 39, 'Manage Category', '/PointSetup/ManageCategory', 'A', NULL, NULL, NULL, NULL)
GO

INSERT INTO dbo.tbl_application_functions (FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate, AdditionalValue)
VALUES (153, 39, 'Points Category Slab List', '/PointSetup/PointsCategorySlabList', 'A', NULL, NULL, NULL, NULL)
GO

INSERT INTO dbo.tbl_application_functions (FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate, AdditionalValue)
VALUES (154, 39, 'Manage Category Points Slab', '/PointSetup/ManageCategoryPointsSlab', 'A', NULL, NULL, NULL, NULL)
GO
INSERT INTO dbo.tbl_application_functions (FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate, AdditionalValue)
VALUES (155, 39, 'Block unblock Category', '/PointSetup/BlockUnblockCategory', 'A', NULL, NULL, NULL, NULL)
GO

INSERT INTO dbo.tbl_application_functions (FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate, AdditionalValue)
VALUES (156, 39, 'Delete Category Points', '/PointSetup/DeleteCategoryPointsSlab', 'A', NULL, NULL, NULL, NULL)
GO

INSERT INTO dbo.tbl_application_functions (FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate, AdditionalValue)
VALUES (157, 39, 'Assing Points Category', '/PointSetup/AssingPointsCategory', 'A', NULL, NULL, NULL, NULL)
GO


INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '147', '14', 'Manage Manager', '/ClubManagement/ManageManager', 'A', 'kiran.acharya', '::1', GETDATE()
)
INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '148', '14', 'Approve Reject Club', '/ClubManagement/ApproveRejectClub', 'A', 'kiran.acharya', '::1', GETDATE()
)

INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '149', '14', 'Manage Pending Club', '/ClubManagement/ManagePendingClub', 'A', 'kiran.acharya', '::1', GETDATE()
)

INSERT INTO tbl_application_functions 
(FunctionId, MenuId, FunctionName, FunctionURL, Status, ActionUser, ActionIP, ActionDate)
VALUES
(
 '150', '14', 'Manage Club Plan', '/ClubPlanManagement/ClubPlanList', 'A', 'kiran.acharya', '::1', GETDATE()
)