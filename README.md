# QuickDevDotnet
QuickDevDotnet is a Helper dll for dotnet developer contain basic function for creating software.

### 1. Data Access
   - **ConnectDatabase** : boolean method to check database connection, if no database connection has been configured, it will return false. this method will also initiate connection from ypur application to database.
   - **SaveConnection** : method to save a database connection to configuration file, you can use this method in your database connection setting form.
   - **GetSingleValue** : method to read database from query and return a single value.
   - **GetData** : method for reading database from query and return a DataTable Object.
   - **ExecQuery** : method for executing query (ExecuteNonQuery).
   - **ExportData** : method for exporting query result to csv format.
### 2. App Security
   - EncrypText : method for encrypting text.
   - DecrypText : method for decrypt text.
   - EncryptFile : method for encrypt a file.
   - DecryptFile : method for decrypt a file.
### 3. 
