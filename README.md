#TicketTest

##Description
Diagnostic tool to test Qlik Sense Proxy authentication API for tickets. The tool lets you request a ticket and get detailed debug information back on the progress

##Installation:

If running on the same host as proxy no need for installation just run the tool with the following syntax
TicketTest.exe R localhost [user] [userdirectory]

If run from a remote machine you will need to export the client certificate from the proxy and import them to the host:
1. Go to QMC and export certificate for host that you intend to test requesting a ticket from. Select to include the private key and use a password.
2. Copy certificates from C:\ProgramData\Qlik\Sense\Repository\Exported Certificates\[host] to the host you are testing from
3. Import the client.pfx certificate into the certificate store for the user you will run the tool as


##Example
´´´
TicketTest.exe R localhost test testdir

Loading certificate to use for ticket request
***********************************************************************
Certificate found
[Subject]
  CN=QlikClient

[Issuer]
  CN=rd-flp2.qliktech.com-CA

[Serial Number]
  00B3EBFF3BD1F20088BED2774540DE4B

[Not Before]
  2014-08-05 10:42:58

[Not After]
  2024-08-12 10:42:58

[Thumbprint]
  5BC943D22C18D6C5F2C5B4059B99252E93607481

Certificate Loaded

Requesting Ticket
***********************************************************************

URL
------------------------
https://localhost:4243/qps/ticket?Xrfkey=0123456789abcdef

Request Headers
------------------------
Accept: application/json
X-Qlik-Xrfkey: 0123456789abcdef
Content-Type: application/json



Body
{ 'UserId':'test','UserDirectory':'testdir','Attributes': []}

Response
***********************************************************************

Ticket
------------------------

{"UserDirectory":"TESTDIR","UserId":"test","Attributes":[],"Ticket":"swiaZulzKHl
fJfzr","TargetUri":null}
´´´