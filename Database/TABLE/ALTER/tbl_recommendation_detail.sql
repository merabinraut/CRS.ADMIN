alter table tbl_recommendation_detail
drop column AgentType

EXEC sp_RENAME 'tbl_recommendation_detail.AgentId' , 'ClubId', 'COLUMN'