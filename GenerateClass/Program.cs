using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GenerateClass {
    class Program {
        static void Main(string[] args) {

            List<TableInfo> tableInfo;

            using (var conn = new SQLiteConnection("Data Source=sample.db")) {
                conn.Open();

                CreateTable(conn);

                tableInfo = GetTableInfo(conn);

                conn.Close();
            }
            
            new Generate().GenerateEntity("Namespace1", "EmployeeEntity", tableInfo);
            new Generate().GenerateHelloWorldExe();
        }



        /// <summary>
        /// employeeテーブルを作成
        /// </summary>
        /// <param name="conn"></param>
        static void CreateTable(SQLiteConnection conn) {

            using (var command = conn.CreateCommand()) {
                command.CommandText = @"
CREATE TABLE IF NOT EXISTS employee (
    id INTEGER PRIMARY KEY AUTOINCREMENT
   ,name TEXT
   ,age INTEGER
   ,height REAL
   ,weight REAL
)";
                command.ExecuteNonQuery();
            }

        }

        /// <summary>
        /// employeeテーブルの列情報を取得
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        static List<TableInfo> GetTableInfo(SQLiteConnection conn) {

            using (var command = conn.CreateCommand()) {
                //employeeテーブル情報取得
                command.CommandText = "PRAGMA TABLE_INFO(employee);";

                var result = new List<TableInfo>();

                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        //name: カラム名, type: データ型
                        var tableInfo = new TableInfo {
                            ColumnName = reader["name"].ToString(),
                            DataType = reader["type"].ToString()
                        };
                        result.Add(tableInfo);
                    }
                }

                return result;
            }

        }
    }
}
