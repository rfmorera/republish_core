/*
Cleanup deprecated users
*/

DELETE FROM dbo.Users
where UserType NOT IN ('contact', 'attorney', 'admin', 'paralegal', 'CorpUser', 'employer', 'superuser')


/*
Cleanup orphaned userlogins
*/

DELETE FROM a
FROM dbo.UserLogin a
LEFT JOIN users b ON b.UserId = a.UserId
WHERE b.userid IS NULL 