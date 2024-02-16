UPDATE tbl_static_data SET AdditionalValue2 = 'Good', AdditionalValue3= N'良い' WHERE StaticDataType = 24 

update dbo.tbl_static_data set staticdatalabel = 'on site payment', staticdatavalue = 1, additionalvalue1 =n'現地で支払う' where staticdatatype = 10 and id = 49
update dbo.tbl_static_data set staticdatalabel = 'stripe payment', staticdatavalue = 2, additionalvalue1 =n'クレジットカード' where staticdatatype = 10 and id = 51