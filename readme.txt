Backend:
==========================
implemented relationships in core model 
just need to install EF and code first model to store in DB 

1)need to implement Elmah for logging 
2)Validate JSON data models before processing 
3)Save processed requests in the file system for now before saving it to DB 
4)Implement code first to store data in sql db
5)Need to change requests to asynch calls in code
===========================

Front End:-
exists:
1) Add bus/add/per data on startup this stores in local storage
2)Dashboard is empty first populate with dumy data 
3)Submit will read period details from local storage and user needs to fill
vat data and submit for process 
	1) First Submission request if this is sucesss send another request to poll(inline request) to check status 
		if any errors in submission request the respons send backs to client(hmrc response object),
		if any errors in poll request the response send back to client(hmrc response object) with deletion request and resumit as new request 
		if sucess then send back success object to cliet to store in local storage and update next period model (next quater)
From this we should build Dash board stats - Pending 


TODO: 
From sucess object IRmark field needs to be disaplyed to user 
And capture VAT period details to build dashboard from sucessresponse.Responsedata and extract body content and seralize to VATResponse object and store in
client side


20/04/2017
work on dash board, create object from pervm and hmrc response and store in array and push to local storage and build dash board based on figures

03/05/2017 = change serialization code to xdoc code as its not working at the momemnt  --- Done 
16/05/2017 = deployed to azure and working sucessfully, implemented log in api server
TODO:// work on front end and dash board to look better
23/05/2017: Created models and storig in local storage, 
TODO: build view model to display payment stuff when user click on each submission like list example
TODO:api code add company + vat no to create folder to check if this alredy exist 

 
14/07/2017
=======================
Convert VAT100 class prop to decimal as its string at the moment 

added valid check, this can be implemented in later future 
31/08/2017
=======================
Implement disbale/enable log feature 
Implement membership feature 

11/09/2017
========================
Installed EF and added data annotation to support code first 
created services, repository, infrastructure model with in solution
just need to add unity and inject service interfaces to UI

