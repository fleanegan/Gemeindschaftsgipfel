How to build and start the application
----
1. Rename the .env-example to .env and modify according to your needs
    -> important is at least the ip address of the server
2. Get ssl certificates and place them in a folder named ssl in the ClientApp (./ClientApp/ssl/my-site.crt && ./ClientApp/ssl/my-site.key) -> the file names must be exactly the same.
3. run chmod +x ./startup.sh && ./startup.sh in order to run the application


How to access the application
-----
The application will be accessible from https://<IP_ADDRESS>:<PROXY_PORT_SSL>, according to the values set in the .env
