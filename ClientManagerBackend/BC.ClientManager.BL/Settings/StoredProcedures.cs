namespace BC.ClientManager.BL.Settings
{
    internal class StoredProcedures
    {
        // ------------Clients stored procedures--------------
        public const string CreateClient = "dbo.sp_CreateClient";
        public const string UpdateClient = "dbo.sp_UpdateClient";
        public const string GetClients = "dbo.sp_GetClients";
        public const string GetClientById = "sp_GetClientById";
        public const string LinkContactToClient = "dbo.sp_LinkContactToClient";
        public const string UnlinkContactFromClient = "dbo.sp_UnlinkContactFromClient";
        public const string GetContactsByClient = "dbo.sp_GetContactsByClient";

        // -------------Contacts stored procedures-------------
        public const string CreateContact = "dbo.sp_CreateContact";
        public const string GetContacts = "dbo.sp_GetContacts";
        public const string GetContactById = "dbo.sp_GetContactById";
        public const string GetClientsByContact = "dbo.sp_GetClientsByContact";

    }
}
