delete from
  userlogin
where
  userloginid not in (
    select
      max(userloginid)
    from
      userlogin
    GROUP BY
      loginid
   )
