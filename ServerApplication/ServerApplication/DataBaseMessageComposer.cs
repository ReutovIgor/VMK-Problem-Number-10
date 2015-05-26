using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ServerApplication
{
    static class DataBaseMessageComposer
    {
        private static DBWorker dbWorker;
        private static Dictionary<string, string> queryTemplates;

        static DataBaseMessageComposer()
        {
            dbWorker = new DBWorker();
            queryTemplates = new Dictionary<string, string>()
            {
                //Message Control Authorization check query
                {"check_authorization", "SELECT Id_<table> FROM <table> WHERE Id_Man IN (SELECT Id_Man FROM Man WHERE Username='<username>')"},

                #region Consultation Control queries
                {"get_departments", "SELECT Name, Address, Phone FROM Subsidiary"},
                {"get_specializations", "SELECT Spec FROM Specialization"},
                {"get_doctors", @"SELECT Man.Name, Man.Surname, Man.Patronymic, Specialization.spec 
                                  FROM Doctor INNER JOIN Specialization ON Doctor.Id_Specialization=Specialization.Id_Specialization 
                                  INNER JOIN Man ON Doctor.Id_Man=Man.Id_Man 
                                  WHERE Doctor.Id_Subsidiary=(SELECT Id_Subsidiary FROM Subsidiary WHERE Name=N'<subsidiary>')"},
                {"get_time_status", "IF NOT EXISTS(SELECT Id_Consultation FROM Consultation WHERE StartTime='<time>') SELECT 'Time is free' ELSE SELECT 'Consultation Exists'"},
                {"create_consultation", @"IF NOT EXISTS(SELECT Id_Consultation FROM Consultation WHERE StartTime='<time>') 
                                            INSERT INTO Consultation (Id_Patient, Id_Doctor, StartTime, Code, Status, PatientText) Output Inserted.Id_Consultation 
                                            VALUES ((SELECT Id_Patient FROM Patient 
                                                     WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>')), 
                                                    (SELECT Id_Doctor FROM Doctor 
                                                        WHERE Id_Man=(SELECT Id_Man FROM Man 
                                                                           WHERE Name=N'<name>' 
                                                                           AND Surname=N'<surname>' 
                                                                           AND Patronymic=N'<patronymic>')
                                                        AND Id_Specialization=(SELECT Id_Specialization FROM Specialization WHERE Spec=N'<specialization'))
                                                    ,'<time>', '<code>', '<status>', N'<text>' ) 
                                          ELSE SELECT 'CONSULTATION EXISTS'"},
                {"get_consultations", @"SELECT * FROM Consultation 
                                            WHERE Status='<status>' 
                                            AND (Id_Patient=(SELECT Id_Patient FROM Patient 
                                                                WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>')) 
                                                                OR Id_doctor=(SELECT Id_Doctor FROM Doctor WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>')))"},
                {"add_note", @"IF EXISTS (SELECT Id_Consultation FROM Consultation 
                                            WHERE Id_Doctor=(SELECT Id_Doctor FROM Doctor 
                                                                WHERE Id_Man=(SELECT Id_Man FROM Man 
                                                                                WHERE Username='<username>')))
                                        BEGIN 
                                        UPDATE Consultation SET DoctorText='<text>' WHERE Id_Consultation='<consultationId>'
                                        SELECT 'Note Added'
                                        END
                               ELSE SELECT 'No rights'"},
                {"close_consultation", "UPDATE Consultation SET EndTime='<time>',Status='<status>' WHERE Id_Consultation='<consultationId>'"},
                {"cancel_consultation", "UPDATE Consultation SET Status='<status>' WHERE Id_Consultation='<consultationId>'"},

                {"send_message", @"DECLARE @dialogID INT 
                            SET @dialogID = null
                            SELECT @dialogID=Id_Dialog FROM Dialogs
			                            WHERE Id_Man1=(SELECT Id_Man FROM Man WHERE Username='<username>' OR (Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>')) 
			                              AND Id_Man2=(SELECT Id_Man FROM Man WHERE Username='<username>' OR (Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>'))
                            IF @dialogID is not null
	                            BEGIN 
	                            INSERT INTO Messages (Id_Dialog, Author, Text, Time) Values (@dialogID, (SELECT  Surname + ' ' +NAME + ' '+ Patronymic AS author FROM Man WHERE Username='<username>'), '<text>', '<time>')
	                            END
                            ELSE
	                            BEGIN 
	                            SELECT 'DOES NOT EXIST'
	                            DECLARE @dID TABLE(ID INT)
	                            INSERT INTO Dialogs (Id_Man1, Id_Man2, Dialog_Type) OUTPUT inserted.Id_Dialog INTO @dID(ID) VALUES((SELECT Id_Man FROM Man WHERE Username='<username>'), (SELECT Id_Man FROM Man WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>'), '<type>')
	                            INSERT INTO Messages (Id_Dialog, Author, Text, Time) VALUES ((SELECT ID FROM @dID), (SELECT  Surname + ' ' +NAME + ' '+ Patronymic AS author FROM Man WHERE Username='<username>'), '<text>', '<time>')
	                            END"},

                {"get_messages", @"SELECT * FROM Messages WHERE Id_Dialog=(SELECT Id_Dialog FROM Dialogs WHERE (Id_Man1=(SELECT Id_Man FROM Man WHERE Username='<username>') 
																	                                        OR Id_Man2=(SELECT Id_Man FROM Man WHERE Username='<username>')) 
                                                                                                            AND Dialog_Type='<type>') "},
                #endregion

                #region User Control queries
                {"register_user", @"IF NOT EXISTS(SELECT Id_Man FROM Man WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>')
	                                    BEGIN
		                                    INSERT INTO Man (Name, Surname, Patronymic, DateOfBirth, Username, Password, Approved) 
                                            Output Inserted.Id_Man
		                                    VALUES (N'<name>', N'<surname>', N'<patronymic>', '<date>', '<username>', '<password>', '0')
	                                    END
                                    ELSE
	                                    BEGIN
		                                    DECLARE @number TABLE (num INT)
		                                    INSERT INTO @number VALUES (1);
		                                    WHILE(SELECT Id_Man FROM Man WHERE Name=N'<name>' AND Surname=N'<surname>'+ Convert(nvarchar(20),(SELECT num FROM @number)) AND Patronymic=N'<patronymic>') is not null
		                                    BEGIN
			                                    UPDATE @number SET num = num + 1
		                                    END
		                                    INSERT INTO Man (Name, Surname, Patronymic, DateOfBirth, Username, Password, Approved)
                                            Output Inserted.Id_Man 
		                                    VALUES (N'<name>', N'<surname>' + Convert(nvarchar(20),(SELECT num FROM @number)), N'<patronymic>', '<date>', '<username>', '<password>', '0')
	                                    END"},
                {"get_pendings", @"SELECT Name, Surname, Patronymic FROM Man WHERE Approved=0"},
                {"approve_user", @"UPDATE Man SET Approved=1 WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>')
                                   INSERT INTO Patient (Id_Man) Output Inserted.Id_Patient VALUES ((SELECT Id_Man FROM Man WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>'))"},
                {"get_users", @"SELECT Name, Surname, Patronymic, 'Doctor' AS Type FROM Man INNER JOIN Doctor ON Man.Id_Man=Doctor.Id_Man
                                UNION
                                SELECT Name, Surname, Patronymic, 'Patient' AS Type FROM Man INNER JOIN Patient ON Man.Id_Man=Patient.Id_Man AND Man.Id_Man NOT IN (SELECT Id_Man FROM Doctor)"},
                {"add_rights_doctor", @"Declare @tableID TABLE (ID INT)
                                        INSERT INTO TimeTable (Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday) 
                                        Output Inserted.Id_TimeTable INTO @tableID(ID)
                                        VALUES ('<monday>', '<tuesday>', '<wednesday>', '<thursday>', '<friday>', '<saturday>', '<sunday>')
                                        INSERT INTO Doctor (Id_Man, Id_Subsidiary, Id_Specialization, Id_Manager, Id_TimeTable)
                                        Output Inserted.Id_Doctor
                                        VALUES ((SELECT Id_Man FROM Man WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>'), 
                                                (SELECT Id_Subsidiary FROM Subsidiary WHERE Name=N'<subsidiary>'), 
                                                (SELECT Id_Specialization FROM Specialization WHERE spec=N'<specialization>'), 
                                                (SELECT Id_Manager FROM Manager WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>')),
                                                (SELECT ID FROM @tableID))"},
                {"add_rights_manager", "INSERT INTO Manager (Id_Man) Output Inserted.Id_Manager VALUES ((SELECT Id_Man FROM Man WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>'))"},
                {"get_schedule", "SELECT * FROM TimeTable WHERE Id_Timetable=(SELECT Id_TimeTable FROM Doctor WHERE Id_Doctor=(SELECT Id_Doctor FROM Doctor WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>')))"},
                {"change_schedule", @"UPDATE Timetable SET <update> WHERE Id_Timetable=(SELECT Id_TimeTable FROM Doctor WHERE Id_Doctor=(SELECT Id_Doctor FROM Doctor WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>')))
                                      SELECT 'TimeTable updated'"},
                {"vacation_planning", @"Declare @timeTableID TABLE (ID INT)
                                                INSERT INTO @timeTableID(ID) SELECT Id_TimeTable FROM Doctor WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>')
                                                IF (SELECT Vacation FROM TimeTable WHERE Id_TimeTable=(SELECT ID FROM @timeTableID)) is null
	                                                BEGIN 
		                                                UPDATE TimeTable SET Vacation='<vacation>'
		                                                DECLARE @dialogID TABLE (ID INT)
		                                                INSERT INTO @dialogID(ID) SELECT Id_Dialog FROM Dialogs WHERE Id_Man1=(SELECT Id_Man FROM Man WHERE Username='<username>')
																                                                AND	  Id_Man2=(SELECT Id_Man FROM Manager WHERE Id_Manager=(SELECT Id_Manager FROM Doctor WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>')))
		                                                IF (SELECT ID FROM @dialogID) is null
			                                                BEGIN
			                                                UPDATE @dialogID SET ID = null
			                                                INSERT INTO Dialogs 
			                                                OUTPUT Inserted.Id_Dialog INTO @dialogID(ID)
			                                                VALUES ((SELECT Id_Man FROM Man WHERE Username='<username>'), 
					                                                (SELECT Id_Man FROM Manager WHERE Id_Manager=(SELECT Id_Manager FROM Doctor WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>'))),
					                                                2)
			                                                END
		                                                INSERT INTO Messages 
		                                                VALUES ((SELECT ID FROM @dialogID),
				                                                (SELECT NAME + ' ' + Surname + ' ' + Patronymic AS author FROM Man WHERE Username='<username>'),
				                                                N'<text>',
				                                                '<time>')
		                                                SELECT 'Vacation plan is added'
	                                                END"},
                {"approve_plan", @"DECLARE @dialogID TABLE (ID INT)
                                   INSERT INTO @dialogID(ID) SELECT Id_Dialog FROM Dialogs WHERE Id_Man1=(SELECT Id_Man FROM MAN WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>')
													                                    AND	  Id_Man2=(SELECT Id_Man FROM Man WHERE Username='<username>')
                                   INSERT INTO Messages 
                                   VALUES ((SELECT ID FROM @dialogID),
		                                   (SELECT  Surname + ' ' +NAME + ' '+ Patronymic AS author FROM Man WHERE Username='<username>'),
		                                   N'<text>',
		                                   '<time>')
                                   SELECT 'Approval message was added'"},
                {"reject_plan", @"Declare @timeTableID INT
                                  SELECT @timeTableID = Id_TimeTable FROM Doctor WHERE Id_Man=(SELECT Id_Man FROM Manager WHERE Id_Manager=(SELECT Id_Manager FROM Doctor WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>')))
                                  IF @timeTableID is not null
	                                  BEGIN
		                                  IF (SELECT Vacation FROM TimeTable WHERE Id_TimeTable=@timeTableID) is not null
		                                  BEGIN
			                                  UPDATE TimeTable SET Vacation=NULL WHERE Id_TimeTable=@timeTableID
			                                  DECLARE @dialogID TABLE (ID INT)
			                                  INSERT INTO @dialogID(ID) SELECT Id_Dialog FROM Dialogs WHERE Id_Man1=(SELECT Id_Man FROM MAN WHERE Name=N'<name>' AND Surname=N'<surname>' AND Patronymic=N'<patronymic>')
															                                    AND	  Id_Man2=(SELECT Id_Man FROM Man WHERE Username='<username>')
			                                  INSERT INTO Messages 
			                                  VALUES ((SELECT ID FROM @dialogID),
					                                  (SELECT  Surname + ' ' +NAME + ' '+ Patronymic AS author FROM Man WHERE Username='<username>'),
					                                  N'<text>',
					                                  '<time>')
			                                  SELECT 'Reject message was added'
		                                  END
	                                  END"},
                {"login", @"IF (SELECT Id_Man FROM Man WHERE Username='<username>' AND Password='<password>') is not null
	                            BEGIN
		                            DECLARE @response TABLE (PatientAuth BIT, DoctorAuth BIT, ManagerAuth BIT)
		                            INSERT INTO @response VALUES (0, 0, 0)
		                            IF EXISTS (SELECT Id_Manager FROM Manager WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>' AND Password='<password>'))
			                            UPDATE @response SET ManagerAuth=1
		                            IF EXISTS (SELECT Id_Doctor FROM Doctor WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>' AND Password='<password>'))
			                            UPDATE @response SET DoctorAuth=1
		                            IF EXISTS (SELECT Id_Patient FROM Patient WHERE Id_Man=(SELECT Id_Man FROM Man WHERE Username='<username>' AND Password='<password>'))
			                            UPDATE @response SET PatientAuth=1
		                            SELECT * FROM @response
	                            END
                            ELSE
	                            BEGIN
		                            SELECT 'Username or Password are incorrrect'
	                            END"}
                #endregion

            };
        }

        public static Dictionary<string, dynamic> SendRequest(string request, Dictionary<string, dynamic> data)
        {
            string query = queryTemplates[request];
            if (data != null)
            {
                foreach (var pair in data)
                {
                    query = query.Replace("<" + pair.Key + ">", pair.Value);
                }
            }
            return dbWorker.SendQuery(query);
        }
    }
}
