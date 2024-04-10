UPDATE tbl_static_data SET AdditionalValue2 = 'Good', AdditionalValue3= N'良い' WHERE StaticDataType = 24 

update dbo.tbl_static_data set staticdatalabel = 'on site payment', staticdatavalue = 1, additionalvalue1 =n'現地で支払う' where staticdatatype = 10 and id = 49
update dbo.tbl_static_data set staticdatalabel = 'stripe payment', staticdatavalue = 2, additionalvalue1 =n'クレジットカード' where staticdatatype = 10 and id = 51


	update tbl_static_data set StaticDataLabel='On site payment' ,StaticDataValue='1', AdditionalValue1=N'現地で支払う' where id =49
				update tbl_static_data set StaticDataLabel='Stripe payment' ,StaticDataValue='2', AdditionalValue1=N'クレジットカード' where id =51

update tbl_static_data set additionalvalue1=N'個人' where Id = 115 and StaticDataType = 16
update tbl_static_data set additionalvalue1=N'法人' where Id = 201 and StaticDataType = 16