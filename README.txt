This solution uses StreamInsight, so you'll need to install StreamInsight for this to work.  We test the code using StreamInsight
version 2.1 on-prem running in embedded server.  To run the SI client, you'll need to update the instance_name appsetting in the
app.config file to point to your local StreamInsight instance.

This solution uses WebSocket to send data to the real-time dashboard.  To get WebSocket to work, you need to run the 
StreamInsight application on Windows 8 or Server 2012 with WebSocket enabled. Read the following blog post to understand
how to enable WebSockets
	- http://www.paulbatum.com/2011/09/getting-started-with-websockets-in.html
To get the SI application to properly send data to the real-time dashboard, do the following:
	- Update the ws_url appsettings in Twitter.Client\app.config file with the IP address of the machine 
	  running the SI client application.  
	- Update the wsHost variable in the default.js file with the same IP address.  The default.js file is 
	  located in the RealTimeDashboard\js folder.  
	- Start the Twitter Client application first, then start the real-time dashboard after so that it can
	  connect to the server web sockets created by the SI client application

The StreamInsight application need to write tweet header information to Azure Database.  Connect to your Azure Database and then
execute the "Create Azure DB Objects.sql" to create the database table and stored procedure required by the Azure Database Output Adapter. 
Remember to update the app.config file in the StreamInsight.Demos.Twitter.Client project and provide the correct connection
information to the Azure database.

The SI application also save tweets in the original JSON format to Azure Blob Storage.  You'll need to create a blob storage on
Azure and then create a container in the storage account.  I recommend installing a free tool call CloudXplorer to be able to 
view the files in your storage account.  Also remember to provide the account name and key in the appsettings section of 
the SI client app.config file.

Once StreamInsight application saves tweets to Azure Blog Storage and Azure Database, then perform the following steps on 
HadoopOnAzure in the order listed:
- Map the Azure Blob Storage to HOA.
	- Log into your HOA cluster and click on "Manage Cluster".
	- Click on "Set up ASV"
	- Specify your Azure Storage Account Name and Passkey.  You get these fields from your Azure Portal.  This is the same info
	  you specify in the StreamInsight.Demos.Twitter.Client app.config file
	- In the HOA home page, click on the Open Ports icon and then open the ODBC Server port so that you can connect to the cluster
	  using the ODBC driver later.
- Remote Desktop to the Hadoop head node.  Launch the Hadoop commnd prompt and run the following commands:
	- Import the Azure Database table which contains twitter header information into Hive.  Replace the server, database, and 
	  user credential with your own.  Here, I'm 
		- c:\apps\dist\sqoop\bin\sqoop import --connect "jdbc:sqlserver://sql_azure_server_name_here.database.windows.net;username=your_sql_azure_username_here@sql_azure_server_name_here;password=your_sql_azure_password_here;database=your_db_name_here" --table TweetInfo --hive-import -hive-overwrite
	- If you need to run the above sqoop commands multiple times, then run the following commands to clean things out:
		- hive -e "drop table tweetinfo;"
		- del tweetinfo.java
	- Get tweets from Azure Blob Storage and save to HDFS.  This may take awhile.  I also have code in "Analyze Tweets with Hive.txt",
	  which I commented, that will you to create an external Hive table that reads directly from Azure Blob Storage.
		- del /q c:\twitter
		- hadoop fs -get asv://twitter/ C:\twitter
		- hadoop fs -put c:\twitter /twitter
	- Process data using Hive.  The Hive queries are in the file "Analyze Tweets with Hive.txt".  Copy this file to the c:\ drive on HOA 
	  head node and then run the following command(It is possible to share your clipboard with the headnode).  The results will be saved in a few hive tables.  Read the queries in that file to find 
	  out more. 
	    - cd C:\apps\dist\hive-0.9.0\bin
		- hive.cmd
		- hive -f "c:\Analyze Tweets with Hive.txt"
	- The above scripts will generate a series of hive tables.  You will pull data from those tables into PowerPivot using
	  the queries listed in the file "Hive Queries for ODBC Driver.txt".  There are currently some issues with the way
	  the Hive ODBC driver works, so you'll need the workaround specified in those scripts.
	- Inside PowerPivot, you'll need to create relationships between table and also add "related" fields into the tweet_details 
	  table.  See the Hive Results - PP 2010.xlsx to see how the PP data model is created.
	- Once the PP model is created, save it into SharePoint 2010 and create the PowerView report.  A picture of what the Power
	  View report looks like is included in the "Logical Architecture Diagram.vsdx" file.  It was created using Visio 2013.



