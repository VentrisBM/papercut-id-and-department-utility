# papercut-id-generation-utility
Generates ID numbers for existing PaperCut users.

At the time of creation of this application, PaperCut did not support automatic ID generation 
containing both characters and digits (one could only generate login ID's of n length containing only digits).

I wrote this utility application in order to fulfill a client's request. The application uses WPF for the GUI,
and RNGCryptoServiceProvider for generating unique and secure ID's of sufficient randomness. 
PaperCut's proprietary XML-RPC proxy (ServerCommandProxy.cs) is also utilized but with a custom made utility wrapper.

The rest of the application logic is located in the "btnUpdate_Click" event handler, which I suppose is not best practice,
but it got the job done and time was of the essence.