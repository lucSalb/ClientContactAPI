using ClientContactAPI.Classes;
using ClientContactAPI.Interfaces;
using Microsoft.Data.SqlClient;

namespace ClientContactAPI.Services
{
    public class TemplateService : ITemplate
    {
        private static IConfiguration? _configuration;
        public TemplateService()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();
        }
        public async Task<bool> CreateTemplateDB()
        {
            var connString = _configuration.GetConnectionString("connString");
            string queryString = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TemplatesTABLE' )
                                    BEGIN
                                        CREATE TABLE [dbo].[TemplatesTABLE](
                                            [Id]      NVARCHAR (50) NOT NULL,
                                            [Name]    NVARCHAR (80) NOT NULL,
                                            [Subject] NVARCHAR (150) NOT NULL,
                                            [Body]    NVARCHAR (255) NOT NULL,
                                        )
                                    END";

            using (SqlConnection connection = new SqlConnection(connString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            return true;
        }
        public async Task<List<Template>> GetTemplates()
        {
            List<Template> templates = new List<Template>();

            var connString = _configuration.GetConnectionString("connString");
            string sqlQuery = @"SELECT * FROM TemplatesTABLE;";

            using (SqlConnection connection = new SqlConnection(connString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            templates.Add(new Template()
                            {
                                Id = Utils.PrettyTextDisplay(reader["Id"].ToString()),
                                Name = Utils.PrettyTextDisplay(reader["Name"].ToString()),
                                Subject = Utils.PrettyTextDisplay(reader["Subject"].ToString()),
                                Body = reader["Body"].ToString()
                            });
                        }
                    }
                }
            }

            return templates;
        }
        public async Task<Template?> GetTemplate(string id)
        {
            Template? template = null;

            var sqlQuery = @"SELECT * FROM TemplatesTABLE WHERE Id = UPPER(@Id);";
            var connString = _configuration.GetConnectionString("connString");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            template = new Template()
                            {
                                Id =  Utils.PrettyTextDisplay(reader["Id"].ToString()),
                                Name = Utils.PrettyTextDisplay(reader["Name"].ToString()),
                                Subject = Utils.PrettyTextDisplay(reader["Subject"].ToString()),
                                Body = reader["Body"].ToString()
                            };
                        }
                    }
                }
            }

            return template;
        }
        public async Task<APIResponse> RegisterTemplate(Template template)
        {
            var sqlConnection = _configuration.GetConnectionString("connString");
            var countQuery = @"SELECT COUNT(*) FROM TemplatesTABLE WHERE Name = UPPER(@Name) AND Subject = UPPER(@Subject) AND Body = @Body";
            var sqlQuery = @"INSERT INTO TemplatesTABLE (Id, Name, Subject, Body) VALUES (UPPER(@Id), UPPER(@Name), UPPER(@Subject), @Body);";

            using (SqlConnection connection = new SqlConnection(sqlConnection))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(countQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", template.Name);
                    command.Parameters.AddWithValue("@Subject", template.Subject);
                    command.Parameters.AddWithValue("@Body", template.Body);
                    int count = (int)await command.ExecuteScalarAsync();
                    if (count > 0) return new APIResponse() { Success = false, Message = "Template already registered in the system.", Data = null };
                }

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", template.Id);
                    command.Parameters.AddWithValue("@Name", template.Name);
                    command.Parameters.AddWithValue("@Subject", template.Subject);
                    command.Parameters.AddWithValue("@Body", template.Body);
                    await command.ExecuteNonQueryAsync();
                }
            }

            template.Id = Utils.PrettyTextDisplay(template.Id);
            template.Name = Utils.PrettyTextDisplay(template.Name);
            template.Subject = Utils.PrettyTextDisplay(template.Subject);
            return new APIResponse() { Success = true, Message = "Template successfuly registered.", Data = template };
        }
        public async Task<APIResponse> UpdateTemplate(Template template)
        {
            APIResponse result = new APIResponse() { Success = true, Message = "Customer data updated.", Data = template };
            try
            {
                var sqlConnection = _configuration.GetConnectionString("connString");
                var countQuery = @"SELECT COUNT(*) FROM TemplatesTABLE WHERE (Name = UPPER(@Name) AND Subject = UPPER(@Subject) AND Body = @Body) AND Id <> UPPER(@Id) ";
                var sqlQuery = @"UPDATE TemplatesTABLE SET Name = UPPER(@Name), Subject = UPPER(@Subject), Body = @Body WHERE Id = UPPER(@Id);";

                using (SqlConnection connection = new SqlConnection(sqlConnection))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(countQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", template.Id);
                        command.Parameters.AddWithValue("@Name", template.Name);
                        command.Parameters.AddWithValue("@Subject", template.Subject);
                        command.Parameters.AddWithValue("@Body", template.Body);
                        int count = (int)await command.ExecuteScalarAsync();

                        if (count > 0) return new APIResponse() { Success = false, Message = "The provided information already exists in the system.", Data = null };
                    }

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", template.Id);
                        command.Parameters.AddWithValue("@Name", template.Name);
                        command.Parameters.AddWithValue("@Subject", template.Subject);
                        command.Parameters.AddWithValue("@Body", template.Body);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                template.Id = Utils.PrettyTextDisplay(template.Id);
                template.Name = Utils.PrettyTextDisplay(template.Name);
                template.Subject = Utils.PrettyTextDisplay(template.Subject);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<APIResponse> DeleteTemplate(string id)
        {
            APIResponse result = new APIResponse() { Success = true, Message = "Template removed.", Data = null };
            var sqlConnection = _configuration.GetConnectionString("connString");
            var sqlQuery = @"DELETE FROM TemplatesTABLE WHERE Id = UPPER(@Id);";
            using (SqlConnection connection = new SqlConnection(sqlConnection))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int count = await command.ExecuteNonQueryAsync();
                    if (count <= 0)
                    {
                        result = new APIResponse() { Success = false, Message = "Template information not found.", Data = null };
                    }
                }
            }
            return result;
        }
                    
    }
}
