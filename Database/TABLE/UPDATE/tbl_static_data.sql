UPDATE tbl_static_data SET AdditionalValue2 = 'Good', AdditionalValue3= N'良い' WHERE StaticDataType = 24 

update dbo.tbl_static_data set staticdatalabel = 'on site payment', staticdatavalue = 1, additionalvalue1 =n'現地で支払う' where staticdatatype = 10 and id = 49
update dbo.tbl_static_data set staticdatalabel = 'stripe payment', staticdatavalue = 2, additionalvalue1 =n'クレジットカード' where staticdatatype = 10 and id = 51

UPDATE dbo.tbl_static_data SET AdditionalValue1 = '161 AND 165' WHERE StaticDataType = 20 AND StaticDataValue = 1
UPDATE dbo.tbl_static_data SET AdditionalValue1 = '166 AND 170' WHERE StaticDataType = 20 AND StaticDataValue = 2
UPDATE dbo.tbl_static_data SET AdditionalValue1 = '171 AND 175' WHERE StaticDataType = 20 AND StaticDataValue = 3
UPDATE dbo.tbl_static_data SET AdditionalValue1 = '176 AND 180' WHERE StaticDataType = 20 AND StaticDataValue = 4
UPDATE dbo.tbl_static_data SET AdditionalValue1 = '181 AND 185' WHERE StaticDataType = 20 AND StaticDataValue = 5
