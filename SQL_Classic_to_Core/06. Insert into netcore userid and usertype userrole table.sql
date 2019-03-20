/*
Insert superuser into Role Table
*/

INSERT INTO UserRole 
(
    UserId,
    RoleId
)
SELECT b.UserLoginId,
       1
FROM Users a
INNER JOIN dbo.UserLogin AS b ON b.userid = a.userid
WHERE UserType = 'superuser';

/*
Insert admin into Role Table
*/

INSERT INTO UserRole
(
    UserId,
    RoleId
)
SELECT b.UserLoginId,
       2
FROM Users a
INNER JOIN dbo.UserLogin AS b ON b.userid = a.userid
WHERE UserType = 'admin';

/*
Insert attorney into Role Table
*/

INSERT INTO UserRole
(
    UserId,
    RoleId
)
SELECT b.UserLoginId,
       3
FROM Users a
INNER JOIN dbo.UserLogin AS b ON b.userid = a.userid
WHERE UserType = 'attorney';

/*
Insert paralegal into Role Table
*/

INSERT INTO UserRole
(
    UserId,
    RoleId
)
SELECT b.UserLoginId,
       4
FROM Users a
INNER JOIN dbo.UserLogin AS b ON b.userid = a.userid
WHERE UserType = 'paralegal';

/*
Insert FN into Role Table
*/

INSERT INTO UserRole
(
    UserId,
    RoleId
)
SELECT b.UserLoginId,
      4
FROM Users a
INNER JOIN dbo.UserLogin AS b ON b.userid = a.userid
WHERE UserType = 'contact';

/*
Insert Corp user into Role Table
*/

INSERT INTO UserRole
(
    UserId,
    RoleId
)
SELECT b.UserLoginId,
       5
FROM Users a
INNER JOIN dbo.UserLogin AS b ON b.userid = a.userid
WHERE UserType = 'Corpuser';




