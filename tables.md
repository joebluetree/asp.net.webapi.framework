# Table Script

#### userm 
```
CREATE TABLE userm(
	user_id int IDENTITY(1,1) NOT NULL,
	user_code varchar(60) NOT NULL,
	user_name varchar(60) NOT NULL,
	user_password varchar(60) NOT NULL,
	user_email varchar(60),
	user_is_locked char(1) NOT NULL,
	user_is_admin char(1) NOT NULL,
);

```

