# papercut-id-and-department-utility
Generates ID numbers for existing PaperCut users and performs AD-PaperCut department synchronization.

I wrote this utility application in order to fulfill several client requests. The application uses WPF for the GUI, RNGCryptoServiceProvider for generating unique and secure IDs of sufficient randomness, and DirectoryServices for connecting to LDAP. PaperCut's proprietary XML-RPC proxy (ServerCommandProxy.cs) is also utilized for communication with PaperCut but with a custom made utility wrapper. The rest of the application logic is written in the PaperCutHelper and LdapHelper classes.

The application had to fulfill the following requirements:
  1. Generate ID's containing both characters and digits of sufficient randomness - PaperCut does not support this, it can only       generate ID's containing digits of n length . The user must be able to choose which PaperCut ID field will be populated.
  2. Send ID's via email on demand. PaperCut does not support this either, one can only send emails during initial ID
      generation and not manually afterwards.
  3. Manually select the AD property which contained the department number and/or department name and populate the corresponding       PaperCut property. PaperCut currently uses a predefined "Department" and "Office" sync from AD, but if the client has its       department information written in a custom property PaperCut is not able to retrieve that information by default.
