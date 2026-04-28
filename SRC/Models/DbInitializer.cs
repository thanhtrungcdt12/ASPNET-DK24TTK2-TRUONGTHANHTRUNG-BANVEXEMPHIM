using Microsoft.Data.SqlClient;

namespace BanVeXemPhim.Models
{
    public static class DbInitializer
    {
        private const string DbName = "TruongThanhTrungTiket";
        private const string MasterConnectionString =
            "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

        public static void RestoreFromBackup(string contentRootPath)
        {
            var bakPath = Path.Combine(contentRootPath, $"{DbName}.bak");
            if (!File.Exists(bakPath))
                throw new FileNotFoundException($"Không tìm thấy file backup: {bakPath}");

            using var conn = new SqlConnection(MasterConnectionString);
            conn.Open();

            if (DatabaseExists(conn)) return;

            var (dataLogical, logLogical) = GetLogicalFileNames(conn, bakPath);
            var dataDir = Path.GetDirectoryName(bakPath)!;
            var mdfPath = Path.Combine(dataDir, $"{DbName}.mdf");
            var ldfPath = Path.Combine(dataDir, $"{DbName}_log.ldf");

            using var cmd = conn.CreateCommand();
            cmd.CommandTimeout = 120;
            cmd.CommandText = $@"
                RESTORE DATABASE [{DbName}]
                FROM DISK = N'{bakPath}'
                WITH MOVE N'{dataLogical}' TO N'{mdfPath}',
                     MOVE N'{logLogical}' TO N'{ldfPath}',
                     REPLACE";
            cmd.ExecuteNonQuery();
        }

        private static bool DatabaseExists(SqlConnection conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT DB_ID(N'{DbName}')";
            var result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value;
        }

        private static (string DataLogical, string LogLogical) GetLogicalFileNames(SqlConnection conn, string bakPath)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"RESTORE FILELISTONLY FROM DISK = N'{bakPath}'";
            using var reader = cmd.ExecuteReader();

            string dataLogical = "", logLogical = "";
            while (reader.Read())
            {
                var type = reader["Type"].ToString();
                var logical = reader["LogicalName"]?.ToString() ?? "";
                if (type == "D") dataLogical = logical;
                else if (type == "L") logLogical = logical;
            }
            return (dataLogical, logLogical);
        }
    }
}
