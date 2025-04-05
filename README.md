How to build and start the application
----
1. Rename the .env-example to .env and modify according to your needs
    -> important is at least the ip address of the server
2. Get ssl certificates and place them in a folder named ssl in the ClientApp (./ClientApp/ssl/my-site.crt && ./ClientApp/ssl/my-site.key) -> the file names must be exactly the same.
3. run docker compose up --build

How to run the app in dev mode
-----
- ```dotnet restore```
- ```dotnet build```
- ```dotnet ef database update``` This requires the dotnet-ef package to be installed:
- ```dotnet tool install --global dotnet-ef --version <your_dotnet_version>```

How to add and update SupportTasks
-----
- Create admin user (registering as user with username as set in `.env` file)
- Obtain the auth-token e.g.:
```curl -k POST https://localhost:8080/auth/login \
-H "Content-Type: application/json" \
-d '{
  "UserName": <ADMIN_USER_NAME>, 
  "password":<admin password>
}'
```
- Run the actual query e.g.:
```
curl -k POST https://localhost:8080/supporttask/addnew \
-H "Content-Type: application/json" \
-H "Authorization: Bearer <your bearer key obtained earlier>" \ 
-d '{              
  "Title": "Sample Task",
  "Description": "This is a sample task description",
  "Duration": "120",
  "RequiredSupporters": 5
}'
```

How to access the application
-----
The application will be accessible from https://<IP_ADDRESS>:<PROXY_PORT_SSL>, according to the values set in the .env
